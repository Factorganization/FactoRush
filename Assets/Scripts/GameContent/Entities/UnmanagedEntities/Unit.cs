using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class Unit : Entity
    {
        #region Fields

        [SerializeField] private BodyComponent bodyComponent;           // Base component for health and stats
        [SerializeField] private WeaponComponent weaponComponent;       // Optional component for attack stats (damage, attack speed, range)
        [SerializeField] private TransportComponent transportComponent; // Optional component for movement stats (speed)

        private float currentHealth;
        private float passiveDamageTimer;
        private bool isIncomplete;

        #endregion

        #region Properties

        public bool IsAlive => currentHealth > 0;
        public bool CanAttack => weaponComponent != null;
        public bool CanMove => transportComponent != null;
        
        // Using Entity's Position for movement and targeting, making positioning updates simpler.
        
        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            InitializeUnit();
        }

        private void Update()
        {
            if (!IsAlive)
                return;

            HandlePassiveHealthDecay();
            HandleCombat();
            HandleMovement();
        }

        #endregion

        #region Initialization

        private void InitializeUnit()
        {
            // Initialize health based on body component, applying any transport-based multiplier
            currentHealth = bodyComponent.BaseHealth * (transportComponent?.HealthMultiplier ?? 1.0f);

            // Determine if the unit is incomplete (missing weapon or transport), for passive health decay
            isIncomplete = weaponComponent == null || transportComponent == null;

            if (isIncomplete)
                passiveDamageTimer = 1.0f; // Passive health decay applied every second
        }

        #endregion

        #region Health Management

        private void HandlePassiveHealthDecay()
        {
            if (!isIncomplete) return;

            passiveDamageTimer -= Time.deltaTime;
            if (passiveDamageTimer <= 0)
            {
                currentHealth -= bodyComponent.IncompleteHealthLossRate;
                passiveDamageTimer = 1.0f; // Reset for next decay tick

                if (currentHealth <= 0)
                    DestroyUnit();
            }
        }

        private void ApplyDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                DestroyUnit();
        }

        private void DestroyUnit()
        {
            Destroy(gameObject);
            //TODO Pooler ?? ou mieux anim
        }

        #endregion

        #region Movement

        private void HandleMovement()
        {
            if (!CanMove) return;

            // Calculate movement speed with transport multiplier
            float speed = bodyComponent.MovementSpeed * (transportComponent?.SpeedMultiplier ?? 1.0f);
            Position = Vector3.MoveTowards(Position, TargetPosition, speed * Time.deltaTime);
        }

        #endregion

        #region Combat

        private void HandleCombat()
        {
            if (!CanAttack) return;

            if (weaponComponent.CanAttack)
            {
                // Perform attack logic 
                Attack();
            }
        }

        private void Attack()
        {
           // TODO add logic like range, damage etc ta capté et delegate
        }

        #endregion
    }
}
