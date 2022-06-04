using UnityEngine;

namespace DataSakura.JoyStick.Examples.Scripts.Examples
{
    public class JoyStickExample : MonoBehaviour
    {
        [SerializeField] private JoyStick.Scripts.JoyStick m_joyStick;
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private float m_force;

        private Vector3 m_forceDirection;

        private void FixedUpdate()
        {
            m_forceDirection = Camera.main.transform.forward * m_joyStick.Vertical + Camera.main.transform.right * m_joyStick.Horizontal;
            m_rigidbody.AddForce(m_forceDirection.normalized * m_force);
        }
    }
}