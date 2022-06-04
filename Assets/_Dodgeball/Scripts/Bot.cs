using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Configs.GameConfig;

public class Bot : MonoBehaviour
{
    //public bool isBot = false;
    [HideInInspector] public float forwardBackward;
    [HideInInspector] public float leftRight = 0f;
    [HideInInspector] public Transform lookAtTransform;
    [HideInInspector] public bool isNeedAim;
    [HideInInspector] public bool goingToBall = false;
    public float MoveSpeed => m_difficultyParams.botMoveSpeed;
    
    public bool CanThrow = false;
    
    private bool m_startedThrowingProcces = false;
    private bool m_evading = false;
    private bool m_hasVarsSet;
    private List<GameObject> m_players;
    private APRController m_aprController;
    private ThrowingController m_throwingController;
    private DifficultyParams m_difficultyParams;
    
    public void StartActivity()
    {
        if (!CanThrow)
        {
            forwardBackward = m_difficultyParams.botMoveSpeed;
            StartCoroutine(BotRotation());
        }
        else BallThrowing();
    }

    public void SetGameVars()
    {
        m_aprController = gameObject.GetComponentInParent<APRController>();
        m_throwingController = gameObject.GetComponentInParent<ThrowingController>();
        m_difficultyParams = GameManager.s_instance.DifficultyParams;
        m_players = GameManager.s_instance.Players.FindAll(player =>
            player.layer != LayerMask.NameToLayer(m_aprController.thisPlayerLayer));
    }

    private void OnCollisionStay(Collision col)
    {
        if (m_aprController == null) return;
        
        if (m_evading || !m_aprController.isBalanced || m_startedThrowingProcces ||
            col.gameObject.layer == LayerMask.NameToLayer(m_aprController.thisPlayerLayer)) return;
        
        string colLayer = LayerMask.LayerToName(col.gameObject.layer);
        if (colLayer == "Obstacle" || colLayer == "LimitWall" || colLayer.Contains("Player")) 
            StartCoroutine(Evade());
    }

    private IEnumerator Evade()
    {
        //Debug.Log("EVADE");

        if (m_startedThrowingProcces) { yield break; }
        
        m_evading = true;
        forwardBackward = MoveSpeed;

        if (Random.Range(0f, 100f) > 50f)
            leftRight = 1f;
        else leftRight = -1f;

        yield return new WaitForSeconds(0.7f);
        m_evading = false;
        leftRight = 0f;
    }

    private IEnumerator BotRotation()
    {
        float rRot = Random.Range(-0.5f, 0.5f);
        if (!m_evading)
            leftRight = rRot;

        yield return new WaitForSeconds(1f);
        if (!m_evading)
            leftRight = 0f;

        yield return new WaitForSeconds(5f);

        StartCoroutine(BotRotation());
    }

    public void BallThrowing()
    {
        if (CanThrow && !goingToBall && (m_throwingController.reachLeft || m_throwingController.reachRight)
            && (m_throwingController.ballConnectedLeft || m_throwingController.ballConnectedRight) )
        {
            // - preparing
            if (m_throwingController.ballConnectedRight) m_throwingController.prepareRight = true;
            else m_throwingController.prepareLeft = true;
            
            GameObject player = m_players.Find(pl => pl.layer == LayerMask.NameToLayer("Player_1"));
            GameObject lookAtGO = player.GetComponent<APRController>().Head;
            
            StartCoroutine(Aiming(lookAtGO, true)); // AIM and shoot
        }
        else if (!goingToBall)
        {
            goingToBall = true;
            forwardBackward = MoveSpeed;
            
            StartCoroutine(Aiming(m_throwingController.CurrentBall));
        }

        IEnumerator Aiming(GameObject lookAt, bool throwBall = false)
        {
            lookAtTransform = lookAt.transform;
            isNeedAim = true;

            if (throwBall)
            {
                StopCoroutine(Evade());
                m_startedThrowingProcces = true;
                forwardBackward = 0f;
                yield return new WaitForSeconds(Random.Range(1f, 5f));
                
                isNeedAim = false;

                // - throwing
                if (m_throwingController.ballConnectedRight)  m_throwingController.throwRight = true;
                else m_throwingController.throwLeft = true;
                
                m_startedThrowingProcces = false;
                CanThrow = false;
                StartActivity();
            }
        }
    }
}
