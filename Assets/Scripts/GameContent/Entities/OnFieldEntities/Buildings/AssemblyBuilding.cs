using System.Collections.Generic;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public class AssemblyBuilding : StaticBuilding
    {
        #region properties

        public List<FactoryBuilding> FactoryBuildingsRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            
            FactoryBuildingsRef = new List<FactoryBuilding>();
        }

        #endregion
    }
}