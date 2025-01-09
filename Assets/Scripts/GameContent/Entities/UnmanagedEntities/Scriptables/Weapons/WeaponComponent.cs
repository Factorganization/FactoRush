using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public abstract class WeaponComponent : ScriptableObject
    {
        [Header("Graph")]
        public GameObject Graph;
        [Header("Weapon Stats")]
        public float Damage = 10f;        // Base damage of the weapon
        public float AttackSpeed = 1f;   // Time (seconds) between attacks
        public float Range = 5f;         // Attack range of the weapon
        public TargetType targetType;
        
        

        
       
        public float LanceDamageBonus = 0.1f;     // % of movement speed added to damage
        public float ShieldValue = 50f;           // Initial shield value
        public float ShieldAllyRange = 5f;        // Range for shielding allies
        public float RayCannonRange = 5f;         // Range behind the target for ray effect
        public float WarHammerStunDuration = 3f;  // Stun duration for target
        public float WarHammerStunRadius = 2f;    // Stun radius for units behind target
        public float AoEArtilleryRadius = 3f;     // Radius of AoE damage for artillery
        

        // Main attack method
        public void Attack(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            HandleUniqueEffect(attacker, target, allUnitsInRange);
        }
        

        #region Unique Effects Handlers

        protected abstract void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange);
        

        private void HandleArtilleryEffect(Unit target, List<Unit> allUnitsInRange)
        {
        }

        private void HandleMinigunEffect()
        {
        }

        private void HandleSawBladeEffect(Unit target, List<Unit> allUnitsInRange)
        {
            foreach (var unit in allUnitsInRange)
            {
                if (Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                {
                    unit.ApplyDamage(Damage);
                }
            }
        }

        private void HandleC4Effect(Unit attacker)
        {
            attacker.ApplyDamage(attacker.currentHealth / 3); // Lose 1/3 of health
            // Damage nearby enemies
        }

        private void HandleLanceEffect(Unit attacker, Unit target)
        {
            float bonusDamage = attacker.transportComponent.SpeedMultiplier * LanceDamageBonus;
            target.ApplyDamage(Damage + bonusDamage);
        }

        private void HandleSymbioticRifleEffect(List<Unit> allUnitsInRange)
        {
            Unit lowestHPUnit = null;
            float lowestHP = float.MaxValue;

            foreach (var unit in allUnitsInRange)
            {
                if (unit.currentHealth < lowestHP)
                {
                    lowestHP = unit.currentHealth;
                    lowestHPUnit = unit;
                }
            }

            if (lowestHPUnit != null)
            {
                lowestHPUnit.ApplyDamage(Damage);
            }
        }
        

        private void HandleRayCannonEffect(Unit target, List<Unit> allUnitsInRange)
        {
            target.ApplyDamage(Damage);
            foreach (var unit in allUnitsInRange)
            {
                if (Vector3.Distance(target.transform.position, unit.transform.position) <= RayCannonRange)
                {
                    unit.ApplyDamage(Damage / 2); // Half damage for units behind
                }
            }
        }

        private void HandleWarHammerEffect(Unit target, List<Unit> allUnitsInRange)
        {
            target.Stun(WarHammerStunDuration);
            foreach (var unit in allUnitsInRange)
            {
                if (Vector3.Distance(target.transform.position, unit.transform.position) <= WarHammerStunRadius)
                {
                    unit.Stun(WarHammerStunDuration / 2); // Half stun for nearby units
                }
            }
        }

        #endregion
    }
}
