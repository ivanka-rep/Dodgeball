using System;
using System.Collections.Generic;
using DataSakura.JoyStick.Scripts.Recorder;
using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.States
{
    public class JoyStickRecorder : JoyStickBase
    {
        public override void JoyStickInit(JoyStickSettings joyStickSettings)
        {
            base.JoyStickInit(joyStickSettings);
            
            if (null == m_joyStickSettings.Input)
            {
                m_joyStickSettings.Input = new JoyStickInput();
            }
            
            if (null == m_joyStickSettings.Input.FramesInputs)
            {
                m_joyStickSettings.Input.FramesInputs = new List<JoyStickInput.FrameInput>();
            }
            else if (m_joyStickSettings.Input.FramesInputs.Count > 0)
            {
                m_joyStickSettings.Input.FramesInputs.Clear();
            }
        }

        public override void JoyStickUpdate(float deltaTime)
        {
            base.JoyStickUpdate(deltaTime);
            
            m_frameInput = new JoyStickInput.FrameInput();
            
            m_frameInput.AnchoredPosition = m_joyStickSettings.Handle.anchoredPosition / m_joyStickSettings.Radius;
            
            m_frameInput.KeyCodesDown = new List<KeyCode>();
            m_frameInput.KeyCodes = new List<KeyCode>();
            m_frameInput.KeyCodesUp = new List<KeyCode>();
            
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    m_frameInput.KeyCodesDown.Add(kcode);
                } 
                else if (Input.GetKey(kcode))
                {
                    m_frameInput.KeyCodes.Add(kcode);
                } 
                else if (Input.GetKeyUp(kcode))
                {
                    m_frameInput.KeyCodesUp.Add(kcode);
                }
            }
            
            m_joyStickSettings.Input.FramesInputs.Add(m_frameInput);
        }
    }
}