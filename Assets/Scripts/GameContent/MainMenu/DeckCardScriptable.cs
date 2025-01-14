using UnityEngine;

namespace GameContent.MainMenu
{
    [CreateAssetMenu(fileName = "DeckCardScriptable", menuName = "DeckCardScriptable")]
    public class DeckCardScriptable : ScriptableObject
    {
        [SerializeField] public string cardId;
        [SerializeField] public Sprite cardImage;
        public bool isInDeck;
    }
}