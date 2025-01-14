using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void CallPlay(int Level)
    {
        GameManager.Instance.Play(Level);
    }
    
}
