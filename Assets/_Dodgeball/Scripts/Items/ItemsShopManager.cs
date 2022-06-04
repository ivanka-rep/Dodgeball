using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Items
{
    public class ItemsShopManager : MonoBehaviour
    {
        [SerializeField] private RectTransform m_ballsContent;
        [SerializeField] private RectTransform m_skinsContent;
        [SerializeField] private GameObject m_shopObjectBallPrefab;
        [SerializeField] private GameObject m_shopObjectSkinPrefab;
        [SerializeField] private TextMeshProUGUI m_totalCashTMP;
        [SerializeField] private Button m_freeCashButton;
        [SerializeField] private Button m_closeButton;
        
        private GameData m_gameData;
        private GameManager m_gameManager;
        private  List<ItemShopObject> m_shopObjects;
        
        public void Awake()
        {
            m_gameData = GameData.s_instance;
            m_gameManager = GameManager.s_instance;
            m_shopObjects = new List<ItemShopObject>();
            
            
            m_closeButton.onClick.AddListener(() =>
            {
                m_gameManager.PanelsMask.SetActive(false);
                TweenAnimation.PlayAnimation(transform, AnimationType.ClosingTransition, 0.5f, 
                    () => gameObject.SetActive(false));
            });
            m_freeCashButton.onClick.AddListener(() =>
            {
                m_gameData.UserData.Cash += 1000;
                UpdateCashView();
            });
        }

        public void Start()
        {
            m_gameData.Items.ForEach(item =>
            {
                GameObject itemGo;
                ItemShopObject shopObj = null;
                
                if (item.ItemId.Contains("ball"))
                {
                    itemGo = Instantiate(m_shopObjectBallPrefab, m_ballsContent);
                    shopObj = itemGo.GetComponent<ShopObjectBall>();
                }
                else if (item.ItemId.Contains("skin"))
                {
                    itemGo = Instantiate(m_shopObjectSkinPrefab, m_skinsContent);
                    shopObj = itemGo.GetComponent<ShopObjectSkin>();
                }

                if (shopObj == null) return;
                
                shopObj.Init(item);
                shopObj.onClick.AddListener(() => ChooseItem(item, shopObj));
                m_shopObjects.Add(shopObj);
            });
            
            m_skinsContent.parent.gameObject.SetActive(false);
            m_ballsContent.parent.gameObject.SetActive(true);
            UpdateView();
        }

        public void OnEnable()
        {
            UpdateCashView();
        }

        private void UpdateView()
        {
            UpdateCashView();
            
            m_shopObjects.ForEach(shopObj =>
            {
                string itemId = shopObj.ItemId;
                if (itemId == m_gameData.UserData.CurrentBallId || itemId == m_gameData.UserData.CurrentSkinId)
                { shopObj.Activate(true); }

                if (m_gameData.PlayerItems.Contains(itemId))
                {
                    shopObj.PriceGO.SetActive(false);
                    shopObj.IsPurchased = true;
                }
            });
        }

        private void UpdateCashView() => m_totalCashTMP.text = "Total: $" + m_gameData.UserData.Cash;

        private void ChooseItem(Item item, ItemShopObject itemShopObject)
        {
            bool itemAvailable = itemShopObject.IsPurchased;
            bool itemPurchasedNow = false;
            if (itemAvailable == false) 
            { itemAvailable = PurchaseItem(item, itemShopObject); itemPurchasedNow = itemAvailable; }

            if (!itemAvailable) return;
            
            
            switch (item.GetItemType())
            {
                case ItemType.Ball:
                    DisableAllShopItemsByType("ball");
                    m_gameData.UserData.CurrentBallId = item.ItemId; break;
                case ItemType.Skin:
                    DisableAllShopItemsByType("skin");
                    m_gameData.UserData.CurrentSkinId = item.ItemId;
                    GameManager.s_instance.ChangePlayerSkin(); break;
                    
                //TODO: Add cases for clothes, accessories, etc.
            }
            
            SetNewItem(item, itemPurchasedNow);
            UpdateView();

            void DisableAllShopItemsByType(string id)
            {
                m_shopObjects.FindAll(shopItem => shopItem.ItemId.Contains(id))
                    .ForEach(shopItem => shopItem.Activate(false));
            }
        }

        private bool PurchaseItem(Item item, ItemShopObject itemShopObject)
        {
            if (m_gameData.UserData.Cash >= item.GetItemPrice() )
            {
                m_gameData.UserData.Cash -= item.GetItemPrice();
                return true;
            }
            
            //Animation when player doesn't have enough cash.
            TextMeshProUGUI priceTMP = itemShopObject.PriceGO.GetComponent<TextMeshProUGUI>();
            priceTMP.color = Color.red;
            ColorUtility.TryParseHtmlString("#26FF00FF", out Color oldColor);
            TweenAnimation.PlayAnimation(priceTMP.transform, AnimationType.Shake, 1f, 
                () => priceTMP.color = oldColor);
            return false;
        }
        
        private void SetNewItem(Item item, bool purchased)
        {
            string itemId = item.ItemId;
            if (purchased)
            {
                List<string> itemsList = m_gameData.PlayerItems.ToList();
                itemsList.Add(itemId);
                m_gameData.PlayerItems = itemsList.ToArray();
            }
            m_gameData.SaveUserData();
        }

    }
}