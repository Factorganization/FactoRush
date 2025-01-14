using GameContent.Entities.OnFieldEntities.Buildings;
using UnityEngine;

namespace GameContent.InGameUI
{
    public class CardAtlas : MonoBehaviour
    {
        #region properties

        public CardData[] Cards => cardDataAtlas;

        #endregion
        
        #region methodes

        private void Start()
        {
            _cardsIds = CardParser(GameManager.Instance.deck);

            for (var i = 0; i < cards.Length; i++)
            {
                var c = cards[i];
                c.SetCardData(cardDataAtlas[_cardsIds[i]]);
            }
            
            EncyclopediaUI.Encyclopedia.InitPlayerEncyclopedia(this, _cardsIds);
        }

        private static int[] CardParser(string cards)
        {
            var i = new int[5];

            for (var j = 0; j < 5; j++)
            {
                i[j] = int.Parse(cards[(2 * j)..(2 * j + 2)]);
            }
            Debug.Log("Card parser" + cards);
            Debug.Log("Game Manager deck " + GameManager.Instance.deck);
            return i;
        }
        
        #endregion
        
        #region fields

        [SerializeField] private Card[] cards;

        [SerializeField] private CardData[] cardDataAtlas;

        [SerializeField] private string test;
        
        private int[] _cardsIds;

        #endregion
    }

    [System.Serializable]
    public struct CardData
    {
        public FactoryData factoryData;
        
        public Sprite cardSprite;
        
        public Sprite cardDescription;
    }
}