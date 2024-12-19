namespace GameContent.Entities.GridEntities
{
    /// <summary>
    /// Odd id key
    /// </summary>
    public sealed class WeaponTargetTile : Tile
    {
        #region properties

        public override bool IsBlocked => true;

        public override bool IsSelected
        {
            get => false;
            set {}
        }

        #endregion

        #region fields

        //TODO queue

        #endregion
    }
}