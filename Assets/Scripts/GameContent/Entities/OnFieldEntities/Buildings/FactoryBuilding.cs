using System.Collections.Generic;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public class FactoryBuilding : StaticBuilding
    {
        #region properties

        public List<MineBuilding> MineBuildingRef { get; set; }
        
        public List<AssemblyBuilding> AssemblyBuildingRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            
            MineBuildingRef = new List<MineBuilding>();
            AssemblyBuildingRef = new List<AssemblyBuilding>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        #endregion
        
        #region fields

        

        #endregion
    }
}