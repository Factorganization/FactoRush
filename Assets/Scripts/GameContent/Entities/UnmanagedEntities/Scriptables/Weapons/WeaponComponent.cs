using System.Collections.Generic;
using GameContent.Entities.UnmanagedEntities.Scriptables;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public abstract class WeaponComponent : UnitComponent
    {
        [Header("Graph")]
        public GameObject Graph;
        [Header("Weapon Stats")]
        public float Damage = 10f;        // Base damage of the weapon
        public float AttackSpeed = 1f;   // Time (seconds) between attacks
        public float Range = 5f;         // Attack range of the weapon
        public TargetType targetType;
        public bool isAPriorityTarget = false;

        
        // Play at Start
        public virtual void Initialize(Unit unit)
        {
            
        }

        // Main attack method
        public void Attack(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            HandleUniqueEffect(attacker, target, allUnitsInRange);
        }

        // Unique Effects Handlers
        protected abstract void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange);
        
    }
}
