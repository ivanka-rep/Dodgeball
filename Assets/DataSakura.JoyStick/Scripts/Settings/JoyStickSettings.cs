using DataSakura.JoyStick.Scripts.Recorder;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.Settings
{
    [CreateAssetMenu(fileName = "JoyStick Settings", menuName = "JoyStick Settings", order = 54)]
    public class JoyStickSettings : ScriptableObject
    {
        [SerializeField] private JoyStickInput m_input;
        
        [SerializeField] private float m_radius;
        [SerializeField] private float m_speed;
        
        private RectTransform m_handle, m_background;
        private bool m_isTouched;
        
        private Vector3 m_startPos, m_dragPos; 

        public RectTransform Background
        {
            get => m_background;
            set => m_background = value;
        }
        
        public bool IsTouched
        {
            get => m_isTouched;
            set => m_isTouched = value;
        }

        public Vector3 StartPos
        {
            get => m_startPos;
            set => m_startPos = value;
        }

        public Vector3 DragPos
        {
            get => m_dragPos;
            set => m_dragPos = value;
        }
        
        public enum JoyStickMode
        {
            Base,
            Recorder,
            Player
        }

        [SerializeField] private JoyStickMode m_mode;

        public JoyStickMode Mode
        {
            get => m_mode;
            set => m_mode = value;
        }
        
        public enum JoyStickType
        {
            Fixed,
            Floating,
            Dynamic
        }

        [SerializeField] private JoyStickType m_type;

        public JoyStickType Type
        {
            get => m_type;
            set => m_type = value;
        }

        public JoyStickInput Input
        {
            get => m_input;
            set => m_input = value;
        }
        
        public float Radius
        {
            get => m_radius;
            set => m_radius = value;
        }
        
        public float Speed
        {
            get => m_speed;
            set => m_speed = value;
        }

        public RectTransform Handle
        {
            get => m_handle;
            set => m_handle = value;
        }
    }
}