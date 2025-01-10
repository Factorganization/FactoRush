using GameContent.Entities.OnFieldEntities.Buildings;

namespace GameContent.Entities.GridEntities
{
    public class StaticTileGroup : TileList
    {
        #region properties

        private FactoryBuilding RafineRef
        {
            get
            {
                if (CenterRef?.CurrentBuildingRef is FactoryBuilding rafine)
                    return rafine;
                
                return null;
            }
        }
        
        /// <summary>
        /// /!\ NE PAS CALL AVANT FIN DE GEN DE MAP /!\
        /// </summary>
        private CenterStaticBuildingTile CenterRef => this[2] as CenterStaticBuildingTile;
        
        #endregion
        
        #region constructors

        public StaticTileGroup(params Tile[] tiles) : base(tiles)
        {
        }
        
        #endregion

        #region methodes
        
        public void UpdateGroup()
        {
            
        }

        #endregion
    }
}