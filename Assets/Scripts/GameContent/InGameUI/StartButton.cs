using GameContent.GridManagement;
using UnityEngine;

namespace GameContent.InGameUI
{
    public class StartButton : MonoBehaviour
    {
        #region methodes

        public void SetStartButton()
        {
            if (GridManager.Manager.CurrentLockMode is not GridLockMode.Unlocked)
                return;

            GameManager.Instance.CanStart = true;
        }

        #endregion
    }
}