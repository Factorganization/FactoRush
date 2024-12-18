using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    [CreateAssetMenu(fileName = "NewTransportComponent", menuName = "Components/TransportComponent")]
    public class TransportComponent : ScriptableObject
    {
        [Header("Transport Stats")]
        public float SpeedMultiplier = 1.5f; // Speed boost
        public float HealthMultiplier = 1.2f; // Health boost
        public bool IsFlying = false;        // Determines if the unit is aerial

        // Convenience property to determine mobility
        public bool CanMove => SpeedMultiplier > 0;
    }
}