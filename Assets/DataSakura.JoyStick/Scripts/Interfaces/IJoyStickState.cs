using DataSakura.JoyStick.Scripts.Settings;
using UnityEngine;

namespace DataSakura.JoyStick.Scripts.Interfaces
{
    public interface IJoyStickState
    {
        Vector2 Direction { get; }
        
        void JoyStickInit(JoyStickSettings joyStickSettings);

        void JoyStickUpdate(float deltaTime);

        void JoyStickUpdateDirection();
    }
}