using GameContent.Entities.UnmanagedEntities.Scriptables;

namespace GameContent.CraftResources
{
    public class RefinedResource : BaseResource
    {
        #region methodes

        public void SetUnitComponent(UnitComponent component) => _unitComponent = component; 

        #endregion
        
        #region fields

        private UnitComponent _unitComponent;

        #endregion
    }
}