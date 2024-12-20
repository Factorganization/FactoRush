using GameContent.Entities.UnmanagedEntities;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
   
    #region Fields
    
    [SerializeField] private Unit[] units;
    [SerializeField] private GameObject unitPrefab;
    
    [Header("Factory")]
    [SerializeField] private AllyBase allyBase;
    [SerializeField] private EnemyBase enemyBase;
    
    #endregion

    private void Start()
    {
        InitialCheckup();
    }
    
    public void SpawnUnit(bool isAlly, TransportComponent transportComponent =  null, WeaponComponent weaponComponent = null)
    {
        Instantiate(unitPrefab, isAlly ? allyBase.transform : enemyBase.transform);
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
        return;
    }
}
