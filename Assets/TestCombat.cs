using System;
using GameContent.Entities.UnmanagedEntities;
using UnityEngine;
using UnityEngine.UI;

public class TestCombat : MonoBehaviour
{

    [SerializeField] private WeaponComponent weaponComponentAlly;
    [SerializeField] private TransportComponent transportComponentAlly;
    [SerializeField] private WeaponComponent weaponComponentEnemy;
    [SerializeField] private TransportComponent transportComponentEnemy;
    
    private bool isPaused = false;
    
    [Header("Toggles")]
    [SerializeField] private Toggle weaponBaseToggleAlly;
    [SerializeField] private Toggle weaponArtilleryToggleAlly;
    [SerializeField] private Toggle weaponC4ToggleAlly;
    [SerializeField] private Toggle weaponCanonToggleAlly;
    [SerializeField] private Toggle weaponMinigunToggleAlly;
    [SerializeField] private Toggle weaponRailgunToggleAlly;
    [SerializeField] private Toggle weaponShieldToggleAlly;
    [SerializeField] private Toggle weaponSpearToggleAlly;
    [SerializeField] private Toggle weaponSpinningBladeToggleAlly;
    [SerializeField] private Toggle weaponSymbioticRifleToggleAlly;
    [SerializeField] private Toggle weaponWarhammerToggleAlly;
    
    [SerializeField] private Toggle transportBaseToggleAlly;
    [SerializeField] private Toggle transportPropellerToggleAlly;
    [SerializeField] private Toggle transportThornmailToggleAlly;
    [SerializeField] private Toggle transportAccumulatorToggleAlly;
    [SerializeField] private Toggle transportDrillToggleAlly;
    [SerializeField] private Toggle transportInsulatingWheelsToggleAlly;
    [SerializeField] private Toggle transportSliderToggleAlly;
    [SerializeField] private Toggle transportTwinBootsToggleAlly;
    
    [SerializeField] private Toggle weaponBaseToggleEnemy;
    [SerializeField] private Toggle weaponArtilleryToggleEnemy;
    [SerializeField] private Toggle weaponC4ToggleEnemy;
    [SerializeField] private Toggle weaponCanonToggleEnemy;
    [SerializeField] private Toggle weaponMinigunToggleEnemy;
    [SerializeField] private Toggle weaponRailgunToggleEnemy;
    [SerializeField] private Toggle weaponShieldToggleEnemy;
    [SerializeField] private Toggle weaponSpearToggleEnemy;
    [SerializeField] private Toggle weaponSpinningBladeToggleEnemy;
    [SerializeField] private Toggle weaponSymbioticRifleToggleEnemy;
    [SerializeField] private Toggle weaponWarhammerToggleEnemy;
        
    [SerializeField] private Toggle transportBaseToggleEnemy;
    [SerializeField] private Toggle transportPropellerToggleEnemy;
    [SerializeField] private Toggle transportThornmailToggleEnemy;
    [SerializeField] private Toggle transportAccumulatorToggleEnemy;
    [SerializeField] private Toggle transportDrillToggleEnemy;
    [SerializeField] private Toggle transportInsulatingWheelsToggleEnemy;
    [SerializeField] private Toggle transportSliderToggleEnemy;
    [SerializeField] private Toggle transportTwinBootsToggleEnemy;
    

    [Header("Components")] 
    [SerializeField] private WeaponComponent weaponBase;
    [SerializeField] private WeaponComponent weaponArtillery;
    [SerializeField] private WeaponComponent weaponC4;
    [SerializeField] private WeaponComponent weaponCanon;
    [SerializeField] private WeaponComponent weaponMinigun;
    [SerializeField] private WeaponComponent weaponRailgun;
    [SerializeField] private WeaponComponent weaponShield;
    [SerializeField] private WeaponComponent weaponSpear;
    [SerializeField] private WeaponComponent weaponSpinningBlade;
    [SerializeField] private WeaponComponent weaponSymbioticRifle;
    [SerializeField] private WeaponComponent weaponWarhammer;
    

    
    [SerializeField] private TransportComponent transportBase;
    [SerializeField] private TransportComponent transportPropeller;
    [SerializeField] private TransportComponent transportThornmail;
    [SerializeField] private TransportComponent transportAccumulator;
    [SerializeField] private TransportComponent transportDrill;
    [SerializeField] private TransportComponent transportInsulatingWheels;
    [SerializeField] private TransportComponent transportSlider;
    [SerializeField] private TransportComponent transportTwinBoots;


