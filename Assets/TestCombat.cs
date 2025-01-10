using GameContent.Entities.UnmanagedEntities;
using UnityEngine;

public class TestCombat : MonoBehaviour
{

    [SerializeField] private WeaponComponent weaponComponentAlly;
    [SerializeField] private TransportComponent transportComponentAlly;
    [SerializeField] private WeaponComponent weaponComponentEnemy;
    [SerializeField] private TransportComponent transportComponentEnemy;
    
    private bool isPaused = false;
    
    
    
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
}
