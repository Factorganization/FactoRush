using GameContent.Entities.OnFieldEntities.Buildings;
using UnityEngine;

namespace GameContent.InGameUI
{
    public class CardAtlas : MonoBehaviour
    {
        #region properties

        public Card[] Cards => cards;

        #endregion
        
        #region methodes

        private void Start()
        {
            _cardsIds = CardParser(test);

            for (var i = 0; i < cards.Length; i++)
            {
                var c = cards[i];
                c.SetCardData(cardDataAtlas[_cardsIds[i]]);
            }
        }

        private static int[] CardParser(string cards)
        {
            var i = new int[5];

            for (var j = 0; j < 5; j++)
            {
                i[j] = int.Parse(cards[(2 * j)..(2 * j + 2)]);
            }
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
    }
}