using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    // WeaponComponent holds stats related to the unit's weapon.
    [CreateAssetMenu(fileName = "NewWeaponComponent", menuName = "Components/WeaponComponent")]
    public class WeaponComponent : ScriptableObject
    {
        [Header("Weapon Stats")]
        public float Damage = 10f; 
        public float AttackCooldown = 1f; // Time between attacks.
        public float Range = 10f; 
        public bool CanAttackAirUnits = false; // Whether the weapon can target air units.
        
        private float attackCooldownTimer = 0f;
        public bool CanAttack => attackCooldownTimer <= 0f;

        // Check if the unit can attack now (based on cooldown).

        // Handle the cooldown between attacks.
        public void UpdateCooldown()
        {
            if (attackCooldownTimer > 0f)
                attackCooldownTimer -= Time.deltaTime;
        }

        // Reset the attack timer after an attack.
        public void ResetCooldown()
        {
            attackCooldownTimer = AttackCooldown;
        }
    }
}
