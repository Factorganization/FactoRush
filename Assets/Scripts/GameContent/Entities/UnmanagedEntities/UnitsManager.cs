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
        StartCoroutine(SpawnUnitCoroutine(isAlly, transportComponent, weaponComponent, cloneOf, delay));
    }
    
    private IEnumerator SpawnUnitCoroutine(bool isAlly, TransportComponent transportComponent = null, WeaponComponent weaponComponent = null, Unit cloneOf = null, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        // Instantiate the unit prefab and set its components
        var unitComponent = Instantiate(unitPrefab, isAlly ? allyBase.spawnPoint.position : enemyBase.spawnPoint.position, isAlly ? allyBase.spawnPoint.rotation : enemyBase.spawnPoint.rotation);
        //Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.isAlly = isAlly;
        // Set the default components if the parameters are null
        if (transportComponent == null)
        {
            transportComponent = transportComponentDefault;
        }
        if (weaponComponent == null)
        {
            weaponComponent = weaponComponentDefault;
        }
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
            Debug.LogError("Charles lit l'erreur stp");
        }

        if (allyBase != null && enemyBase != null) return;
        Debug.LogError(allyBase == null ? "Ally base is not instantiated" : "Enemy base is not instantiated");
    }
}
