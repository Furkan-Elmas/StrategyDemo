using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyDemoProject
{
    public interface ISelectable
    {
        public Transform Transform { get; }
        public PlaceableDataSO PlaceableDataSO { get; }

        public void SetVisualColor(Color color);
        public void OnSelect();
        public void OnDeselect();
    }
}
