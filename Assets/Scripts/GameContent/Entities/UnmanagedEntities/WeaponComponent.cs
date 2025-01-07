using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public abstract class WeaponComponent : ScriptableObject
    {
        [Header("Weapon Stats")]
        public float Damage = 10f;        // Base damage of the weapon
        public float AttackSpeed = 1f;   // Time (seconds) between attacks
        public float Range = 5f;         // Attack range of the weapon

        protected float attackCooldown;

        public TargetType targetType;

        
        [Header("Effect Parameters")]
        public float MinigunSpeedIncrease = 0.05f; // % speed increase per attack
        public float LanceDamageBonus = 0.1f;     // % of movement speed added to damage
        public float ShieldValue = 50f;           // Initial shield value
        public float ShieldAllyRange = 5f;        // Range for shielding allies
        public float RayCannonRange = 5f;         // Range behind the target for ray effect
        public float WarHammerStunDuration = 3f;  // Stun duration for target
        public float WarHammerStunRadius = 2f;    // Stun radius for units behind target
        public float AoEArtilleryRadius = 3f;     // Radius of AoE damage for artillery

        // Can the weapon attack right now?
        public bool CanAttack => attackCooldown <= 0;

        // Main attack method
        public abstract void Attack(Unit attacker, Unit target, List<Unit> allUnitsInRange);

        public void UpdateCooldown()
        {
            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;
        }

        #region Unique Effects Handlers

        protected abstract void HandleUniqueEffectExemple(Unit target);
        

        private void HandleArtilleryEffect(Unit target, List<Unit> allUnitsInRange)
        {
            foreach (var unit in allUnitsInRange)
            {
                if (unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= AoEArtilleryRadius)
                {
                    unit.ApplyDamage(Damage);
                }
            }
        }

        private void HandleMinigunEffect()
        {
            AttackSpeed = Mathf.Max(0.1f, AttackSpeed * (1 - MinigunSpeedIncrease)); // Increase attack speed
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

        private void HandleShieldEffect(Unit attacker, List<Unit> allUnitsInRange)
        {
            attacker.AddShield(ShieldValue); // Add shield to attacker
            foreach (var ally in allUnitsInRange)
            {
                if (Vector3.Distance(attacker.transform.position, ally.transform.position) <= ShieldAllyRange && ally.isGroundUnit)
                {
                    ally.AddShield(ShieldValue / 2); // Shield allies
                }
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
