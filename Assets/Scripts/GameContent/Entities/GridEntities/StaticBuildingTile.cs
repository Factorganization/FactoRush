using System;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public abstract class StaticBuildingTile : Tile
    {
        #region properties
        
        public byte StaticGroup { get; private set; }
        
        #endregion
        
        #region methodes

        public void InitStaticTile(byte staticGroup, int rotation)
        {
            StaticGroup = staticGroup;
            graphTransform.localRotation = Quaternion.Euler(0, rotation, 0);
        }

        public static int GetStaticRotation(int n) => n switch
        {
            1 or 3 => 0, // Call me Houdini
            2 => 90,
            4 => -90,
            5 => 180,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
        };
        
        public static TileType GetStaticType(int n) => n switch
        {
            1 or 2 or 4 or 5 => TileType.SideStaticTile, // magical numbers everywhere
            3 => TileType.CenterStaticTile,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
        };
        
        #endregion
    }
}