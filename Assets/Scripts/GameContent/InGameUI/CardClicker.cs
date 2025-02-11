using UnityEngine;

namespace GameContent.InGameUI
{
    public class CardClicker : MonoBehaviour
    {
        #region properties

        public int DescIndex { get; set; }

        public EncyclopediaUI Encyclopedia { get; set; }
        
        #endregion
        
        #region methodes

        public void OnClick()
        {
            Encyclopedia.CardDesc[DescIndex].SetActive(true);
        }

        #endregion
    }
}