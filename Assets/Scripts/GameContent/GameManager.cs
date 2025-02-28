using System.Collections;
using GameContent.MainMenu;
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
    
    [SerializeField] private GameObject victoryScreen;
    
    [SerializeField] private GameObject defeatScreen;
    
    public bool CanStart { get; set; }
    
    #endregion
    
    #region Methods
    
    #region Unity Methods
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        //victoryText.gameObject.SetActive(true);
        victoryScreen.SetActive(true);
        StartCoroutine(LoadMainMenu());
    }
    
    public void EnemyWin()
    {
        victoryText.text = "Défaite !";
        //victoryText.gameObject.SetActive(true);
        defeatScreen.SetActive(true);
        StartCoroutine(LoadMainMenu());
    }
    
    private IEnumerator LoadMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "ClemScene")
        {
            yield return new WaitForSeconds(3);
            victoryText.gameObject.SetActive(false);
            victoryScreen.SetActive(false);
            defeatScreen.SetActive(false);
            //enemySpawner.linesQueue.Clear();
            SceneManager.LoadScene("MainScene");
        }
    }
    private void DoInitStart()
    {
        // if CurrentScene is MainMenu
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            CanStart = false;
            if (!enemySpawner.enabled) return;
            enemySpawner.enabled = false;
            
        }
        
        // if CurrentScene is Game
        if (SceneManager.GetActiveScene().name == "ClemScene")
        {
            if (enemySpawner.enabled) return;
            enemySpawner.enabled = true;
        }
    }
    
    public void Play(int level)
    {
        DeckManager.Instance.UpdateDeckString();
        
        if (level == 1)
        {
            deck = "0404141414";
        }
        if (level == 2)
        {
            deck = "0414090700";
        }
        
        enemySpawner.enemyDataId = level.ToString();
        LevelToLoad = level.ToString();
        SceneManager.LoadScene("ClemScene");
    }
    
    
    #endregion
}
