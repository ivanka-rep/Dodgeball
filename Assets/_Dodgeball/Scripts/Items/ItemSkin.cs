using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Items/Skin", order = 1)]
    public class ItemSkin : Item
    {
        [Header("Individual item properties")] 
        [SerializeField] private Material m_material;
        public Material GetMaterial() => m_material;
        public override ItemType GetItemType() => ItemType.Skin;
    }
}