using UnityEngine;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    [CreateAssetMenu(menuName = "FactoryData", fileName = "FactoryData")]
    public class FactoryData : ScriptableObject
    {
        [Header("0 : Transport / 1 : Weapon")]
        [Range(0, 1)]
        public int partType;
        
        //public PartData partData;
        
        public Recipe recipe;
    }

    [System.Serializable]
    public struct Recipe
    {
        [Header("Wood")]
        public int NecessaryMineResource1;
        
        [Header("Cobble")]
        public int NecessaryMineResource2;
        
        [Header("Iron")]
        public int NecessaryMineResource3;
        
        [Header("Gold")]
        public int NecessaryMineResource4;

        [Header("Diamond")]
        public int NecessaryMineResource5;
        
        [Header("Netherite")]
        public int NecessaryMineResource6;
    }
}