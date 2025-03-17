using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    [CreateAssetMenu(fileName = "TransportTwinBoots", menuName = "Components/TransportsComponent/TransportTwinBoots")]
    public class TransportTwinBoots : TransportComponent
    { 
        public override void UniqueBehavior(Unit unit, Unit target = null)
        {
            UnitsManager.Instance.SpawnUnit(unit.isAlly, unit.transportComponent, unit.weaponComponent, unit, 0.5f);
        }
    }
}