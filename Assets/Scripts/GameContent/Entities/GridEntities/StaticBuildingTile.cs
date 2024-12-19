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
        
        #endregion
    }
}