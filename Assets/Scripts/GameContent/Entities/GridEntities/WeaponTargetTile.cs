using GameContent.Entities.OnFieldEntities;

namespace GameContent.Entities.GridEntities
{
    /// <summary>
    /// Odd id key
    /// </summary>
    public sealed class WeaponTargetTile : Tile
    {
        #region properties

        public override bool IsBlocked => CurrentBuildingRef is not null;

        public override bool IsSelected
        {
            get;
            set;
        }

        #endregion

        #region fields

        //TODO queue

        #endregion
    }
}