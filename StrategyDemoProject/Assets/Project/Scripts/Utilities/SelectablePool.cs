using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    /// <summary>
    /// Selectable object pool static class.
    /// </summary>
    public static class SelectablePool
    {
        private static readonly Dictionary<Transform, ISelectable> selectables = new Dictionary<Transform, ISelectable>();


        public static void Subscribe(Transform transform, ISelectable selectable)
        {
            if (!selectables.ContainsKey(transform))
                selectables.Add(transform, selectable);
        }

        public static void Unsubscribe(Transform transform)
        {
            if (selectables.ContainsKey(transform))
                selectables.Remove(transform);
        }

        public static ISelectable GetSelectable(Transform transform)
        {
            if (selectables.ContainsKey(transform))
                return selectables[transform];

            return null;
        }
    } 
}
