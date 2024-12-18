using GameContent.Entities.OnFieldEntities.Buildings;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class Unit : Entity
    {
        #region Fields

        [Header("Unit Components")]
        [SerializeField] private WeaponComponent weaponComponent;       // Reference to weapon stats
        [SerializeField] private TransportComponent transportComponent; // Reference to transport stats

        private float currentHealth;
        private float passiveDamageTimer;
        
        // reference to the event on factorybuilding script
        private FactoryBuilding factoryBuilding;

        #endregion

        #region Properties

        public bool IsAlive => currentHealth > 0;
        public bool CanAttack => weaponComponent != null;
        public bool CanMove => transportComponent != null;

        #endregion

        #region Unity Lifecycle

        protected override void OnAwake()
        {
            base.OnAwake();
            //factoryBuilding.OnAllyFactoryDeath.AddListener(DestroyUnit);
        }

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
            // Calculate initial stats based on components
            float baseHealth = 100; // Default health if no transport
            float baseSpeed = 1;    // Default speed if no transport

            currentHealth = transportComponent != null
                ? baseHealth * transportComponent.HealthMultiplier
                : baseHealth;

            // If transportComponent is missing or weaponComponent is missing, enable passive health decay
            passiveDamageTimer = (transportComponent == null || weaponComponent == null) ? 1.0f : 0;
        }

        #endregion

        #region Health Management

        private void HandlePassiveHealthDecay()
        {
            if (passiveDamageTimer <= 0) return;

            passiveDamageTimer -= Time.deltaTime;
            if (passiveDamageTimer <= 0)
            {
                passiveDamageTimer = 1.0f; // Reset timer

                currentHealth -= 10; // Decay amount (adjust as needed)
                if (currentHealth <= 0)
                    DestroyUnit();
            }
        }

        public void ApplyDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                DestroyUnit();
        }

        private void DestroyUnit()
        {
            Destroy(gameObject);
            // Optionally trigger pooling or animations here
        }

        #endregion

        #region Movement

        private void HandleMovement()
        {
            if (!CanMove) return;

            // Apply movement logic using transport component
            float speed = transportComponent.SpeedMultiplier;
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        #endregion

        #region Combat

        private void HandleCombat()
        {
            if (!CanAttack || !weaponComponent.CanAttack) return;

            // Example: Use weapon range and attack speed logic here
            weaponComponent.Attack();
        }

        #endregion
    }
}
