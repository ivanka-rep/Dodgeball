using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    [SerializeField] private GameObject m_fireParticle;
    
    private bool m_hasHit = false;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Ball_Player_1") || m_hasHit ) return;
        m_hasHit = true;

        TutorialManager.s_instance.AddTutorialScore();
        m_fireParticle.SetActive(true);
    }
}
