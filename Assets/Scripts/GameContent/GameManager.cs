using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    # region Fields
    
    public static GameManager Instance;
    
    [SerializeField] private GameObject unitPrefab;
    
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
    
    #endregion
}
