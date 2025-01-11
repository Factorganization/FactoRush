using System;
using System.Collections;
using System.Collections.Generic;
using GameContent.Entities.UnmanagedEntities;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
   
    #region Fields
    
    [SerializeField] public List<Unit> allyUnits;
    [SerializeField] public List<Unit> enemyUnits;
    [SerializeField] private GameObject unitPrefab;
    
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
        StartCoroutine(SpawnUnitCoroutine(isAlly, transportComponent, weaponComponent, cloneOf, delay));
    }
    
    private IEnumerator SpawnUnitCoroutine(bool isAlly, TransportComponent transportComponent =  null, WeaponComponent weaponComponent = null, Unit cloneOf = null, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        // Instantiate the unit prefab and set its components
        GameObject unit = Instantiate(unitPrefab, isAlly ? allyBase.spawnPoint.position : enemyBase.spawnPoint.position, isAlly ? allyBase.spawnPoint.rotation : enemyBase.spawnPoint.rotation);
        Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.isAlly = isAlly;
        unitComponent.transportComponent = transportComponent;
        unitComponent.weaponComponent = weaponComponent;
        unitComponent.allyBase = allyBase;
        unitComponent.enemyBase = enemyBase;
        
        // Add the unit to the units array
        if (isAlly)
        {
            allyUnits.Add(unitComponent);
        }
        else
        {
            enemyUnits.Add(unitComponent);
        }
        
        // Set the clone bool if Needed
        if (cloneOf != null)
        {
            unitComponent.alreadyCloned = true;
        }
    }

    private void InitialCheckup()
    {
        if (allyBase == null)
        {
            allyBase = AllyBase.Instance;
            Debug.LogWarning("Ally base is not assigned in the inspector. Assigning it to the instance of AllyBase");
        }
        if (enemyBase == null)
        {
            enemyBase = EnemyBase.Instance;
            Debug.LogWarning("Enemy base is not assigned in the inspector. Assigning it to the instance of EnemyBase");
        }
        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned in the inspector. Please assign it.");
        }

        if (allyBase != null && enemyBase != null) return;
        Debug.LogError(allyBase == null ? "Ally base is not instantiated" : "Enemy base is not instantiated");
    }
}
