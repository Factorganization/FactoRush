using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] public Sprite[] preLVlSprite;
    [SerializeField] public GameObject preLvlSprite;

    private int pressedLvl;
    public void CallPlay(int Level)
    {
        preLvlSprite.SetActive(true);
        preLvlSprite.GetComponent<Image>().sprite = preLVlSprite[Level - 3];
        pressedLvl = Level; 
    }

    private void Awake()
    {
        preLvlSprite.GetComponent<Button>().onClick.AddListener(() =>
        {
            CallPlayForReal(pressedLvl);
        });
        
        preLvlSprite.GetComponentInChildren<Button>().onClick.AddListener(Back);


    }

    public void CallPlayForReal(int Level)
    {
        GameManager.Instance.Play(Level);
    }

    public void Back()
    {
        preLvlSprite.SetActive(false);
    }
    
    
    
}
