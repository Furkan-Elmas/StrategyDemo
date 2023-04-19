using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public interface IDamageable
    {
        public int HealthPoint { get; }

        public List<PathNode> GetAvailablePointsAround();
        public void TakeDamage(int damage);
        public void Destroy();
    } 
}
