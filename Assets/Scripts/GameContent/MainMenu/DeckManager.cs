using System;
using UnityEngine;

namespace GameContent.MainMenu
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            setAllCardsToNull();
            // At the start of the game, circle trough all the cards and if they are in the deck, AddCard them to the deck
            foreach (var card in MainMenuAtlas.Instance.allCards)
            {
                if (card.deckCardScriptable.isInDeck)
                {
                    addCard(card);
                }
            }
            UpdateDeckString();
        }
        

        public void setAllCardsToNull()
        {
            MainMenuAtlas.Instance.deckCard1.card = null;
            MainMenuAtlas.Instance.deckCard2.card = null;
            MainMenuAtlas.Instance.deckCard3.card = null;
            MainMenuAtlas.Instance.deckCard4.card = null;
            MainMenuAtlas.Instance.deckCard5.card = null;
        }

        public bool addCard(DeckCardScript card)
        {
            // Check which card slot are available in the deck
            if (MainMenuAtlas.Instance.deckCard1.card == null)
            {
                MainMenuAtlas.Instance.deckCard1.card = card;
                UpdateDeckString();
                return true;
            }
            if (MainMenuAtlas.Instance.deckCard2.card == null)
            {
                MainMenuAtlas.Instance.deckCard2.card = card;
                UpdateDeckString();
                return true;
            }
            if (MainMenuAtlas.Instance.deckCard3.card == null)
            {
                MainMenuAtlas.Instance.deckCard3.card = card;
                UpdateDeckString();
                return true;
            }
            if (MainMenuAtlas.Instance.deckCard4.card == null)
            {
                MainMenuAtlas.Instance.deckCard4.card = card;
                UpdateDeckString();
                return true;
            }
            if (MainMenuAtlas.Instance.deckCard5.card == null)
            {
                MainMenuAtlas.Instance.deckCard5.card = card;
                UpdateDeckString();
                return true;
            }
            
            return false;
            
        }

        public void UpdateDeckString()
        {
            // Update the deck string on GameManager by concatenating the cardId of each card in the deck 
            if (MainMenuAtlas.Instance.deckCard1.card != null && MainMenuAtlas.Instance.deckCard2.card != null && MainMenuAtlas.Instance.deckCard3.card != null && MainMenuAtlas.Instance.deckCard4.card != null && MainMenuAtlas.Instance.deckCard5.card != null)
            {
                GameManager.Instance.deck = MainMenuAtlas.Instance.deckCard1.cardId + MainMenuAtlas.Instance.deckCard2.cardId + MainMenuAtlas.Instance.deckCard3.cardId + MainMenuAtlas.Instance.deckCard4.cardId + MainMenuAtlas.Instance.deckCard5.cardId;

            }
            else
            {
                GameManager.Instance.deck = "";
            }
            
        }
    }
}