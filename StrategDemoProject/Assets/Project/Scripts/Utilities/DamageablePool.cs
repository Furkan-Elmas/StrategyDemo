using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    /// <summary>
    /// Damageable object pool static class.
    /// </summary>
    public static class DamageablePool
    {
        private static readonly Dictionary<Transform, IDamageable> damageables = new Dictionary<Transform, IDamageable>();


        public static void Subscribe(Transform transform, IDamageable damageable)
        {
            if (!damageables.ContainsKey(transform))
                damageables.Add(transform, damageable);
        }

        public static void Unsubscribe(Transform transform)
        {
            if (damageables.ContainsKey(transform))
                damageables.Remove(transform);
        }

        public static IDamageable GetSelectable(Transform transform)
        {
            if (damageables.ContainsKey(transform))
                return damageables[transform];

            return null;
        }
    }
}
