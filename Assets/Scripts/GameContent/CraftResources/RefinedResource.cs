using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using GameContent.Entities.UnmanagedEntities.Scriptables;

namespace GameContent.CraftResources
{
    public class RefinedResource : BaseResource
    {
        #region properties
        
        private UnitAssemblyTile UnitAssemblyTile { get; set; }

        #endregion
        
        #region methodes

        public void SetUnitComponent(UnitComponent component) => _unitComponent = component;

        public override void Created(ConveyorGroup conveyorRef)
        {
            base.Created(conveyorRef);
            
            UnitAssemblyTile = conveyorRef.ToStaticTile as UnitAssemblyTile;
        }

        protected override void RemoveSelf()
        {
            UnitAssemblyTile?.AddResource(_unitComponent);

            base.RemoveSelf();
        }

        #endregion
        
        #region fields

        private UnitComponent _unitComponent;

        #endregion
    }
}