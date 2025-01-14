using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    # region Fields
    
    public static GameManager Instance;
    
    [SerializeField] private EnemySpawner enemySpawner;

    public string deck = "0102030412";
    
    #endregion
    
    #region Methods
    
    #region Unity Methods
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        DontDestroyOnLoad(this);
        DoInitStart();
    }
    
    private void Update()
    {
        
    }
    
    #endregion
    
    private void DoInitStart()
    {
        // if CurrentScene is MainMenu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            enemySpawner.enabled = false;
        }
        
        // if CurrentScene is Game
        if (SceneManager.GetActiveScene().name == "Game")
        {
            enemySpawner.enabled = true;
        }
    }
    
    #endregion
}
