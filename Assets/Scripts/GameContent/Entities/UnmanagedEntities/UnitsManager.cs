using GameContent.Entities.OnFieldEntities.Buildings;
using GameContent.Entities.UnmanagedEntities;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
   
    #region Fields
    
    [SerializeField] private Unit[] units;
    
    [Header("Factory")]
    [SerializeField] private FactoryBuilding allyFactory;
    [SerializeField] private FactoryBuilding enemyFactory;
    
    #endregion

}
