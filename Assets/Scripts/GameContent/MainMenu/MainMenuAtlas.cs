using UnityEngine;
using UnityEngine.UI;

public class MainMenuAtlas : MonoBehaviour
{

    #region Fields
    
    [Header("Main Menu Components")]
    [SerializeField] public Button homeButton;
    [SerializeField] public Button cardsButton;
    [SerializeField] public Button shopButton;
    
    [Header("Deck Builder Components")]
    [SerializeField] public Button deckPanelButton;
    [SerializeField] public Button collectionPanelButton;
    [Space] 
    [SerializeField] public Button deckCard1;
    [SerializeField] public Button deckCard2;
    [SerializeField] public Button deckCard3;
    [SerializeField] public Button deckCard4;
    [SerializeField] public Button deckCard5;
    
    

    #endregion
}
