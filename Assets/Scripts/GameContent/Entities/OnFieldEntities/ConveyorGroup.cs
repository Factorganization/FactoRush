namespace GameContent.Entities.OnFieldEntities
{
    public class ConveyorGroup : DynamicBuildingList
    {
        #region properties
        
        public byte ConveyorGroupId { get; set; }
        
        public byte FromStaticBuildId { get; set; }
        
        public byte ToStaticBuildId { get; set; }
        
        #endregion
        
        #region constructor
        
        public ConveyorGroup(params DynamicBuilding[] dl) : base(dl)
        {
        }
        
        #endregion
        
        #region methodes
        
        //TODO move methods for entities on the rails
        
        #endregion
    }
}