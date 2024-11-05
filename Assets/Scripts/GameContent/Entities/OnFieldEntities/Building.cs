using GameContent.Entities.GridEntities;

namespace GameContent.Entities.OnFieldEntities
{
    public abstract class Building : Entity
    {
        #region properties

        public Tile TileRef { get; set; }

        #endregion
        
        #region methodes

        public void Added(Tile tile) => TileRef = tile;
        
        #endregion
    }
}