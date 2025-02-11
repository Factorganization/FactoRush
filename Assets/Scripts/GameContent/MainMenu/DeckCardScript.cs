using UnityEngine;
using UnityEngine.UI;

namespace GameContent.MainMenu
{
    public class DeckCardScript : MonoBehaviour
    {
        [SerializeField] public DeckCardScriptable deckCardScriptable;
        
        
        private Image spriteRenderer;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                OnButtonPressed();
            });
        }

        private void Start()
        {
            spriteRenderer = GetComponent<Image>();
            spriteRenderer.sprite = deckCardScriptable.cardImage;
            
        }

        // Grey out the card if it is in the deck
        private void Update()
        {
            if (deckCardScriptable.isInDeck)
            {
                spriteRenderer.color = Color.grey;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
        
        private void OnButtonPressed()
        {
            if (DeckManager.Instance.addCard(this))
            {
                deckCardScriptable.isInDeck = !deckCardScriptable.isInDeck;
            }
        }
        
        
    }
}