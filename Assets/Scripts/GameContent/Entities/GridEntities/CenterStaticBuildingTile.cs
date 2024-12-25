namespace GameContent.Entities.GridEntities
{
    public class CenterStaticBuildingTile : StaticBuildingTile
    {
        #region properties

        public override bool IsBlocked => true;

        public override bool IsSelected
        {
            get;
            set;
        }

        #endregion
    }
}