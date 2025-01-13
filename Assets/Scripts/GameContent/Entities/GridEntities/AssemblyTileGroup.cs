namespace GameContent.Entities.GridEntities
{
    public sealed class AssemblyTileGroup : TileList
    {
        #region properties

        public WeaponTargetTile WeaponTile { get; private set; }
        
        public TransTargetTile TransTile { get; private set; }
        
        #endregion
            
        #region constructors

        public AssemblyTileGroup(sbyte assemblyId, params Tile[] tiles) : base(tiles)
        {
        }

        #endregion

        #region methodes
        
        public override void AddTile(Tile tile)
        {
            base.AddTile(tile);

            switch (tile)
            {
                case WeaponTargetTile w:
                    WeaponTile = w;
                    break;
                case TransTargetTile t:
                    TransTile = t;
                    break;
            }
        }

        #endregion
    }
}