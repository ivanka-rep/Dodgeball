using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace DataSakura.JoyStick.Examples.Scripts.Examples
{
    public class TypeExample : MonoBehaviour
    {
        [SerializeField] private JoyStickSettings m_stickSettings;
        
        private Dropdown m_dropdownTypes;

        private void Awake()
        {
            m_dropdownTypes = GetComponent<Dropdown>();
            m_dropdownTypes.value = (int)m_stickSettings.Type;
        }

        public void OnTypeChanged()
        {
            switch (m_dropdownTypes.options[m_dropdownTypes.value].text)
            {
                case "Fixed":
                    m_stickSettings.Type = JoyStickSettings.JoyStickType.Fixed;
                    break;
                
                case "Floating":
                    m_stickSettings.Type = JoyStickSettings.JoyStickType.Floating;
                    break;
                
                case "Dynamic":
                    m_stickSettings.Type = JoyStickSettings.JoyStickType.Dynamic;
                    break;
            }
        }
    }
}