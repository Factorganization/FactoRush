using System.Collections.Generic;

namespace GameContent.Entities.OnFieldEntities
{
    public abstract class DynamicBuildingList
    {
        #region properties
        
        public List<DynamicBuilding> DynamicBuildings { get; }
        
        public DynamicBuilding this[int index]
        {
            get => DynamicBuildings[index];
            set => DynamicBuildings[index] = value;
        }
        
        #endregion
        
        #region constructors

        protected DynamicBuildingList(params DynamicBuilding[] dl)
        {
            DynamicBuildings = new List<DynamicBuilding>(dl);
        }
        
        #endregion
        
        #region methodes

        public virtual void AddBuild(DynamicBuilding building)
        {
            DynamicBuildings.Add(building);
        }
        
        #endregion
    }
}