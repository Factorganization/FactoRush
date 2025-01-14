using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    # region Fields
    
    public static GameManager Instance;
    
    [SerializeField] private EnemySpawner enemySpawner;

    public string deck = "0102030412";

    public string LevelToLoad;
    
    [SerializeField] private TMP_Text victoryText;
    
    public bool CanStart { get; set; }
    
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
        CanStart = false;
    }
    
    private void Update()
    {
        DoInitStart();
    }
    
    #endregion
    
    public void AllyWin()
    {
        victoryText.text = "Victoire !";
        victoryText.gameObject.SetActive(true);
        StartCoroutine(LoadMainMenu());
    }
    
    public void EnemyWin()
    {
        victoryText.text = "Défaite !";
        victoryText.gameObject.SetActive(true);
        StartCoroutine(LoadMainMenu());
    }
    
    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(3);
        victoryText.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
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
            if (enemySpawner.enabled) return;
            enemySpawner.enabled = true;
        }
    }
    
    public void Play(int level)
    {
        if (level == 1)
        {
            deck = "0404141400";
        }
        if (level == 2)
        {
            deck = "0414090714";
        }
        enemySpawner.enemyDataId = level.ToString();
        LevelToLoad = level.ToString();
        SceneManager.LoadScene("ClemScene");
    }
    
    
    #endregion
}
