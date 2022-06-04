using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PressDetection : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private ThrowingController m_playerThrowCtrl;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        m_playerThrowCtrl.OnPointerDown();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_playerThrowCtrl.OnDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_playerThrowCtrl.OnPointerUp();
    }
}