    private void Awake()
    {
        // Add a listener to all toggle, when they are clicked, uncheck all other toggles of the same group and change the weapon/transport component
        weaponBaseToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponBaseToggleAlly, true, true); weaponComponentAlly = weaponBase; });
        weaponArtilleryToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponArtilleryToggleAlly, true, true); weaponComponentAlly = weaponArtillery; });
        weaponC4ToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponC4ToggleAlly, true, true); weaponComponentAlly = weaponC4; });
        weaponCanonToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponCanonToggleAlly, true, true); weaponComponentAlly = weaponCanon; });
        weaponMinigunToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponMinigunToggleAlly, true, true); weaponComponentAlly = weaponMinigun; });
        weaponRailgunToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponRailgunToggleAlly, true, true); weaponComponentAlly = weaponRailgun; });
        weaponShieldToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponShieldToggleAlly, true, true); weaponComponentAlly = weaponShield; });
        weaponSpearToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponSpearToggleAlly, true, true); weaponComponentAlly = weaponSpear; });
        weaponSpinningBladeToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponSpinningBladeToggleAlly, true, true); weaponComponentAlly = weaponSpinningBlade; });
        weaponSymbioticRifleToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponSymbioticRifleToggleAlly, true, true); weaponComponentAlly = weaponSymbioticRifle; });
        weaponWarhammerToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(weaponWarhammerToggleAlly, true, true); weaponComponentAlly = weaponWarhammer; });
        
        transportBaseToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportBaseToggleAlly, false, true); transportComponentAlly = transportBase; });
        transportPropellerToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportPropellerToggleAlly, false, true); transportComponentAlly = transportPropeller; });
        transportThornmailToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportThornmailToggleAlly, false, true); transportComponentAlly = transportThornmail; });
        transportAccumulatorToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportAccumulatorToggleAlly, false, true); transportComponentAlly = transportAccumulator; });
        transportDrillToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportDrillToggleAlly, false, true); transportComponentAlly = transportDrill; });
        transportInsulatingWheelsToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportInsulatingWheelsToggleAlly, false, true); transportComponentAlly = transportInsulatingWheels; });
        transportSliderToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportSliderToggleAlly, false, true); transportComponentAlly = transportSlider; });
        transportTwinBootsToggleAlly.onValueChanged.AddListener(delegate { UncheckToggle(transportTwinBootsToggleAlly, false, true); transportComponentAlly = transportTwinBoots; });
        
        weaponBaseToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponBaseToggleEnemy, true, false); weaponComponentEnemy = weaponBase; });
        weaponArtilleryToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponArtilleryToggleEnemy, true, false); weaponComponentEnemy = weaponArtillery; });
        weaponC4ToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponC4ToggleEnemy, true, false); weaponComponentEnemy = weaponC4; });
        weaponCanonToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponCanonToggleEnemy, true, false); weaponComponentEnemy = weaponCanon; });
        weaponMinigunToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponMinigunToggleEnemy, true, false); weaponComponentEnemy = weaponMinigun; });
        weaponRailgunToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponRailgunToggleEnemy, true, false); weaponComponentEnemy = weaponRailgun; });
        weaponShieldToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponShieldToggleEnemy, true, false); weaponComponentEnemy = weaponShield; });
        weaponSpearToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponSpearToggleEnemy, true, false); weaponComponentEnemy = weaponSpear; });
        weaponSpinningBladeToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponSpinningBladeToggleEnemy, true, false); weaponComponentEnemy = weaponSpinningBlade; });
        weaponSymbioticRifleToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponSymbioticRifleToggleEnemy, true, false); weaponComponentEnemy = weaponSymbioticRifle; });
        weaponWarhammerToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(weaponWarhammerToggleEnemy, true, false); weaponComponentEnemy = weaponWarhammer; });
        
        transportBaseToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportBaseToggleEnemy, false, false); transportComponentEnemy = transportBase; });
        transportPropellerToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportPropellerToggleEnemy, false, false); transportComponentEnemy = transportPropeller; });
        transportThornmailToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportThornmailToggleEnemy, false, false); transportComponentEnemy = transportThornmail; });
        transportAccumulatorToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportAccumulatorToggleEnemy, false, false); transportComponentEnemy = transportAccumulator; });                     
        transportDrillToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportDrillToggleEnemy, false, false); transportComponentEnemy = transportDrill; });
        transportInsulatingWheelsToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportInsulatingWheelsToggleEnemy, false, false); transportComponentEnemy = transportInsulatingWheels; });
        transportSliderToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportSliderToggleEnemy, false, false); transportComponentEnemy = transportSlider; });
        transportTwinBootsToggleEnemy.onValueChanged.AddListener(delegate { UncheckToggle(transportTwinBootsToggleEnemy, false, false); transportComponentEnemy = transportTwinBoots; });
        
        oldWeaponAlly = weaponBaseToggleAlly;
        oldWeaponEnemy = weaponBaseToggleEnemy;
        oldTransportAlly = transportBaseToggleAlly;
        oldTransportEnemy = transportBaseToggleEnemy;
    }


    public void SpawnUnitAlly()
    {
        UnitsManager unitsManager = FindObjectOfType<UnitsManager>();
        unitsManager.SpawnUnit(true, transportComponentAlly, weaponComponentAlly);
    }
    
    public void SpawnUnitEnemy()
    {
        UnitsManager unitsManager = FindObjectOfType<UnitsManager>();
        unitsManager.SpawnUnit(false, transportComponentEnemy, weaponComponentEnemy);
    }
    
    
    
    # region Pause
    
    public void PauseResumeTime()
    {
        if (isPaused)
        {
            ResumeTime();
        }
        else
        {
            PauseTime();
        }

        isPaused = !isPaused;
    }
    
    private void PauseTime()
    {
        Time.timeScale = 0;
    }
    
    private void ResumeTime()
    {
        Time.timeScale = 1;
    }
    
    # endregion
    
    #region Components

    private Toggle oldWeaponAlly;
    private Toggle oldWeaponEnemy;
    private Toggle oldTransportAlly;
    private Toggle oldTransportEnemy;

    private void UncheckToggle(Toggle toggle, bool isWeapon, bool isAlly)
    {
        // make the toggle the new old toggle and uncheck the old toggle
        if (isAlly)
        {
            if (isWeapon)
            {
                if (oldWeaponAlly != null)
                {
                    oldWeaponAlly.isOn = false;
                }
                oldWeaponAlly = toggle;
            }
            else
            {
                if (oldTransportAlly != null)
                {
                    oldTransportAlly.isOn = false;
                }
                oldTransportAlly = toggle;
            }
        }
        else
        {
            if (isWeapon)
            {
                if (oldWeaponEnemy != null)
                {
                    oldWeaponEnemy.isOn = false;
                }
                oldWeaponEnemy = toggle;
            }
            else
            {
                if (oldTransportEnemy != null)
                {
                    oldTransportEnemy.isOn = false;
                }
                oldTransportEnemy = toggle;
            }
        }
    }
    
    
    
    #endregion
}
