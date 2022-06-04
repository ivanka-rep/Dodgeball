using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Configs.GameConfig;
using DataSakura.JoyStick.Scripts;
using Items;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThrowingController : MonoBehaviour
{
    [SerializeField] private APRController m_aprController;

    [Header("Game Params")] 
    [SerializeField] private Transform m_resetBallPos;
    [SerializeField] private Transform m_middleResetBallPos;
    [SerializeField] private string m_thisPlayerBallLayer = "";
    
    [Header("Control joystick")] [Tooltip("Joystick for control rotation and aiming")]
    [SerializeField] private JoyStick m_controlJoystick;
    
    [Header("Ball positions in hands")] 
    public Transform ballPosRight;
    public Transform ballPosLeft;
    
    public GameObject CurrentBall { get; set; }
    public JoyStick ControlJoystick => m_controlJoystick;
    public string ThisPlayerBallLayer => m_thisPlayerBallLayer;
    public float ChangingAdditionalForce => m_additionalForce;

    [HideInInspector] public bool reachRight, reachLeft, prepareRight, prepareLeft;
    [HideInInspector] public bool hasThrown, ballConnectedRight, ballConnectedLeft;
    [HideInInspector] public bool throwRight, throwLeft;
    [HideInInspector] public Rigidbody ball;

    private GameManager m_gameManager;
    private GameData m_gameData;
    private DifficultyParams m_difficultyParams;
    
    // private Slider m_powerMeter;
    
    private Bot m_bot;
    private GameObject[] m_APR_Parts;
    private Quaternion m_bodyTarget, m_upperRightArmTarget, m_lowerRightArmTarget, m_upperLeftArmTarget, m_lowerLeftArmTarget;
    
    private float m_handPushForce = 15f;
    private float m_additionalForce = 1f;
    private float m_multiply = 2f;
    
    public void SetAprVars(GameObject[] aprParts) // Start
    {
        m_APR_Parts = aprParts;

        m_bodyTarget = aprParts[0].GetComponent<ConfigurableJoint>().targetRotation;
        m_upperRightArmTarget = aprParts[3].GetComponent<ConfigurableJoint>().targetRotation;
        m_lowerRightArmTarget = aprParts[4].GetComponent<ConfigurableJoint>().targetRotation;
        m_upperLeftArmTarget = aprParts[5].GetComponent<ConfigurableJoint>().targetRotation;
        m_lowerLeftArmTarget = aprParts[6].GetComponent<ConfigurableJoint>().targetRotation;
    }

    public void SetGameVars()
    { 
        m_gameManager = GameManager.s_instance;
        m_gameData = GameData.s_instance;
        m_difficultyParams = m_gameManager.DifficultyParams;
        m_bot = gameObject.GetComponentInChildren<Bot>();
        m_bot.SetGameVars();
    }

    private void Update()
    {
        if ((m_aprController.useControls || m_aprController.isBot) && !m_aprController.inAir)
        {
            HandsControl();
        }

        // Controls for debug
        if (Input.GetKey(KeyCode.T) && ballConnectedLeft) throwLeft = true;
        if (Input.GetKey(KeyCode.T) && ballConnectedRight) throwRight = true;
    }

    private void HandsControl()
    {
        //throw right
        if ( ballConnectedRight && !throwLeft && prepareRight)
        {
            prepareRight = false;

            m_APR_Parts[3].GetComponent<ConfigurableJoint>().targetRotation = m_upperRightArmTarget;
            m_APR_Parts[4].GetComponent<ConfigurableJoint>().targetRotation = m_lowerRightArmTarget;

            //Right hand throw pull back pose
            m_APR_Parts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, -0.15f, 0, 1);
            m_APR_Parts[3].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.62f, -0.6f, -0.5f, 1);
            m_APR_Parts[4].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0f, -1f, -0.02f, 1);
        }

        if (ballConnectedRight && throwRight)
        {
            throwRight = false;
            IgnoreCollisionsByBall(true);
            StartCoroutine(ThrowBall(m_aprController.RightHand));
            
            //Right hand throw release pose
            m_APR_Parts[1].GetComponent<ConfigurableJoint>().targetRotation = m_bodyTarget;
            m_APR_Parts[3].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(1.2f, 0f, 0.04f, 1);
            m_APR_Parts[4].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0f, -0.5f, 0.02f, 1);

            //Right hand throw force
            m_aprController.RightHand.AddForce(m_APR_Parts[0].transform.forward * m_handPushForce, ForceMode.Impulse);
            StartCoroutine(DelayAction(RightHandToTarget, 0.4f));
        }


        //throw left
        if (ballConnectedLeft && !throwRight && prepareLeft)
        {
            prepareLeft = false;

            m_APR_Parts[5].GetComponent<ConfigurableJoint>().targetRotation = m_upperLeftArmTarget;
            m_APR_Parts[6].GetComponent<ConfigurableJoint>().targetRotation = m_lowerLeftArmTarget;

            //Left hand throw pull back pose
            m_APR_Parts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, 0.15f, 0, 1);
            m_APR_Parts[5].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0.62f, -0.6f, 0.5f, 1);
            m_APR_Parts[6].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0f, -1f, 0.02f, 1);
            
        }

        if (ballConnectedLeft && throwLeft)
        {
            throwLeft = false;
            IgnoreCollisionsByBall(true);
            StartCoroutine(ThrowBall(m_aprController.LeftHand));
            
            //Left hand throw release pose
            m_APR_Parts[1].GetComponent<ConfigurableJoint>().targetRotation = m_bodyTarget;
            m_APR_Parts[5].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-1.2f, 0f, -0.04f, 1);
            m_APR_Parts[6].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0f, -0.5f, -0.02f, 1);

            //Left hand throw force
            m_aprController.LeftHand.AddForce(m_APR_Parts[0].transform.forward * m_handPushForce, ForceMode.Impulse);
            StartCoroutine(DelayAction(LeftHandToTarget, 0.4f));
        }
    }

    private IEnumerator ThrowBall(Rigidbody hand)
    {
        FixedJoint fj = hand.GetComponent<FixedJoint>();
        
        //Debug.Log("Ball connected R_L " + ballConnectedRight + " " + ballConnectedLeft);
        if (ball == null ||fj == null || fj.connectedBody != ball || !( ballConnectedRight || ballConnectedLeft ))
        {IgnoreCollisionsByBall(false); yield break;}
        
        Ball ballCtrl = ball.GetComponent<Ball>();
        Transform ballTransform = ball.transform;
        ItemBall itemBall = (ItemBall)m_gameData.Items.Find(item => item.ItemId == m_gameData.UserData.CurrentBallId);
        
        ballCtrl.BallCanHit = true;
        hasThrown = true;

        DetachBall(fj, hand.GetComponent<HandContact>());
        ball.velocity = Vector3.zero;
        ball.transform.rotation = Quaternion.LookRotation(m_APR_Parts[0].transform.forward);

        Vector3 dir = m_APR_Parts[0].transform.forward;
        float throwForce =  m_difficultyParams.playerThrowForce;
        float distance = (itemBall.GetDistance() / 100f) + 1f; // Numbers between 1 and 2; 
        float speed = (itemBall.GetSpeed() / 100f) + 1f; // Numbers between 1 and 2; 

        if (m_aprController.isBot)
        {
            dir = (m_bot.lookAtTransform.position - ball.position ) + ballTransform.up;
            throwForce = m_difficultyParams.botThrowForce; 
            speed = 1f; distance = 2f;
        }
        
        ball.velocity = (dir * (throwForce * speed * m_additionalForce) + Vector3.up * distance);
        m_additionalForce = 1f;
        
        if (ball.velocity.magnitude >= 20f) ballCtrl.BallCanStun = true;
        yield return new WaitForSeconds(1.5f);
        
        ballCtrl.BallCanHit = false; hasThrown = false;
        IgnoreCollisionsByBall(false);
        StartCoroutine(ballCtrl.DestroyBall());
    }

    private void IgnoreCollisionsByBall(bool ignore)
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer(m_aprController.thisPlayerLayer), LayerMask.NameToLayer(m_thisPlayerBallLayer), ignore);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LimitWall"), LayerMask.NameToLayer(m_thisPlayerBallLayer), ignore);
    }
    
    private void DetachBall(FixedJoint fj, HandContact handCont)
    {
        Destroy(fj);
        
        handCont.hasBallJointLeft = false; handCont.hasBallJointRight = false;
        ballConnectedRight = false; ballConnectedLeft = false;
        reachLeft = false; reachRight = false;
        
        if (m_thisPlayerBallLayer == "Ball_Player_1")
        {
            ball.GetComponent<TrajectorySimulation>().enabled = false;
            m_gameManager.SpheresPool.gameObject.SetActive(false);
            m_controlJoystick.gameObject.SetActive(false);
            m_aprController.ControlHorizontal = 0f;
            m_aprController.ControlVertical = 0f;
        }
    }

    // Input space
    
    public void OnPointerDown()
    {
        if (CanInput() == false) return;
        m_aprController.ThrowOriginRotation = m_APR_Parts[0].GetComponent<ConfigurableJoint>().targetRotation;
    }

    public void OnDrag()
    {
        if (CanInput() == false) return;
        
        float horizontal = m_controlJoystick.Horizontal;
        float vertical = m_controlJoystick.Vertical;
        
        m_aprController.ControlHorizontal = horizontal;
        m_aprController.ControlVertical = vertical;
        
        if (horizontal < 0) horizontal = -horizontal; if (vertical < 0) vertical = -vertical;
        if (horizontal != 0 && vertical != 0) m_additionalForce =( (horizontal + vertical) / 2f  + 0.5f ) * m_multiply;
        else m_additionalForce = (horizontal + vertical + 0.5f) * m_multiply;
    }

    public void OnPointerUp()
    {
        if (CanInput() == false) return;
        
        if (ballConnectedRight) throwRight = true;
        if (ballConnectedLeft) throwLeft = true;
                
        m_controlJoystick.gameObject.SetActive(false);
        m_gameManager.SpheresPool.gameObject.SetActive(false);

        // throw with more than 80% of power will stun.
        if (m_additionalForce >= m_multiply * 1.2f) ball.GetComponent<Ball>().BallCanStun = true;
    }

    private bool CanInput()
    {
        if (m_aprController.useControls || TutorialManager.s_instance.GuideActive && !m_aprController.isBot && !m_aprController.inAir)
        { return (ballConnectedRight || ballConnectedLeft); }

        return false;
    }
    
    
    public void LeftHandToTarget()
    {
        m_APR_Parts[5].GetComponent<ConfigurableJoint>().targetRotation = m_upperLeftArmTarget;
        m_APR_Parts[6].GetComponent<ConfigurableJoint>().targetRotation = m_lowerLeftArmTarget;
    }

    public void RightHandToTarget()
    {
        m_APR_Parts[3].GetComponent<ConfigurableJoint>().targetRotation = m_upperRightArmTarget;
        m_APR_Parts[4].GetComponent<ConfigurableJoint>().targetRotation = m_lowerRightArmTarget;
    }

    public void CreateNewBall()
    {
        bool isPlayer = m_aprController.thisPlayerLayer == "Player_1";
        bool isBallInMiddle = (m_difficultyParams.difficulty == DifficultyType.Easy &&
                               (isPlayer || m_aprController.thisPlayerLayer == "Player_4") ||
                               (m_difficultyParams.difficulty == DifficultyType.Normal && m_aprController.thisPlayerLayer == "Player_1"));

        Vector3 pos = isBallInMiddle ? m_middleResetBallPos.position : m_resetBallPos.position;
        GameObject newBall = isPlayer ? m_gameManager.PlayerBallPrefab : m_gameManager.EnemyBallPrefab;
        
        CurrentBall = Instantiate(newBall, pos, Quaternion.identity);
        CurrentBall.layer = LayerMask.NameToLayer(m_thisPlayerBallLayer);
        CurrentBall.GetComponent<Ball>().ThrowingPlayer = this.gameObject;

        if (isPlayer)
        {
            ItemBall itemBall = (ItemBall) m_gameData.Items.Find(
                item => item.ItemId == m_gameData.UserData.CurrentBallId);
            Material ballMat = itemBall.GetMaterial();
            CurrentBall.GetComponent<MeshRenderer>().material = ballMat;
            CurrentBall.GetComponent<TrailRenderer>().material.color = ballMat.color;
        }
        else
        {
            m_bot.CanThrow = true;
            m_bot.StartActivity();
        }
    }

    public void DropTheBall()
    {
        FixedJoint fjLeft = m_aprController.LeftHand.GetComponent<FixedJoint>();
        FixedJoint fjRight = m_aprController.RightHand.GetComponent<FixedJoint>();
        
        if (ball == null || !( ballConnectedRight || ballConnectedLeft )) return;

        if (fjLeft != null && ballConnectedLeft) { DetachBall(fjLeft, m_aprController.LeftHand.GetComponent<HandContact>()); }
        if (fjRight != null && ballConnectedRight) { DetachBall(fjRight, m_aprController.RightHand.GetComponent<HandContact>()); }
        
        // cause the animation is enabled, we also can't take the ball, but for future we make this variable true
        ball.GetComponent<Ball>().CanTakeBall = true;
        IgnoreCollisionsByBall(false);
    }

    private IEnumerator DelayAction(Action act, float time)
    {
        yield return new WaitForSeconds(time);
        act.Invoke();
    }
    
   
}
