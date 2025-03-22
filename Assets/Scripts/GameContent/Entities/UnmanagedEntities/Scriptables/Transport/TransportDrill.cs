using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    [CreateAssetMenu(fileName = "TransportDrill", menuName = "Components/TransportsComponent/TransportDrill")]
    public class TransportDrill : TransportComponent
    {
        public override void UniqueBehavior(Unit unit, Unit target = null)
        {
            Unit closestEnemyUnit = null;
            Unit closestAllyUnit = null;
                
            if (unit.isAlly)
            {
                // Find the closest enemy unit by iterating through all enemy units and checking their transform position
                var closestDistance = Mathf.Infinity;
                
                foreach (var enemyUnit in UnitsManager.Instance.enemyUnits)
                {
                    // Si l'unité est volante on la skip
                    if (enemyUnit.isAirUnit)
                        continue;
                    
                    if (enemyUnit.ETransform.position.x > 53)
                        continue; // On skip les unités qui sont après les 2 tiers du terrain
                        
                    var distance = Vector3.Distance(unit.transform.position, enemyUnit.transform.position);
                    
                    if (distance >= closestDistance)
                        continue;
                    
                    closestDistance = distance;
                    closestEnemyUnit = enemyUnit;
                }

                if (closestEnemyUnit is not null)
                    unit.transform.position = closestEnemyUnit.transform.position - closestEnemyUnit.transform.forward;
            }
            else
            {
                // Find the closest ally unit by iterating through all allies units and checking their transform position
                var closestDistance = Mathf.Infinity;
                
                foreach (var allyUnit in UnitsManager.Instance.allyUnits)
                {
                    // Si l'unité est volante on la skip
                    if (allyUnit.isAirUnit)
                        continue;
                    
                    if (allyUnit.ETransform.position.x < 46) 
                        continue; // On skip les unités qui sont après les 2 tiers du terrain
                        
                    var distance = Vector3.Distance(unit.transform.position, allyUnit.transform.position);
                    
                    if (distance >= closestDistance)
                        continue;
                    
                    closestDistance = distance;
                    closestAllyUnit = allyUnit;
                }
                
                if (closestAllyUnit is not null)
                    unit.transform.position = closestAllyUnit.transform.position - closestAllyUnit.transform.forward;
            }
        }
    }
}