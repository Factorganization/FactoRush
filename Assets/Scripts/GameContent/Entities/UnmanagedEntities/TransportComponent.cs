using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{ 
    // TransportComponent holds stats related to the unit's transport/vehicle (if any).
    [CreateAssetMenu(fileName = "NewTransportComponent", menuName = "Components/TransportComponent")]
    public class TransportComponent : ScriptableObject
    {
        [Header("Transport Stats")]
        public float SpeedMultiplier = 1.5f; // Speed boost given by transport (if any).
        public float HealthMultiplier = 1.2f; // Health boost provided by transport (if any).
        public bool IsFlying = false; // Whether the unit has flying capabilities.

        // Can the transport move the unit? True if the unit can move, false otherwise.
        public bool CanMove => SpeedMultiplier > 0;
    }
}
