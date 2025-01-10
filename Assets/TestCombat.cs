using GameContent.Entities.UnmanagedEntities;
using UnityEngine;

public class TestCombat : MonoBehaviour
{

    [SerializeField] private WeaponComponent weaponComponent;
    [SerializeField] private TransportComponent transportComponent;
    

    
    public void SpawnUnit(bool isAlly = true)
    {
        UnitsManager unitsManager = FindObjectOfType<UnitsManager>();
        unitsManager.SpawnUnit(isAlly, transportComponent, weaponComponent);
    }
}
