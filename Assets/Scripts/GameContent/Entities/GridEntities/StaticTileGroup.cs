namespace GameContent.Entities.GridEntities
{
    public class StaticTileGroup : TileList
    {
        #region properties

        //TODO idk if it needs refs to then center Tile id as Vector2Int maybe idk

        #endregion
        
        #region constructors

        public StaticTileGroup(params Tile[] tiles) : base(tiles)
        {
        }

        #endregion
    }
}