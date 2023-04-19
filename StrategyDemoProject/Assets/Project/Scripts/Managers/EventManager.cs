using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StrategyDemoProject
{
    public static class EventManager
    {
        #region Actions
        public static Action<IPlaceable> OnPlaceableObjectPicked;
        public static Action<ISelectable, bool> OnSelectableObjectPicked;
        public static Action OnPlaceableObjectUnpicked;
        public static Action<IPlaceable> OnPlaceableObjectDestroyed;
        public static Action<ISelectable> OnSelectableObjectDestroyed;
        #endregion
    }
}
