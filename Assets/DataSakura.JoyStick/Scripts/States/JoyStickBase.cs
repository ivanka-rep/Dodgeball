using DataSakura.JoyStick.Scripts.Interfaces;
using DataSakura.JoyStick.Scripts.Recorder;
using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.States
{
    public class JoyStickBase : IJoyStickState
    {
        protected JoyStickSettings m_joyStickSettings;
        
        protected JoyStickInput.FrameInput m_frameInput;
        
        private Vector2 m_direction;

        public Vector2 Direction
        {
            get => m_direction;
        }
        
        private Vector3 m_pos;
        private float m_angle;

        public virtual void JoyStickInit(JoyStickSettings joyStickSettings)
        {
            m_joyStickSettings = joyStickSettings;
        }

        public virtual void JoyStickUpdate(float deltaTime)
        {
            if (m_joyStickSettings.Type == JoyStickSettings.JoyStickType.Floating)
            {
                m_joyStickSettings.Background.position = m_joyStickSettings.StartPos;
            }
            
            if (false == m_joyStickSettings.IsTouched)
            {
                m_joyStickSettings.Handle.anchoredPosition = Vector3.Lerp(
                    m_joyStickSettings.Handle.anchoredPosition, Vector3.zero, Time.fixedDeltaTime * m_joyStickSettings.Speed
                );
            }
            else
            {
                m_joyStickSettings.Handle.transform.position = Vector3.Lerp(
                    m_joyStickSettings.Handle.transform.position, m_joyStickSettings.DragPos, Time.fixedDeltaTime * m_joyStickSettings.Speed
                );

                if (
                    m_joyStickSettings.Handle.anchoredPosition.x * m_joyStickSettings.Handle.anchoredPosition.x + 
                    m_joyStickSettings.Handle.anchoredPosition.y * m_joyStickSettings.Handle.anchoredPosition.y > Mathf.Pow(m_joyStickSettings.Radius, 2)
                )
                {
                    if (m_joyStickSettings.Type == JoyStickSettings.JoyStickType.Dynamic)
                    {
                        m_joyStickSettings.Background.position = Vector3.Lerp(
                            m_joyStickSettings.Background.position,
                            m_joyStickSettings.DragPos - new Vector3(
                                m_joyStickSettings.Handle.anchoredPosition.x,
                                m_joyStickSettings.Handle.anchoredPosition.y
                            ),
                            deltaTime * m_joyStickSettings.Speed
                        );
                    }
                    
                    m_angle = Vector2.Angle
                    (
                        new Vector2(m_joyStickSettings.Radius, 0),
                        new Vector2(m_joyStickSettings.Handle.anchoredPosition.x, m_joyStickSettings.Handle.anchoredPosition.y)
                    );

                    if (m_joyStickSettings.Handle.anchoredPosition.y < 0)
                    {
                        m_angle = 360 - m_angle;
                    }
                
                    m_angle = m_angle * (Mathf.PI / 180f);
                
                    m_pos = new Vector3(Mathf.Cos(m_angle) * m_joyStickSettings.Radius, Mathf.Sin(m_angle) * m_joyStickSettings.Radius);

                    m_joyStickSettings.Handle.anchoredPosition = m_pos;
                }
            }
        }

        public void JoyStickUpdateDirection()
        {
            m_direction = m_joyStickSettings.Handle.anchoredPosition / m_joyStickSettings.Radius;
        }
    }
}