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
    }
}