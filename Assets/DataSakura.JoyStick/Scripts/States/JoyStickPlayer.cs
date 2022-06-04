using System.Collections.Generic;
using DataSakura.JoyStick.Scripts.Recorder;
using DataSakura.JoyStick.Scripts.Settings;

namespace DataSakura.JoyStick.Scripts.States
{
    public class JoyStickPlayer : JoyStickBase
    {
        private int m_step;
        
        public override void JoyStickInit(JoyStickSettings joyStickSettings)
        {
            base.JoyStickInit(joyStickSettings);

            m_step = 0;
            
            if (null == m_joyStickSettings.Input)
            {
                m_joyStickSettings.Input = new JoyStickInput();
            }
            
            if (null == m_joyStickSettings.Input.FramesInputs)
            {
                m_joyStickSettings.Input.FramesInputs = new List<JoyStickInput.FrameInput>();
            }
        }

        public override void JoyStickUpdate(float deltaTime)
        {
            m_frameInput = m_joyStickSettings.Input.GetFrameInput(m_step++);

            if (null != m_frameInput)
            {
                m_joyStickSettings.Handle.anchoredPosition = m_frameInput.AnchoredPosition * m_joyStickSettings.Radius;
            }
        }
    }
}