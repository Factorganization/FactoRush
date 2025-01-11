using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    // Read a txt file and spawn enemies based on the data in the file
    
    # region Fields
    
    public static EnemySpawner Instance;

    public string enemyDataId;
    
    #endregion
    
    #region Methods
    
    #region Unity Methods
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        
    }
    
    private void Update()
    {
        
    }
    
    #endregion

    private void ReadFile()
    {
        // Load the file in Ressources/EnemyData/ with the id enemyDataId
        
    }

    private void ProcessFile()
    {
        // Process the data and invoke the SpawnEnemy method every 10 seconds with the data of the enemy to spawn next
    }
    
    private void SpawnEnemy(string enemyData)
    {
        // Parse the data and spawn the enemy
    }
    
    #endregion
    
}
