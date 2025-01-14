using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameContent.MainMenu
{
    public class DeckCardScript : MonoBehaviour
    {
        [SerializeField] public DeckCardScriptable deckCardScriptable;
        
        
        private SpriteRenderer spriteRenderer;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                deckCardScriptable.isInDeck = !deckCardScriptable.isInDeck;
            });
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = deckCardScriptable.cardImage;
        }

        // Grey out the card if it is in the deck
        private void Update()
        {
            if (deckCardScriptable.isInDeck)
            {
                GetComponent<SpriteRenderer>().color = Color.grey;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        
        private void OnButtonPressed()
        {
            deckCardScriptable.isInDeck = !deckCardScriptable.isInDeck;
            
        }
        
        
    }
}