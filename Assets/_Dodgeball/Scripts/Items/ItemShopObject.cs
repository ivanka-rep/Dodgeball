using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Items
{
    public abstract class ItemShopObject : MonoBehaviour, IPointerClickHandler
    {
        [Header("Base properties")]
        [SerializeField] private TextMeshProUGUI m_nameTMP;
        [SerializeField] private TextMeshProUGUI m_priceTMP;
        [SerializeField] private Image m_iconImage;
        [SerializeField] private GameObject m_activeIndicator;
        private bool m_isPurchased;
        private string m_itemId;
        
        public bool IsPurchased { get => m_isPurchased; set => m_isPurchased = value; }
        public string ItemId { get => m_itemId; set => m_itemId = value; }
        public UnityEvent onClick;
        public GameObject PriceGO => m_priceTMP.gameObject;
        
        public abstract void Init(Item item);

        protected void InitBaseProperties(Item item)
        {
            onClick = new UnityEvent();
            m_itemId = item.ItemId;
            m_nameTMP.text = item.GetItemName();
            m_priceTMP.text = "$" + item.GetItemPrice();
            m_iconImage.sprite = item.GetItemSprite();
        }

        public void Activate(bool value) { m_activeIndicator.SetActive(value); }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
            //TODO: add click animation.
        }
    }
}