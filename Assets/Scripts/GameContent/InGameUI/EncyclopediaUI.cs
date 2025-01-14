using System.Collections.Generic;
using UnityEngine;

namespace GameContent.InGameUI
{
    public class EncyclopediaUI : MonoBehaviour
    {
        #region properties

        public static EncyclopediaUI Encyclopedia { get; private set; }
        
        public bool IsOpen { get; private set; }

        #endregion
        
        #region methodes

        private void Awake()
        {
            Encyclopedia = this;
            
            _transportIds = new HashSet<int>();
            _weaponIds = new HashSet<int>();
        }

        private void Start()
        {
            InitPlayerEncyclopedia();
        }
        
        public void SetEncyclopedia()
        {
            IsOpen = !IsOpen;

            if (!IsOpen)
            {
                preselectUI.SetActive(false);
                playerPanelUI.SetActive(false);
                enemyPanelUI.SetActive(false);
                weaponCardPanelUI.SetActive(false);
                transportCardPanelUI.SetActive(false);
            }
            else
            {
                preselectUI.SetActive(true);
            }
        }

        public void InitPlayerEncyclopedia()
        {
            
        }
        
        public void InitEnemyEncyclopedia(Queue<string> ids)
        {
            foreach (var i in ids)
            {
                var s = i.Split(',');
                foreach (var j in s)
                {
                    int t;
                    int w;
                    if (j.StartsWith('-'))
                        continue;
                    
                    if (j.StartsWith("/"))
                    {
                        t = int.Parse(j[3..^2]);
                        w = int.Parse(j[5..]);
                    }
                    else
                    {
                        t = int.Parse(j[..2]);
                        w = int.Parse(j[2..]);
                    }

                    if (_transportIds.Add(t))
                    {
                        if (_ePartCount % 3 == 0)
                        {
                            var c = Instantiate(rawPrefab, scrollPanel.transform);
                            scrollPanel.sizeDelta = new Vector2(scrollPanel.sizeDelta.x, scrollPanel.sizeDelta.y + 250);
                            _currentEPanel = c.transform;
                        }
                        var b = Instantiate(buttonPrefab, _currentEPanel.transform);
                        //TODO init button
                        
                        _ePartCount++;
                    }

                    if (_weaponIds.Add(w))
                    {
                        if (_ePartCount % 3 == 0)
                        {
                            var c = Instantiate(rawPrefab, scrollPanel.transform);
                            scrollPanel.sizeDelta = new Vector2(scrollPanel.sizeDelta.x, scrollPanel.sizeDelta.y + 250);
                            _currentEPanel = c.transform;
                        }
                        var b = Instantiate(buttonPrefab, _currentEPanel.transform);
                        //TODO init button
                        
                        _ePartCount++;
                    }
                }
            }
        }
        
        #endregion
        
        #region fields

        [SerializeField] private GameObject preselectUI;
        
        [SerializeField] private GameObject playerPanelUI;
        
        [SerializeField] private GameObject enemyPanelUI;

        [SerializeField] private RectTransform scrollPanel;
        
        [SerializeField] private GameObject rawPrefab;
        
        [SerializeField] private GameObject buttonPrefab;
        
        [SerializeField] private GameObject weaponCardPanelUI;
        
        [SerializeField] private GameObject transportCardPanelUI;
            
        private HashSet<int> _transportIds;
        
        private HashSet<int> _weaponIds;

        private Transform _currentEPanel;
        
        private byte _ePartCount;

        #endregion
    }
}