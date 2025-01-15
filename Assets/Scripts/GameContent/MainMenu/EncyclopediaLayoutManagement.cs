using UnityEngine;

namespace GameContent.MainMenu
{
    public class EncyclopediaLayoutManagement : MonoBehaviour
    {
        #region methodes

        public void OpenPanel(int i)
        {
            vLayout.SetActive(false);
            panels[i].SetActive(true);
        }

        public void ClosePanel()
        {
            foreach (var panel in panels)
            {
                panel.SetActive(false);
            }
            vLayout.SetActive(true);
        }

        #endregion
        
        #region fields

        [SerializeField] private GameObject vLayout;
        
        [SerializeField] private GameObject[] panels;

        #endregion
    }
}