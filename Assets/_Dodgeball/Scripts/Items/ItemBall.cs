using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Ball", menuName = "Items/Ball", order = 1)]
    public class ItemBall : Item
    {
        [Header("Individual item properties")] 
        [SerializeField] private Material m_material;
        [Range(0f, 100f)] [SerializeField] private float m_speed;
        [Range(0f, 100f)] [SerializeField] private float m_distance;
        
        public float GetSpeed() => m_speed;
        public float GetDistance() => m_distance;
        public Material GetMaterial() => m_material;
        public override ItemType GetItemType() => ItemType.Ball;
    }
}