namespace GameContent.Entities.OnFieldEntities
{
    public abstract class DynamicBuildingList
    {
        #region properties
        
        public DynamicBuilding[] DynamicBuildings { get; }
        
        #endregion
        
        #region constructors

        protected DynamicBuildingList(params DynamicBuilding[] dl)
        {
            DynamicBuildings = dl;
        }
        
        #endregion
    }
}