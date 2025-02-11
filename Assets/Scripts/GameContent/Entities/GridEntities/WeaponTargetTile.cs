namespace GameContent.Entities.GridEntities
{
    /// <summary>
    /// used to type comparison
    /// </summary>
    public sealed class WeaponTargetTile : UnitAssemblyTile
    {
        #region methodes

        public override void SetBinTileRef(UnitAssemblyTile unitAssemblyTile)
        {
            BinTileGroupRef = unitAssemblyTile;
        }

        #endregion
    }
}