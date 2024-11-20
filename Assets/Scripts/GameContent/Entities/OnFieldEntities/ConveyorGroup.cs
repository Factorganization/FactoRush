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

        public override void UpdateGroup()
        {
            
        }

        private void OnMove()
        {
            
        }

        private void SwitchWayPoint()
        {
            
        }
        
        #endregion
    }
}