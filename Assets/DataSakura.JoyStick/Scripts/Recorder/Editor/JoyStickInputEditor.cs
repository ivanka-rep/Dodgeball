using UnityEditor;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.Recorder.Editor
{
    [CustomEditor(typeof(JoyStickInput))]
    public class JoyStickInputEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (JoyStickInput)target;
            
            if(GUILayout.Button("Import", GUILayout.Height(40)))
            {
                if (null != script.ImportTextAsset)
                {
                    script.Import();
                }
            }
            else if(GUILayout.Button("Export", GUILayout.Height(40)))
            {
                script.Export();
            }
        }
    }
}
