using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Items
{
    public class ShopObjectBall : ItemShopObject
    {
        [Header("Individual properties")]
        [SerializeField] private TextMeshProUGUI m_speedTMP;
        [SerializeField] private TextMeshProUGUI m_distanceTMP;

        public override void Init(Item item)
        {
            InitBaseProperties(item);
            InitIndividualProperties(item);
        }

        private void InitIndividualProperties(Item item)
        {
            ItemBall ball = (ItemBall) item;
            m_speedTMP.text = "Speed - " + ball.GetSpeed().ToString(CultureInfo.InvariantCulture);
            m_distanceTMP.text = "Distance - " + ball.GetDistance().ToString(CultureInfo.InvariantCulture);
        }
    }
}