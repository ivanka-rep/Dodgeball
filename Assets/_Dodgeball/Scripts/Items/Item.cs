using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public abstract class Item : ScriptableObject
    {
        [Header("Common item properties")] 
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] private int m_price;
        [SerializeField] private Sprite m_sprite;
        
        public string ItemId => m_id;
        public string GetItemName() => m_name;
        public int GetItemPrice() => m_price;
        public Sprite GetItemSprite() => m_sprite;
        public abstract ItemType GetItemType();
    }

    public enum ItemType
    {
        Ball,
        Skin,
        Clothes,
        Accessories
    }
}