using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    [CreateAssetMenu(fileName = "TransportDrill", menuName = "Components/TransportsComponent/TransportDrill")]
    public class TransportDrill : TransportComponent
    {
        
        public override void UniqueBehavior(Unit unit, Unit target = null, List<Unit> allUnitsInRange = null)
        {
            // Spawn just behind the closest enemy unit 
            try
            {
                Unit closestEnemyUnit = null;
                Unit closestAllyUnit = null;
                
                if (unit.isAlly)
                {
                    // Find the closest enemy unit by iterating through all enemy units and checking their transform position
                    
                    float closestDistance = Mathf.Infinity;
                    foreach (var enemyUnit in UnitsManager.Instance.enemyUnits)
                    {
                        float distance = Vector3.Distance(unit.transform.position, enemyUnit.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemyUnit = enemyUnit;
                        }
                    }
                }
                else
                {
                    // Find the closest ally unit by iterating through all ally units and checking their transform position
   
                    float closestDistance = Mathf.Infinity;
                    foreach (var allyUnit in UnitsManager.Instance.allyUnits)
                    {
                        float distance = Vector3.Distance(unit.transform.position, allyUnit.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestAllyUnit = allyUnit;
                        }
                    }
                }
                
                // Teleport the unit right behind the closest unit
                unit.transform.position = unit.isAlly ? closestEnemyUnit.transform.position - closestEnemyUnit.transform.forward : closestAllyUnit.transform.position - closestAllyUnit.transform.forward;
            }
            catch (Exception e)
            {
                Debug.Log("No units found, normal spawn");
            }
            
        }
    }
}