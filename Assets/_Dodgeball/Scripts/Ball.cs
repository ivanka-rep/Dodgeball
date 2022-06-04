using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [HideInInspector] public GameObject ThrowingPlayer;
    [HideInInspector] public bool BallCanHit = false;
    [HideInInspector] public bool BallCanStun = false;
    public bool CanTakeBall { get => m_canTakeBall; set => m_canTakeBall = value; }
    
    private bool m_hasCollided = false;
    private bool m_canTakeBall = true;
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground")) StartCoroutine(DestroyBall());
        if (false == GameManager.s_instance.gameStarted || null == ThrowingPlayer) return;

        List<GameObject> players = GameManager.s_instance.Players;
        GameObject curPlayer = ThrowingPlayer;
        APRController curPlayerAPR = curPlayer.GetComponent<APRController>();
        string currentPlayerLayer = curPlayerAPR.thisPlayerLayer;

        TakeBallByBot(col, curPlayerAPR, currentPlayerLayer);

        if (col.gameObject.layer == LayerMask.NameToLayer("Player_1") && currentPlayerLayer == "Player_1")
        {
            var curThrowCtrl = curPlayer.GetComponent<ThrowingController>();
            if (m_canTakeBall && !curPlayer.GetComponent<KnockOutAnimation>().AnimationActive)
            { m_canTakeBall = false; SetBallInHands(curThrowCtrl, true);}
        }
        
        bool isPlayer = false;
        foreach (GameObject player in players)
        {
            if (player.layer == col.gameObject.layer) 
            { isPlayer = true; break;}
        }
        
        if (!isPlayer || col.gameObject.layer == LayerMask.NameToLayer(currentPlayerLayer) || 
            (currentPlayerLayer != "Player_1" && LayerMask.LayerToName(col.gameObject.layer) != "Player_1")) // bot can't hit his mate
            return;
        
        APRController hitPlayerApr = col.gameObject.GetComponentInParent<APRController>();
        ThrowingController playerThrowCtrl = curPlayer.GetComponent<ThrowingController>();

        if (playerThrowCtrl.ballConnectedRight || playerThrowCtrl.ballConnectedLeft || !BallCanHit || m_hasCollided) return;

        m_hasCollided = true;
        Physics.IgnoreLayerCollision(ThrowingPlayer.layer, LayerMask.NameToLayer(playerThrowCtrl.ThisPlayerBallLayer), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LimitWall"), LayerMask.NameToLayer(playerThrowCtrl.ThisPlayerBallLayer), false);

        StartCoroutine(PlayHitEffect(hitPlayerApr.Root.transform, hitPlayerApr));

        KnockOutAnimation knockAnim = hitPlayerApr.gameObject.GetComponent<KnockOutAnimation>();
        if (!knockAnim.AnimationActive && (BallCanStun || col.gameObject.GetComponent<ConfigurableJoint>() == hitPlayerApr.Head.GetComponent<ConfigurableJoint>() ))
        {
            knockAnim.PlayAnim();
            BallCanStun = false;
        }
        
        GameManager.s_instance.AddScore(currentPlayerLayer == "Player_1");
    }

    private void TakeBallByBot(Collision col, APRController curPlayerAPR, string curPlayerLayer )
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(curPlayerLayer) && curPlayerAPR.isBot)
        {
            Bot botCtrl = ThrowingPlayer.GetComponentInChildren<Bot>();
            ThrowingController botThrowCtrl = ThrowingPlayer.GetComponent<ThrowingController>();
                
            if (botCtrl.CanThrow && botCtrl.goingToBall && !botThrowCtrl.hasThrown)
            {
                botCtrl.forwardBackward = 0f;
                botCtrl.goingToBall = false;
                botCtrl.isNeedAim = false;
                    
                //SET BALL IN BOT'S HANDS
                SetBallInHands(botThrowCtrl, false);
            }
        }
    }
    private void SetBallInHands(ThrowingController throwCtrl, bool isPlayer)
    {
        Vector3 pos = new Vector3();
        if (Random.Range(0f, 1f) > 0.5f)
        { pos = throwCtrl.ballPosRight.position;
            throwCtrl.reachRight = true; }
        else 
        { pos = throwCtrl.ballPosLeft.position;
            throwCtrl.reachLeft = true; }
                    
        float t = 0f;
        while (t < 1f)
        { t += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, pos, t); }

        StartCoroutine(isPlayer
            ? ActivateBall(throwCtrl)
            : ThrowBallByBot(throwCtrl, ThrowingPlayer.GetComponentInChildren<Bot>()));
    }

    private IEnumerator ActivateBall(ThrowingController throwCtrl)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (throwCtrl.ballConnectedLeft) throwCtrl.prepareLeft = true;
        else if (throwCtrl.ballConnectedRight) throwCtrl.prepareRight = true;
        else 
        { throwCtrl.reachRight = false; throwCtrl.reachLeft = false;
            m_canTakeBall = true; Debug.Log("Ball isn't connected");  yield break; }
        
        yield return new WaitForSeconds(0.1f);
        
        throwCtrl.ControlJoystick.gameObject.SetActive(true);
        gameObject.GetComponent<TrajectorySimulation>().enabled = true;
        GameManager.s_instance.SpheresPool.gameObject.SetActive(true);
        
        if (TutorialManager.s_instance != null && TutorialManager.s_instance.GuideActive) TutorialManager.s_instance.StartGuide2();
        
    }
    private IEnumerator ThrowBallByBot(ThrowingController botThrowCtrl, Bot botCtrl)
    {
        botCtrl.forwardBackward = 0f;
        
        yield return new WaitForSeconds(0.25f);
        
        if (botThrowCtrl.ballConnectedRight || botThrowCtrl.ballConnectedLeft)
            botCtrl.BallThrowing();
        else
        {
            botThrowCtrl.reachRight = false; botThrowCtrl.reachLeft = false;
            botCtrl.StartActivity();
        }
    }

    public IEnumerator DestroyBall()
    {
        if (gameObject != null)
        {
            Destroy(gameObject, 0.5f);
            yield return new WaitForSeconds(0.5f);
            ThrowingPlayer.GetComponent<ThrowingController>().CreateNewBall();
        }
    }

    private IEnumerator PlayHitEffect(Transform parent, APRController hitPlayerApr)
    {
        Vector3 pos = parent.position; pos.y = 3f; // set y pos to ground
        GameObject effect = Instantiate(GameManager.s_instance.HitParticleEffectPrefab, pos, Quaternion.identity);
        PlayHitSound();
        
        yield return new WaitForSeconds(1f);
        Destroy(effect);
        
        void PlayHitSound()
        {
            int i = Random.Range(0, hitPlayerApr.Impacts.Length);
            hitPlayerApr.SoundSource.clip = hitPlayerApr.Impacts[i];
            hitPlayerApr.SoundSource.volume = GameData.s_instance.GameVolume;
            hitPlayerApr.SoundSource.Play();
        }
    }
    
}
