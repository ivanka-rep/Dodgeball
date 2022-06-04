using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockOutAnimation : MonoBehaviour
{
    [SerializeField] private APRController m_aprController;
    [SerializeField] private GameObject m_starsEffect;

    public bool AnimationActive => m_animationActive;
    private bool m_animationActive;
        
    public void PlayAnim()
    {
        m_animationActive = true;
     
        ConfigurableJoint leftLeg = m_aprController.UpperLeftLeg.GetComponent<ConfigurableJoint>();
        ConfigurableJoint rightLeg = m_aprController.UpperRightLeg.GetComponent<ConfigurableJoint>();
        var botCtrl = gameObject.GetComponentInChildren<Bot>();
        var throwCtrl = gameObject.GetComponent<ThrowingController>();
        var initialPosY = m_aprController.Root.transform.position.y;
        Quaternion initialRightRot = rightLeg.targetRotation;
        Quaternion initialLeftRot = leftLeg.targetRotation;

        // Disabling controls
        if (gameObject.layer == LayerMask.NameToLayer("Player_1"))
        {
            m_aprController.useControls = false;
            m_aprController.Joystick.gameObject.SetActive(false);
        }
        else
        {
            m_aprController.isBot = false;
            botCtrl.StopAllCoroutines();
            botCtrl.goingToBall = false;
        }
        
        throwCtrl.DropTheBall();
        throwCtrl.LeftHandToTarget(); throwCtrl.RightHandToTarget();
        m_aprController.ResetPlayerPose(true);

        //Appear the animation
        RotateObject(leftLeg, 90f);
        RotateObject(rightLeg, 90f);
        m_starsEffect.SetActive(true);
        
        StartCoroutine(DelayCoroutine());
        IEnumerator DelayCoroutine()
        {
            yield return new WaitForSecondsRealtime(3f);
            
            leftLeg.targetRotation = initialLeftRot;
            rightLeg.targetRotation = initialRightRot;

            var pos = m_aprController.Root.transform.position;
            m_aprController.Root.transform.position = new Vector3(pos.x, initialPosY, pos.z);
            
            m_aprController.ResetPosition = true;
            m_aprController.isBalanced = true;
            m_starsEffect.SetActive(false);

            if (gameObject.layer == LayerMask.NameToLayer("Player_1"))
            {   m_aprController.useControls = true;
                m_aprController.Joystick.gameObject.SetActive(true); }

            yield return new WaitForSeconds(0.15f);
            
            if (gameObject.layer != LayerMask.NameToLayer("Player_1"))
            {   m_aprController.isBot = true;
                botCtrl.CanThrow = true;
                botCtrl.StartActivity();    }
            
            m_animationActive = false;
        }
    }
    
    private void RotateObject(ConfigurableJoint obj, float rotation)
    {
        Vector3 rot = obj.targetRotation.eulerAngles;
        obj.targetRotation = Quaternion.Euler(rot.x + rotation, rot.y, rot.z);
    }
}
