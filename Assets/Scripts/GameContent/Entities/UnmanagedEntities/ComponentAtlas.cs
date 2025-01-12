using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class ComponentAtlas : MonoBehaviour
    {
        [Header("Transport Components")]
        [SerializeField] public TransportComponent transportBase;
        [SerializeField] public TransportComponent transportPropeller;
        [SerializeField] public TransportComponent transportThornmail;
        [SerializeField] public TransportComponent transportAccumulator;
        [SerializeField] public TransportComponent transportDrill;
        [SerializeField] public TransportComponent transportInsulatingWheels;
        [SerializeField] public TransportComponent transportSlider;
        [SerializeField] public TransportComponent transportTwinBoots;
        
        [Header("Weapon Components")] 
        [SerializeField] public WeaponComponent weaponBase;
        [SerializeField] public WeaponComponent weaponArtillery;
        [SerializeField] public WeaponComponent weaponC4;
        [SerializeField] public WeaponComponent weaponCanon;
        [SerializeField] public WeaponComponent weaponMinigun;
        [SerializeField] public WeaponComponent weaponRailgun;
        [SerializeField] public WeaponComponent weaponShield;
        [SerializeField] public WeaponComponent weaponSpear;
        [SerializeField] public WeaponComponent weaponSpinningBlade;
        [SerializeField] public WeaponComponent weaponSymbioticRifle;
        [SerializeField] public WeaponComponent weaponWarhammer;
        
        public static ComponentAtlas Instance;
        
        private void Awake()
        {
            Instance = this;
        }
    }
}

/*
// Transport IDs
01 - transportBase
02 - transportPropeller
03 - transportThornmail
04 - transportAccumulator
05 - transportDrill
06 - transportInsulatingWheels
07 - transportSlider
08 - transportTwinBoots

// Weapon IDs
01 - weaponBase
02 - weaponArtillery
03 - weaponC4
04 - weaponCanon
05 - weaponMinigun
06 - weaponRailgun
07 - weaponShield
08 - weaponSpear
09 - weaponSpinningBlade
10 - weaponSymbioticRifle
11 - weaponWarhammer
*/