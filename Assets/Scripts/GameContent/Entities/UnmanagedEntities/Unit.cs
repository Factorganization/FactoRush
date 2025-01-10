using System.Collections;
using System.Collections.Generic;
using GameContent.Entities.UnmanagedEntities.Scriptables.Transport;
using GameContent.Entities.UnmanagedEntities.Scriptables.Weapons;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameContent.Entities.UnmanagedEntities
{
    public class Unit : Entity
    {
        #region Fields

        [Header("Unit Components")]
        [SerializeField] public WeaponComponent weaponComponent;       // Reference to weapon stats
        [SerializeField] public TransportComponent transportComponent; // Reference to transport stats
        
        
        [Header("Vision Cone")]
        [SerializeField] public Material VisionConeMaterial;          // Material for the vision cone
        [SerializeField] public float ConeAngle = 60;                 // Field of view angle
        [SerializeField] public LayerMask ObstructionLayer;           // Layer mask for vision cone obstruction
        [SerializeField] public float offsetX = 0;                    // Horizontal offset for the cone origin
        [SerializeField] public float offsetY = 0;                    // Vertical offset for the cone origin
        [SerializeField] public float offsetZ = 0;                    // Forward offset for the cone origin
        
        [Header("Hp Bar")]
        [SerializeField] public Slider hpBar;

        private Mesh visionConeMesh;
        private MeshFilter meshFilter;
        public float currentHealth;
        private float maxHealth;

        public AllyBase allyBase;
        public EnemyBase enemyBase;
        
        
        

        #endregion

        #region Properties

        public bool isAlly;
        public bool IsAlive => currentHealth > 0;
        public bool CanAttack => weaponComponent != null && attackCooldown <= 0;
        public bool CanMove => transportComponent != null;

        public Vector3 velocity;

        public bool isAirUnit => transportComponent != null && transportComponent.IsFlying;
        
        [Header("Special Weapon Effects")]
        
        private bool isStunned = false;
        private float attackCooldown = 0;
        public float Range;
        public bool isExplosive = false;
        public float attackSpeed;
        public float moveSpeed;
        public float damage;
        public float ExplosiveRange;
        public bool alreadyCloned = false;
        public bool canDash = false;
        private bool hasDash = false;

        #endregion

        #region Unity Lifecycle

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        private void Start()
        {
            InitializeUnit();
            InitializeVisionCone();
        }

        private void Update()
        {
            if (!IsAlive) return;
            
            UpdateHpBar(); // Update the health bar every frame
            
            if (isStunned) return;
            
            if (canDash && !hasDash && Range <= 1)
            {
                var transportComponent = this.transportComponent as TransportSlider;
                var target = GetAllUnitsInRange(transportComponent.dashRange);
                if (target.Count > 0)
                {
                    transportComponent.UniqueBehavior(this, target[0]);
                    hasDash = true;
                }
            }
            
            DrawVisionCone(); // Update the vision cone every frame
            UpdateCooldown(); 
            
            if (!HandleCombat())
            {
                HandleMovement();
            }

        }

        #endregion

        #region Initialization

        private void InitializeUnit()
        {
            float baseHealth = 100;
            damage = weaponComponent != null ? weaponComponent.Damage : 0;
            attackSpeed = weaponComponent != null ? weaponComponent.AttackSpeed : 9999;
            Range = weaponComponent != null ? weaponComponent.Range : 0;
            moveSpeed = transportComponent != null ? transportComponent.SpeedMultiplier : 0;
            
            if (transportComponent != null && transportComponent is TransportTwinBoots)
            {
                baseHealth = 50;
                damage /= 2;
                
                // Clone if no Parent and make as a child so its not recursive
                if (alreadyCloned is false)
                {
                    transportComponent.UniqueBehavior(this);
                }
            }
            
            
            currentHealth = transportComponent != null ? baseHealth * transportComponent.HealthMultiplier : baseHealth;
            maxHealth = currentHealth;
            
            if (isAirUnit)
            {
                transform.position += Vector3.up * 3f; // Lift the unit off the ground
            }
            
            if (weaponComponent != null)
            {
                var weaponGraph = Instantiate(weaponComponent.Graph, transform);
                weaponGraph.transform.parent = graphTransform;
                weaponComponent.Initialize(this);
            }
            if (transportComponent != null)
            {
                var transportGraph = Instantiate(transportComponent.Graph, transform);
                transportGraph.transform.parent = graphTransform;
                if (transportComponent is TransportDrill)
                {
                    transportComponent.UniqueBehavior(this);
                }
                if (transportComponent is TransportSlider)
                {
                    canDash = true;
                }
            }
        }

        private void InitializeVisionCone()
        {
            gameObject.AddComponent<MeshRenderer>().material = VisionConeMaterial;
            meshFilter = gameObject.AddComponent<MeshFilter>();
            visionConeMesh = new Mesh();
        }

        #endregion

        #region Vision Cone

        private void DrawVisionCone()
        {
            if (!weaponComponent) return;

            float visionRange = Range;
            float visionAngle = ConeAngle * Mathf.Deg2Rad;
            int resolution = 120;

            Vector3[] vertices = new Vector3[resolution + 1];
            int[] triangles = new int[(resolution - 1) * 3];

            // Adjust the origin of the cone based on the offsets
            Vector3 coneOrigin = transform.position + transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ;
            vertices[0] = transform.InverseTransformPoint(coneOrigin);

            float currentAngle = -visionAngle / 2;
            float angleIncrement = visionAngle / (resolution - 1);

            for (int i = 0; i < resolution; i++)
            {
                float sin = Mathf.Sin(currentAngle);
                float cos = Mathf.Cos(currentAngle);

                // Direction relative to the unit's orientation
                Vector3 direction = transform.rotation * (Vector3.forward * cos + Vector3.right * sin);
                Vector3 vertexPosition;

                // Raycast for obstructions
                if (Physics.Raycast(coneOrigin, direction, out RaycastHit hit, visionRange, ObstructionLayer))
                {
                    vertexPosition = direction.normalized * hit.distance;
                }
                else
                {
                    vertexPosition = direction.normalized * visionRange;
                }

                vertices[i + 1] = transform.InverseTransformPoint(coneOrigin + vertexPosition);
                currentAngle += angleIncrement;
            }

            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 2;
            }

            visionConeMesh.Clear();
            visionConeMesh.vertices = vertices;
            visionConeMesh.triangles = triangles;
            meshFilter.mesh = visionConeMesh;
        }

        #endregion

        #region Combat

        private bool HandleCombat()
        {

            if (isStunned) return false;
            if (!CanAttack) return false;
            
            List<Unit> unitsInRange = GetAllUnitsInRange(Range);
            
            //if there is at least one unit in range, attack it
            if (unitsInRange.Count > 0)
            {
                // Attack the first valid unit target, if any
                AttackTarget(unitsInRange);
                return true;
            }

            // Adjust the origin of the cone based on the offsets
            Vector3 coneOrigin = transform.position + transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ;

            // Find all potential targets within the attack range
            Collider[] hitColliders = Physics.OverlapSphere(coneOrigin, Range);

            // Handle base attacks (unchanged behavior)
            foreach (var hitCollider in hitColliders)
            {
                if (isAlly)
                {
                    EnemyBase enemyBase = hitCollider.GetComponent<EnemyBase>();
                    if (enemyBase != null && IsInCone(enemyBase.transform.position, coneOrigin))
                    {
                        AttackEnemyBase(enemyBase);
                        return true;
                    }
                }
                else
                {
                    AllyBase allyBase = hitCollider.GetComponent<AllyBase>();
                    if (allyBase != null && IsInCone(allyBase.transform.position, coneOrigin))
                    {
                        AttackAllyBase(allyBase);
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Unit> GetAllUnitsInRange(float range)
        {
            // Adjust the origin of the cone based on the offsets
            Vector3 coneOrigin = transform.position + transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ;

            // Find all potential targets within the attack range
            Collider[] hitColliders = Physics.OverlapSphere(coneOrigin, range);
            //draw that sphere 

            // Collect all valid unit targets in range
            List<Unit> unitsInRange = new List<Unit>();
            foreach (var hitCollider in hitColliders)
            {
                Unit target = hitCollider.GetComponent<Unit>();
                if (target != null && target.IsAlive && IsValidTarget(target) && IsInCone(target.transform.position, coneOrigin))
                {
                    unitsInRange.Add(target);
                }
            }
            return unitsInRange;
        }


        private bool IsInCone(Vector3 targetPosition, Vector3 coneOrigin)
        {
            Vector3 directionToTarget = (targetPosition - coneOrigin).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            return angle <= ConeAngle / 2;
        }

        private void AttackTarget(List<Unit> unitsInRange)
        {
            if (!CanAttack) return;
            if (isExplosive)
            {
                if (Mathf.Approximately(Range, weaponComponent.Range))
                {
                    Range = ExplosiveRange;
                    return;
                }
            }

            weaponComponent.Attack(this, unitsInRange[0], unitsInRange );
            ResetCooldown();
            ResetRange();

            Debug.Log($"{name} attacked {unitsInRange[0].name} for {weaponComponent.Damage} damage.");
        }
        
        private void AttackEnemyBase(EnemyBase enemyBase)
        {
            if (!CanAttack) return;

            enemyBase.TakeDamage(weaponComponent.Damage);
            ResetCooldown();

            Debug.Log($"{name} attacked {enemyBase.name} for {weaponComponent.Damage} damage.");
        }
        
        private void AttackAllyBase(AllyBase allyBase)
        {
            if (!CanAttack) return;

            allyBase.TakeDamage(weaponComponent.Damage);
            ResetCooldown();

            Debug.Log($"{name} attacked {allyBase.name} for {weaponComponent.Damage} damage.");
        }

        private bool IsValidTarget(Unit target)
        {
            if (target == this) return false;
            if (target.isAlly == isAlly) return false;
            
            // if the target is behind this unit, it is not a valid target\
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            if (angle > 90)
            {
                return isExplosive; // If the target is behind, only allow explosive weapons to hit
            }
            

            return weaponComponent.targetType switch
            {
                TargetType.Ground => !target.isAirUnit,
                TargetType.Air => target.isAirUnit,
                TargetType.Both => true,
                _ => false
            };
        }
        
        private void UpdateCooldown()
        {
            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;
        }
        
        private void ResetCooldown()
        {
            attackCooldown = attackSpeed;
            velocity = Vector3.zero;
        }
        
        private void ResetAttackSpeed()
        {
            attackSpeed = weaponComponent.AttackSpeed;
        }
        
        private void ResetRange()
        {
            Range = weaponComponent.Range;
        }

        #endregion

        #region Movement

        private void HandleMovement()
        {
            if (!CanMove) return;
            if (weaponComponent != null && !CanAttack) return;
            if (isStunned) return;
            
            ResetAttackSpeed();
            
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            velocity = transform.forward * moveSpeed;
        }
        
        public void Dash(Vector3 direction, float distance)
        {
            if (!canDash) return;
            if (isStunned) return;
            
            transform.position += direction * distance;
        }

        #endregion

        #region Health Management
        
        private void UpdateHpBar()
        {
            if (hpBar != null)
            {
                //Fill it with a ratio of the current health over the max health
                hpBar.value = currentHealth / maxHealth;
            }
        }

        public void ApplyDamage(float damage)
        {
            //if TransportComponent is a TransportThornmail, it will reflect damage
            if (transportComponent != null && transportComponent is TransportThornmail)
            {
                transportComponent.UniqueBehavior(this, GetAllUnitsInRange(3)[0]);
            }
            if (transportComponent != null && transportComponent is TransportAccumulator)
            {
                transportComponent.UniqueBehavior(this);
            }
            
            if (transportComponent != null && transportComponent is TransportSlider && !hasDash)
            {
                transportComponent.UniqueBehavior(this);
                hasDash = true;
            }
            
            ApplyDamageRaw(damage);

        }
        
        public void ApplyDamageRaw(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                DestroyUnit();
        }

        private void DestroyUnit()
        {
            //remove the unit from the list of units
            if (isAlly)
            {
                UnitsManager.Instance.allyUnits.Remove(this);
            }
            else
            {
                UnitsManager.Instance.enemyUnits.Remove(this);
            }
            Destroy(gameObject);
        }

        #endregion
        
        # region Shield
        
        public void AddShield(float shield)
        {
            currentHealth += shield; //TODO faire un vrai shield  ? 
        }
        
        #endregion
        
        # region Stun
        
        public void Stun(float duration)
        {
            if (transportComponent != null && transportComponent is TransportInsulatingWheels) return; //Boots Imune to stun
            
            Debug.Log($"{name} is stunned for {duration} seconds.");
            isStunned = true;
            StartCoroutine(HandleStun(duration));
        }
        
        //Coroutine that will handle the stun duration
        
        private IEnumerator HandleStun(float duration)
        {
            yield return new WaitForSeconds(duration);
            isStunned = false;
            Debug.Log($"{name} is no longer stunned.");
        }  
        
        #endregion
        
        # region Gizmos
        
        private void OnDrawGizmos()
        {
            if (!weaponComponent) return;

            // Calculate the cone origin using the offsets
            Vector3 coneOrigin = transform.position + transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ;

            // Draw the overlap sphere
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.5f); // Light blue with transparency
            Gizmos.DrawWireSphere(coneOrigin, Range);
            
            if (Application.isPlaying) // Only run during Play mode
            {
                Collider[] hitColliders = Physics.OverlapSphere(coneOrigin, Range);
                foreach (var hitCollider in hitColliders)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(hitCollider.transform.position, 0.2f); // Red spheres for detected targets
                }
            }
        }
        
        #endregion

    }
    
}
