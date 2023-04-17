using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyDemoProject
{
    public class UnitButton : MonoBehaviour
    {
        private Button unitButton;

        private void Awake()
        {
            unitButton = GetComponent<Button>();
        }

        public void ChangeUnitProducer(ISelectable producer)
        {
            unitButton.onClick.RemoveAllListeners();

            unitButton.onClick.AddListener(() => GetUnit(producer));
        }

        private void GetUnit(ISelectable producer)
        {
            Barrack producerBarrack = producer as Barrack;

            producerBarrack.Produce();
        }
    }
}
