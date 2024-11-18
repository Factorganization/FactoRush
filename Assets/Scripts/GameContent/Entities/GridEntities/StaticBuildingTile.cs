using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public sealed class StaticBuildingTile : Tile
    {
        #region properties
        
        public byte StaticGroup { get; set; }
        
        #endregion
        
        #region methodes

        public void InitStaticTile(byte staticGroup, int rotation)
        {
            StaticGroup = staticGroup;
            graphTransform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
        
        #endregion

        #region fields

        [SerializeField] private Transform graphTransform;

        #endregion
    }
}