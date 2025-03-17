using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    public abstract class TransportComponent : UnitComponent
    {
        [Header("Graph")]
        public GameObject graph; // Graphical representation of the unit
        
        [Header("Transport Stats")]
        public float speedMultiplier = 1.5f; // Speed boost
        public float healthMultiplier = 1.2f; // Health boost
        public bool isFlying;        // Determines if the unit is aerial

        public virtual void UniqueBehavior(Unit unit, Unit target = null)
        {
            
        }
        
    }
}