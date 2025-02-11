using UnityEngine;

namespace GameContent.InGameUI
{
    public class StartButton : MonoBehaviour
    {
        #region methodes

        public void SetStartButton()
        {
            GameManager.Instance.CanStart = true;
            gameObject.SetActive(false);
        }

        #endregion
    }
}