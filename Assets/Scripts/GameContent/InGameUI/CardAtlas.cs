using UnityEngine;

namespace GameContent.InGameUI
{
    public class CardAtlas : MonoBehaviour
    {
        #region properties

        public Card[] Cards => cards;

        #endregion
        
        #region fields

        [SerializeField] private Card[] cards;

        #endregion
    }
}