using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    [CreateAssetMenu(fileName = "NewWeaponComponent", menuName = "Components/WeaponComponent")]
    public class WeaponComponent : ScriptableObject
    {
        [Header("Weapon Stats")]
        public float Damage = 10f;        // Base damage of the weapon
        public float AttackSpeed = 1f;   // Time (seconds) between attacks
        public float Range = 5f;         // Attack range of the weapon

        private float attackCooldown;

        // Can the weapon attack right now?
        public bool CanAttack => attackCooldown <= 0;

        public void Attack()
        {
            if (!CanAttack) return;

            attackCooldown = AttackSpeed;
            // Add logic for damage application, targeting, etc.
            Debug.Log("Weapon attacked with damage: " + Damage);
        }

        public void UpdateCooldown()
        {
            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;
        }
    }
}