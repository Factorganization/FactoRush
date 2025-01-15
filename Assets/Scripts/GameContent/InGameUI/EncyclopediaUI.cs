using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameContent.InGameUI
{
    public class EncyclopediaUI : MonoBehaviour
    {
        #region properties

        public static EncyclopediaUI Encyclopedia { get; private set; }
        
        public bool IsOpen { get; private set; }

        public GameObject[] CardDesc => cardDescPanelUI;
            
        #endregion
        
        #region methodes

        private void Awake()
        {
            Encyclopedia = this;
            
            _transportIds = new HashSet<int>();
            _weaponIds = new HashSet<int>();
        }
        
        public void SetEncyclopedia()
        {
            IsOpen = !IsOpen;

            if (!IsOpen)
            {
                preselectUI.SetActive(false);
                playerPanelUI.SetActive(false);
                enemyPanelUI.SetActive(false);
                foreach (var c in cardDescPanelUI)
                {
                    c.SetActive(false);
                }
            }
            else
            {
                preselectUI.SetActive(true);
            }
        }

        public void SetPlayerPanel(bool open)
        {
            playerPanelUI.SetActive(open);
            preselectUI.SetActive(!open);
        }

        public void SetEnemyPanel(bool open)
        {
            enemyPanelUI.SetActive(open);
            preselectUI.SetActive(!open);
        }

        public void CardBack()
        {
            foreach (var c in cardDescPanelUI)
            {
                c.SetActive(false);
            }
        }
        
        public void InitPlayerEncyclopedia(CardAtlas atlas, int[] indexes)
        {
            for (var i = 0; i < indexes.Length; i++)
            {
                playerButtons[i].sprite = atlas.Cards[indexes[i]].cardSprite;
                if (playerButtons[i].TryGetComponent(out CardClicker c))
                {
                    c.DescIndex = indexes[i];
                    c.Encyclopedia = this;
                }
            }
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
                        if (b.TryGetComponent(out Image img))
                        {
                            img.sprite = CardAtlas.Instance.Cards[GetTransportId(t)].cardSprite;
                        }
                        if (b.TryGetComponent(out CardClicker cc))
                        {
                            cc.DescIndex = GetTransportId(t);
                            cc.Encyclopedia = this;
                        }
                        
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
                        if (b.TryGetComponent(out Image img))
                        {
                            img.sprite = CardAtlas.Instance.Cards[GetWeaponId(w)].cardSprite;
                        }
                        if (b.TryGetComponent(out CardClicker cc))
                        {
                            cc.DescIndex = GetWeaponId(w);
                            cc.Encyclopedia = this;
                        }
                        
                        _ePartCount++;
                    }
                }
            }
        }

        private int GetTransportId(int i) => i switch
        {
            2 => 0,
            3 => 1,
            4 => 2,
            5 => 3,
            6 => 4,
            7 => 5,
            8 => 6,
            _ => 0
        };
        
        private int GetWeaponId(int i) => i switch
        {
            2 => 7,
            3 => 8,
            4 => 9,
            5 => 10,
            6 => 11,
            7 => 12,
            8 => 13,
            9 => 14,
            10 => 15,
            11 => 16,
            _ => 0
        };
        
        #endregion
        
        #region fields

        [SerializeField] private GameObject preselectUI;
        
        [SerializeField] private GameObject playerPanelUI;
        
        [SerializeField] private GameObject enemyPanelUI;

        [SerializeField] private RectTransform scrollPanel;
        
        [SerializeField] private GameObject rawPrefab;
        
        [SerializeField] private GameObject buttonPrefab;
        
        [SerializeField] private GameObject[] cardDescPanelUI;

        [SerializeField] private Image[] playerButtons;
        
        private HashSet<int> _transportIds;
        
        private HashSet<int> _weaponIds;

        private Transform _currentEPanel;
        
        private byte _ePartCount;

        #endregion
    }
}