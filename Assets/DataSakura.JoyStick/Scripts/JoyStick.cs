using System;
using DataSakura.JoyStick.Scripts.Interfaces;
using DataSakura.JoyStick.Scripts.Settings;
using DataSakura.JoyStick.Scripts.States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DataSakura.JoyStick.Scripts
{
    public class JoyStick : MonoBehaviour, IJoyStick, 
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Range(-1.0f, 1.0f)]
        [SerializeField] private float m_horizontal, m_vertical;

        [SerializeField] private JoyStickSettings m_joyStickSettings;
        [SerializeField] private RectTransform m_joyStickHandler, m_joyStickBackground;
        
        private IJoyStickState m_joyStickState;
        
        private JoyStickBase m_joyStickBase;
        private JoyStickRecorder m_joyStickRecorder;
        private JoyStickPlayer m_joyStickPlayer;

        private JoyStickSettings.JoyStickMode m_prevJoyStickMode;
        private JoyStickSettings.JoyStickType m_prevJoyStickType;

        private UnityEvent m_onPointerDownCustomAction;
        public UnityEvent ONPointerDownCustomAction
        {
            get => m_onPointerDownCustomAction;
            set => m_onPointerDownCustomAction = value;
        }

        public float Horizontal
        {
            get => m_horizontal;
        }

        public float Vertical
        {
            get => m_vertical;
        }

        public JoyStickSettings Settings
        {
            get => m_joyStickSettings;
            set
            {
                m_joyStickSettings = value;
                
                ChangeState();
            }
        }
        
        public void ChangeState(IJoyStickState nextState)
        {
            m_joyStickState = nextState;
            m_joyStickState.JoyStickInit(m_joyStickSettings);
        }

        public void ChangeState()
        {
            m_prevJoyStickMode = m_joyStickSettings.Mode;
            
            switch (m_joyStickSettings.Mode)
            {
                case JoyStickSettings.JoyStickMode.Base:
                    ChangeState(m_joyStickBase);
                    break;
                
                case JoyStickSettings.JoyStickMode.Recorder:
                    ChangeState(m_joyStickRecorder);
                    break;
                    
                case JoyStickSettings.JoyStickMode.Player:
                    ChangeState(m_joyStickPlayer);
                    break;
            }
        }

        public void ChangeType()
        {
            m_prevJoyStickType = m_joyStickSettings.Type;
            
            if (m_joyStickSettings.Type != JoyStickSettings.JoyStickType.Fixed)
            {
                m_joyStickHandler.GetComponent<Image>().enabled = false;
                m_joyStickBackground.GetComponent<Image>().enabled = false;
            }
            else
            {
                m_joyStickHandler.GetComponent<Image>().enabled = true;
                m_joyStickBackground.GetComponent<Image>().enabled = true;
            }
        }

        private void Awake()
        {
            m_joyStickSettings.Handle = m_joyStickHandler;
            m_joyStickSettings.Background = m_joyStickBackground;
            
            m_joyStickBase = new JoyStickBase();
            m_joyStickRecorder = new JoyStickRecorder();
            m_joyStickPlayer = new JoyStickPlayer();
            
            m_joyStickSettings.StartPos = m_joyStickSettings.DragPos = Vector3.zero;
            m_onPointerDownCustomAction = new UnityEvent();
        }

        private void Start()
        {
            ChangeState();
            ChangeType();
        }

        private void Update()
        {
            CheckChangeState();
            CheckChangeType();
            
            m_joyStickState.JoyStickUpdate(Time.deltaTime);
            m_joyStickState.JoyStickUpdateDirection();

            m_horizontal = m_joyStickState.Direction.x;
            m_vertical = m_joyStickState.Direction.y;
        }

        private void CheckChangeState()
        {
            if (m_prevJoyStickMode != m_joyStickSettings.Mode)
            {
                ChangeState();
            }
        }
        
        private void CheckChangeType()
        {
            if (m_prevJoyStickType != m_joyStickSettings.Type)
            {
                ChangeType();
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_joyStickSettings.IsTouched = true;
            m_joyStickSettings.DragPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_joyStickSettings.IsTouched = true;
            m_joyStickSettings.DragPos = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_joyStickSettings.IsTouched = false;
            m_joyStickSettings.DragPos = eventData.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_joyStickSettings.Type != JoyStickSettings.JoyStickType.Fixed)
            {
                m_joyStickHandler.GetComponent<Image>().enabled = true;
                m_joyStickBackground.GetComponent<Image>().enabled = true;
            }
            
            m_joyStickSettings.IsTouched = true;
            m_joyStickSettings.StartPos = m_joyStickSettings.DragPos = eventData.position;

            m_onPointerDownCustomAction?.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_joyStickSettings.Type != JoyStickSettings.JoyStickType.Fixed)
            {
                m_joyStickHandler.GetComponent<Image>().enabled = false;
                m_joyStickBackground.GetComponent<Image>().enabled = false;
            }
            
            m_joyStickSettings.IsTouched = false;
            m_joyStickSettings.DragPos = eventData.position;
        }
    }
}