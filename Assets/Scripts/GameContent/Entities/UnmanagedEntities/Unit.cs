using System.Collections;
using GameContent.Entities.OnFieldEntities.Buildings;
using UnityEngine;

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

        private Mesh visionConeMesh;
        private MeshFilter meshFilter;
        public float currentHealth;

        public AllyBase allyBase;
        public EnemyBase enemyBase;

        #endregion

        #region Properties

        public bool isAlly;
        public bool IsAlive => currentHealth > 0;
        public bool CanAttack => weaponComponent != null;
        public bool CanMove => transportComponent != null;

        public bool isAirUnit => transportComponent != null && transportComponent.IsFlying;
        public bool isGroundUnit => transportComponent == null || !transportComponent.IsFlying;
        
        private bool isStunned = false;

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
            if (!IsAlive)
                return;

            HandleCombat();
            HandleMovement();
            DrawVisionCone(); // Update the vision cone every frame
        }

        #endregion

        #region Initialization

        private void InitializeUnit()
        {
            float baseHealth = 100;
            currentHealth = transportComponent != null ? baseHealth * transportComponent.HealthMultiplier : baseHealth;
            if (isAirUnit)
            {
                transform.position += Vector3.up * 2f; // Lift the unit off the ground
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

            float visionRange = weaponComponent.Range;
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

        private void HandleCombat()
        {
            if (isStunned) return;
            if (!CanAttack || !weaponComponent.CanAttack)
            {
                weaponComponent.UpdateCooldown();
                return;
            };
            

            // Adjust the origin of the cone based on the offsets
            Vector3 coneOrigin = transform.position + transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ; 
            
            // Find all potential targets within the attack range
            Collider[] hitColliders = Physics.OverlapSphere(coneOrigin, weaponComponent.Range);
            //draw that sphere 
    
            foreach (var hitCollider in hitColliders)
            {
                Unit target = hitCollider.GetComponent<Unit>();
                if (target != null && target.IsAlive && IsValidTarget(target) && IsInCone(target.transform.position, coneOrigin))
                {
                    AttackTarget(target);
                }

                if (isAlly)
                {
                    EnemyBase enemyBase = hitCollider.GetComponent<EnemyBase>();
                    if (enemyBase != null && IsInCone(enemyBase.transform.position, coneOrigin))
                    {
                        AttackEnemyBase(enemyBase);
                    }
                }
                else
                {
                    AllyBase allyBase = hitCollider.GetComponent<AllyBase>();
                    if (allyBase != null && IsInCone(allyBase.transform.position, coneOrigin))
                    {
                        AttackAllyBase(allyBase);
                    }
                }
            }

            weaponComponent.UpdateCooldown();
        }

        private bool IsInCone(Vector3 targetPosition, Vector3 coneOrigin)
        {
            Vector3 directionToTarget = (targetPosition - coneOrigin).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            return angle <= ConeAngle / 2;
        }

        private void AttackTarget(Unit target)
        {
            if (!weaponComponent.CanAttack) return;

            target.ApplyDamage(weaponComponent.Damage);
            //weaponComponent.Attack(); TODO

            Debug.Log($"{name} attacked {target.name} for {weaponComponent.Damage} damage.");
        }
        
        private void AttackEnemyBase(EnemyBase enemyBase)
        {
            if (!weaponComponent.CanAttack) return;

            enemyBase.TakeDamage(weaponComponent.Damage);
            //weaponComponent.Attack(); TODO

            Debug.Log($"{name} attacked {enemyBase.name} for {weaponComponent.Damage} damage.");
        }
        
        private void AttackAllyBase(AllyBase allyBase)
        {
            if (!weaponComponent.CanAttack) return;

            allyBase.TakeDamage(weaponComponent.Damage);
            //weaponComponent.Attack(); TODO

            Debug.Log($"{name} attacked {allyBase.name} for {weaponComponent.Damage} damage.");
        }

        private bool IsValidTarget(Unit target)
        {
            switch (weaponComponent.targetType)
            {
                case TargetType.Ground:
                    return target.isGroundUnit;
                case TargetType.Air:
                    return target.isAirUnit;
                case TargetType.Both:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Movement

        private void HandleMovement()
        {
            if (!CanMove) return;
            if (weaponComponent != null && !weaponComponent.CanAttack) return;
            if (isStunned) return;
            
            float speed = transportComponent.SpeedMultiplier;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        #endregion

        #region Health Management

        public void ApplyDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                DestroyUnit();
        }

        private void DestroyUnit()
        {
            Destroy(gameObject);
        }

        #endregion
        
        # region Shield
        
        public void AddShield(float shield)
        {
            currentHealth += shield;
        }
        
        #endregion
        
        # region Stun
        
        public void Stun(float duration)
        {
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
            Gizmos.DrawWireSphere(coneOrigin, weaponComponent.Range);
            
            if (Application.isPlaying) // Only run during Play mode
            {
                Collider[] hitColliders = Physics.OverlapSphere(coneOrigin, weaponComponent.Range);
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
