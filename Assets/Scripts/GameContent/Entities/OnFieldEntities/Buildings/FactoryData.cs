﻿using GameContent.Entities.UnmanagedEntities.Scriptables;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    [CreateAssetMenu(menuName = "FactoryData", fileName = "FactoryData")]
    public class FactoryData : ScriptableObject
    {
        [Header("0 : Transport / 1 : Weapon")]
        [Range(0, 1)]
        public int partType;
        
        public UnitComponent component;
        
        public Recipe recipe;

        public Sprite sprite;
    }

    [System.Serializable]
    public struct Recipe
    {
        public int iron;
        
        public int copper;
        
        public int gold;
    }
}