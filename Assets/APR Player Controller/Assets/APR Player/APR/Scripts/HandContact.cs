using System;
using System.Collections;
using UnityEngine;


    //-------------------------------------------------------------
    //--APR Player
    //--Hand Contact
    //
    //--Unity Asset Store - Version 1.0
    //
    //--By The Famous Mouse
    //
    //--Twitter @FamousMouse_Dev
    //--Youtube TheFamouseMouse
    //-------------------------------------------------------------


public class HandContact : MonoBehaviour
{
    public APRController APR_Player;
    //Is left or right hand
	public bool Left;
    
    //Have joint/grabbed
    public bool hasJoint;

    public bool hasBallJointRight
    {
        get => m_hasBallJointRight;
        set => m_hasBallJointRight = value;
    }

    public bool hasBallJointLeft
    {
        get => m_hasBallJointLeft;
        set => m_hasBallJointLeft = value;
    }

    private ThrowingController m_throwingController;
    private Bot m_bot;
    private bool m_hasBallJointRight, m_hasBallJointLeft;
    private void Start()
    {
        m_throwingController = APR_Player.ThrowingController;
        m_bot = this.GetComponentInParent<Bot>();
    }

    void Update()
    {
        if(APR_Player.useControls || APR_Player.isBot)
        {
            //Left Hand
            //On input release destroy joint
            if(Left)
            {
                if(hasJoint && !m_throwingController.reachLeft
                            && gameObject.GetComponent<FixedJoint>() != null)
                {
                    this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    hasJoint = false;
                    
                    if (m_hasBallJointLeft)
                    {
                        m_hasBallJointLeft = false;
                        m_throwingController.ballConnectedLeft = false;
                    }
                }

                if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                {
                    hasJoint = false;
                }
            }

            //Right Hand
            //On input release destroy joint
            if(!Left)
            {
                if(hasJoint && !m_throwingController.reachRight 
                            && gameObject.GetComponent<FixedJoint>() != null)
                {
                    this.gameObject.GetComponent<FixedJoint>().breakForce = 0;
                    hasJoint = false;

                    if (m_hasBallJointRight)
                    {
                        m_hasBallJointRight = false;
                        m_throwingController.ballConnectedRight = false;
                    }
                }

                if(hasJoint && this.gameObject.GetComponent<FixedJoint>() == null)
                {
                    hasJoint = false;
                }
            }
        }
    }

    //Grab on collision when input is used
    void OnCollisionEnter(Collision col)
    {
        string colLayer = LayerMask.LayerToName(col.gameObject.layer);
        string thisPlayerBallLayer = m_throwingController.ThisPlayerBallLayer;
        
        if (!(APR_Player.useControls || APR_Player.isBot) 
            || colLayer != thisPlayerBallLayer 
            || colLayer == thisPlayerBallLayer && m_throwingController.hasThrown )
            return;

        //Left Hand
        if(Left)
        {
            if(col.gameObject.CompareTag("CanBeGrabbed") && col.gameObject.layer != LayerMask.NameToLayer(APR_Player.thisPlayerLayer) && !hasJoint)
            {
                if( m_throwingController.reachLeft && !hasJoint && !APR_Player.punchingLeft)
                {
                    hasJoint = true;
                    this.gameObject.AddComponent<FixedJoint>();
                    this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                    this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();
                    
                   SetBallVars(col.gameObject, false);
                }
            }
        }

        //Right Hand
        if(!Left)
        {
            if(col.gameObject.CompareTag("CanBeGrabbed") && col.gameObject.layer != LayerMask.NameToLayer(APR_Player.thisPlayerLayer) && !hasJoint)
            {
                if( m_throwingController.reachRight && !hasJoint && !APR_Player.punchingRight)
                {
                    hasJoint = true;
                    this.gameObject.AddComponent<FixedJoint>();
                    this.gameObject.GetComponent<FixedJoint>().breakForce = Mathf.Infinity;
                    this.gameObject.GetComponent<FixedJoint>().connectedBody = col.gameObject.GetComponent<Rigidbody>();

                    SetBallVars(col.gameObject, true);
                }
            }
        }
    }

    void SetBallVars(GameObject go, bool right)
    {
        if (go.layer == LayerMask.NameToLayer(m_throwingController.ThisPlayerBallLayer))
        {
            //Debug.Log("SetBallVars");
            m_throwingController.ball = go.GetComponent<Rigidbody>();

            if (right)
            {
                m_hasBallJointRight = true;
                m_throwingController.ballConnectedRight = true;
            }
            else
            {
                m_hasBallJointLeft = true;
                m_throwingController.ballConnectedLeft = true;
            }
        }
    }
    
}
