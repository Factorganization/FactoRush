<<<<<<< HEAD
﻿using System.Collections.Generic;

=======
>>>>>>> 69fc8b8183ef83e7a49619ec02e528d58b14e9d8
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