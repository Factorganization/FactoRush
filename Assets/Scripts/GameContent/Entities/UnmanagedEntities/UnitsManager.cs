using System.Collections;
using System.Collections.Generic;
using GameContent.Entities.UnmanagedEntities.Bases;
using GameContent.Entities.UnmanagedEntities.Scriptables.Transport;
using GameContent.Entities.UnmanagedEntities.Scriptables.Weapons;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class UnitsManager : MonoBehaviour
    {
   
        #region Fields
    
        [SerializeField] public List<Unit> allyUnits;
        [SerializeField] public List<Unit> enemyUnits;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private WeaponComponent weaponComponentDefault;
        [SerializeField] private TransportComponent transportComponentDefault;
    
        [Header("Factory")]
        [SerializeField] private AllyBase allyBase;
        [SerializeField] private EnemyBase enemyBase;
    
        public static UnitsManager Instance;
    
        #endregion

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitialCheckup();
        }
    
        public void SpawnUnit(bool isAlly, TransportComponent transportComponent =  null, WeaponComponent weaponComponent = null, Unit cloneOf = null, float delay = 0)
        {
            if (unitPrefab is null)
            {
                //Debug.LogError("Unit prefab is not assigned in the inspector");
                return;
            }
            // if there is more than 20 units in total, don't spawn any more
            if (allyUnits.Count + enemyUnits.Count >= 20)
            {
                //Debug.LogWarning("There are already 20 units in total. Can't spawn more units");
                return;
            }
        
            if (Instance is null)
                return;
            
            StartCoroutine(SpawnUnitCoroutine(isAlly, transportComponent, weaponComponent, cloneOf, delay));
        }
    
        private IEnumerator SpawnUnitCoroutine(bool isAlly, TransportComponent transportComponent = null, WeaponComponent weaponComponent = null, Unit cloneOf = null, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            
            // Instantiate the unit prefab and set its components
            var unit = Instantiate(unitPrefab,
                isAlly
                    ? allyBase.spawnPoint.position
                    : enemyBase.spawnPoint.position,
                isAlly
                    ? allyBase.spawnPoint.rotation
                    : enemyBase.spawnPoint.rotation);
            
            //Unit unitComponent = unit.GetComponent<Unit>();
            unit.isAlly = isAlly;
            
            // Set the default components if the parameters are null
            transportComponent ??= transportComponentDefault;
            weaponComponent ??= weaponComponentDefault;
            
            unit.transportComponent = transportComponent;
            unit.weaponComponent = weaponComponent;
        
            unit.allyBase = allyBase;
            unit.enemyBase = enemyBase;
        
            // Add the unit to the units array
            if (isAlly)
            {
                allyUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
        
            // Set the clone bool if Needed
            if (cloneOf is not null)
            {
                unit.alreadyCloned = true;
            }
        }

        private void InitialCheckup()
        {
            if (allyBase is null)
            {
                allyBase = AllyBase.Instance;
                Debug.LogWarning("Ally base is not assigned in the inspector. Assigning it to the instance of AllyBase");
            }
            if (enemyBase is null)
            {
                enemyBase = EnemyBase.Instance;
                Debug.LogWarning("Enemy base is not assigned in the inspector. Assigning it to the instance of EnemyBase");
            }
            if (unitPrefab is null)
            {
                Debug.LogError("Charles lit l'erreur stp");
            }

            if (allyBase != null && enemyBase != null)
                return;
            
            Debug.LogError(allyBase == null ? "Ally base is not instantiated" : "Enemy base is not instantiated");
        }
    }
}
