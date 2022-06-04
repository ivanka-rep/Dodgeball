using System.Collections.Generic;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.Interfaces
{
    public interface IJoyStickFrameInput
    {
        Vector2 AnchoredPosition { get; set; }
            
        List<KeyCode> KeyCodesDown { get; set; }
            
        List<KeyCode> KeyCodes { get; set; }
            
        List<KeyCode> KeyCodesUp { get; set; }

        bool GetKeyDown(KeyCode keyCode);

        bool GetKey(KeyCode keyCode);

        bool GetKeyUp(KeyCode keyCode);
    }
}