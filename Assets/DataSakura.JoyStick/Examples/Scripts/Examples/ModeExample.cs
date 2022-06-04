using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace DataSakura.JoyStick.Examples.Scripts.Examples
{
    public class ModeExample : MonoBehaviour
    {
        [SerializeField] private JoyStickSettings m_stickSettings;
        
        private Dropdown m_dropdownModes;

        private void Awake()
        {
            m_dropdownModes = GetComponent<Dropdown>();
            m_dropdownModes.value = (int)m_stickSettings.Mode;
        }

        public void OnModeChanged()
        {
            switch (m_dropdownModes.options[m_dropdownModes.value].text)
            {
                case "Base":
                    m_stickSettings.Mode = JoyStickSettings.JoyStickMode.Base;
                    break;
                
                case "Recorder":
                    m_stickSettings.Mode = JoyStickSettings.JoyStickMode.Recorder;
                    break;
                
                case "Player":
                    m_stickSettings.Mode = JoyStickSettings.JoyStickMode.Player;
                    break;
            }
        }
    }
}