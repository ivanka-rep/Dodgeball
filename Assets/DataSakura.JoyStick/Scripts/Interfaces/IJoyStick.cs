using DataSakura.JoyStick.Scripts.Settings;

namespace DataSakura.JoyStick.Scripts.Interfaces
{
    public interface IJoyStick
    {
        float Horizontal  { get; }

        float Vertical  { get; }

        JoyStickSettings Settings { get; set; }
        
        void ChangeState(IJoyStickState nextState);
    }
}