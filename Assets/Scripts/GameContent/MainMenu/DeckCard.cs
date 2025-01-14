using UnityEngine;
using UnityEngine.UI;

namespace GameContent.MainMenu
{
    public class DeckCard : MonoBehaviour
    {
        public string cardId;
        public DeckCardScript card;

        private Button button;
        private Image spriteRenderer;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<Image>();
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (card is not null)
                {
                    card.deckCardScriptable.isInDeck = false;
                    card = null;
                    spriteRenderer.color = Color.grey;
                    spriteRenderer.sprite = null;
                }
            });
            
            if (card is not null)
            {
                spriteRenderer.color = Color.white;
                spriteRenderer.sprite = card.deckCardScriptable.cardImage;
                cardId = card.deckCardScriptable.cardId;
            }
            else
            {
                spriteRenderer.color = Color.grey;
                spriteRenderer.sprite = null;
                cardId = "";
            }
        }

        private void Update()
        {
            // if card is not null, set the sprite and color
            if (card is not null)
            {
                if (spriteRenderer.color == Color.white) return;
                
                spriteRenderer.sprite = card.deckCardScriptable.cardImage;
                spriteRenderer.color = Color.white;
                cardId = card.deckCardScriptable.cardId;
            }
        }
    }
}