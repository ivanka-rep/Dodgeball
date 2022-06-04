using System;
using System.Collections.Generic;
using System.IO;
using DataSakura.JoyStick.Scripts.Interfaces;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.Recorder
{
    [CreateAssetMenu(fileName = "JoyStick Input", menuName = "JoyStick Input", order = 53)]
    public class JoyStickInput : ScriptableObject
    {
        [SerializeField] private List<FrameInput> m_framesInputs;
        [SerializeField] private TextAsset m_importTextAsset;
        [SerializeField] private string m_exportPath = "DataSakura.JoyStick/Records";

        public TextAsset ImportTextAsset
        {
            get => m_importTextAsset;
            set => m_importTextAsset = value;
        }

        private string m_str;

        private FrameInput m_frameInput;

        private float m_x, m_y;
        
        private List<KeyCode> m_keyCodes;

        public List<FrameInput> FramesInputs
        {
            get => m_framesInputs;
            set => m_framesInputs = value;
        }

        [Serializable]
        public class FrameInput : IJoyStickFrameInput
        {
            [SerializeField] private Vector2 m_anchoredPosition;
            [SerializeField] private List<KeyCode> m_keyCodesDown, m_keyCodes, m_keyCodesUp;

            public Vector2 AnchoredPosition
            {
                get => m_anchoredPosition;
                set => m_anchoredPosition = value;
            }
            
            public List<KeyCode> KeyCodesDown
            {
                get => m_keyCodesDown;
                set => m_keyCodesDown = value;
            }
            
            public List<KeyCode> KeyCodes
            {
                get => m_keyCodes;
                set => m_keyCodes = value;
            }
            
            public List<KeyCode> KeyCodesUp
            {
                get => m_keyCodesUp;
                set => m_keyCodesUp = value;
            }

            public bool GetKeyDown(KeyCode keyCode)
            {
                return FindKeyInList(keyCode, m_keyCodesDown);
            }
            
            public bool GetKey(KeyCode keyCode)
            {
                return FindKeyInList(keyCode, m_keyCodes);
            }
            
            public bool GetKeyUp(KeyCode keyCode)
            {
                return FindKeyInList(keyCode, m_keyCodesUp);
            }
            
            private bool FindKeyInList(KeyCode keyCode, List<KeyCode> list)
            {
                foreach (KeyCode key in list)
                {
                    if (key == keyCode)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        public FrameInput GetFrameInput(int frameIndex)
        {
            if (frameIndex >= 0 && frameIndex < m_framesInputs.Count)
            {
                return m_framesInputs[frameIndex];
            }

            return null;
        }
        
        public void Import()
        {
            if (null == m_framesInputs)
            {
                m_framesInputs = new List<FrameInput>();
            }
            else
            {
                m_framesInputs.Clear();
            }

            string[] lines = m_importTextAsset.text.Split('\n');
            foreach (string line in lines)
            {
                string[] values = line.Split(';');
                
                m_frameInput = new FrameInput();

                if (float.TryParse(values[0], out m_x) && float.TryParse(values[1], out m_y))
                {
                    m_frameInput.AnchoredPosition = new Vector2(m_x, m_y);
                }
                else
                {
                    m_frameInput.AnchoredPosition = Vector2.zero;
                }

                if (values.Length > 2)
                {
                    m_frameInput.KeyCodesDown = ImportKeyCodes(values[2]);
                }
                
                if (values.Length > 3)
                {
                    m_frameInput.KeyCodes = ImportKeyCodes(values[3]);
                }
                
                if (values.Length > 4)
                {
                    m_frameInput.KeyCodesUp = ImportKeyCodes(values[4]);
                }
            
                m_framesInputs.Add(m_frameInput);
            }
        }

        private List<KeyCode> ImportKeyCodes(string str)
        {
            m_keyCodes = new List<KeyCode>();
            
            if (string.IsNullOrEmpty(str))
            {
                return m_keyCodes;
            }
            
            string[] keyCodesStrings = str.Split(':');
            foreach (string key in keyCodesStrings)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                
                m_keyCodes.Add((KeyCode) int.Parse(key) );
            }

            return m_keyCodes;
        }

        public void Export()
        {
            if (null == m_framesInputs || 0 == m_framesInputs.Count)
            {
                return;
            }
            
            m_str = String.Empty;
            
            for (int i = 0; i < m_framesInputs.Count; i++)
            {
                m_str += m_framesInputs[i].AnchoredPosition.x + ";";
                m_str += m_framesInputs[i].AnchoredPosition.y + ";";
                
                m_str += ExportKeyCodes(m_framesInputs[i].KeyCodesDown) + ";";
                m_str += ExportKeyCodes(m_framesInputs[i].KeyCodes) + ";";
                m_str += ExportKeyCodes(m_framesInputs[i].KeyCodesUp) + (i < m_framesInputs.Count - 1 ? "\n" : "");
            }
            
            using 
            (
                StreamWriter outputFile = new StreamWriter
                (
                    Path.Combine(Application.dataPath + "/" + m_exportPath + "/", name + ".txt")
                )
            )
            {
                outputFile.Write(m_str);
            }
        }
        
        private string ExportKeyCodes(List<KeyCode> keyCodes)
        {
            if (null == keyCodes || 0 == keyCodes.Count)
            {
                return "";
            }
            
            string str = (int)keyCodes[0] + (keyCodes.Count > 2 ? ":" : "");

            for (int i = 1; i < keyCodes.Count; i++)
            {
                str += ":" + (int)keyCodes[i];
            }

            return str;
        }
    }
}