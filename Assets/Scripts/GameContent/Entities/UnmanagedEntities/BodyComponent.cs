using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

namespace GameContent.Entities.UnmanagedEntities
{
    // BodyComponent holds stats related to the unit's health and movement speed.
    [CreateAssetMenu(fileName = "NewTransportComponent", menuName = "Components/BodyComponent")]
    public class BodyComponent : ScriptableObject
    {
        [Header("Health & Stats")]
        public float BaseHealth = 100f; 
        public float MovementSpeed = 5f; // Base movement speed (before transport modifier).
        
        [Header("Multipliers")]
        public float DamageMultiplier = 1f; // Damage multiplier for the unit (based on body type).
        public float SpeedMultiplier = 1f; // Speed multiplier (from transport or body type).

        [Header("Incomplete Unit Penalty")]
        public float IncompleteHealthLossRate = 2f; // Health loss per second if the unit is incomplete.
        
        // Determine which types of targets this unit prioritizes.
        public TargetType PreferredTargetType = TargetType.Ground; // Default is Ground targets.
    }
}
