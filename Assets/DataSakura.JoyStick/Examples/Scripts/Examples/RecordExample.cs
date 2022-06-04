using System.Collections.Generic;
using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;

namespace DataSakura.JoyStick.Examples.Scripts.Examples
{
    public class RecordExample : MonoBehaviour
    {
        [SerializeField] private GameObject m_record;
        [SerializeField] private List<JoyStick.Scripts.JoyStick> m_joySticks;

        private bool m_flag;

        private void Update()
        {
            m_flag = false;
            foreach (JoyStick.Scripts.JoyStick joyStick in m_joySticks)
            {
                if (joyStick.Settings.Mode == JoyStickSettings.JoyStickMode.Recorder)
                {
                    m_flag = true;
                    break;
                }
            }
            
            m_record.SetActive(m_flag);
        }
    }
}