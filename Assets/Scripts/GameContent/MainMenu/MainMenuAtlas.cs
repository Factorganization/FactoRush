using GameContent.MainMenu;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAtlas : MonoBehaviour
{

    #region Fields
    
    public static MainMenuAtlas Instance;
    
    [Header("Main Menu Components")]
    [SerializeField] public Button homeButton;
    [SerializeField] public Button cardsButton;
    [SerializeField] public Button shopButton;
    
    [Header("Deck Builder Components")]
    [SerializeField] public Button deckPanelButton;
    [SerializeField] public Button collectionPanelButton;
    [Space] 
    [SerializeField] public DeckCard deckCard1;
    [SerializeField] public DeckCard deckCard2;
    [SerializeField] public DeckCard deckCard3;
    [SerializeField] public DeckCard deckCard4;
    [SerializeField] public DeckCard deckCard5;
    
    [Header("All Cards")]
    [SerializeField] public DeckCardScript[] allCards;
    
    [Header("BeforeLevel Sprite")]
    [SerializeField] public GameObject beforeLevelPanel;
    [SerializeField] public Sprite[] beforeLevelSprite;
    
    

    #endregion
    
    #region Methods
    
    #region Unity Methods
    
    private void Awake()
    {
        Instance = this;
    }
    
    #endregion
    
    #endregion
    
}
