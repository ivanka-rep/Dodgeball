using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Items
{
    public class ShopObjectSkin : ItemShopObject
    {
        // [Header("Individual properties")]
        
        public override void Init(Item item)
        {
            InitBaseProperties(item);
            // InitIndividualProperties(item);
        }

        private void InitIndividualProperties(Item item)
        {
            
        }
    }
}