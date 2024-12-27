using System;
using System.Collections;
using System.Collections.Generic;

namespace GameContent.Entities.OnFieldEntities
{
    public abstract class DynamicBuildingList : IEnumerable<DynamicBuilding>
    {
        #region properties
        
        public int Count => _dynamicBuildings.Count;
        
        public DynamicBuilding this[int index]
        {
            get
            {
                if (index < 0 || index >= _dynamicBuildings.Count)
                    throw new IndexOutOfRangeException();

                return _dynamicBuildings[index];
            }
            set => _dynamicBuildings[index] = value;
        }

        public List<DynamicBuilding> DynamicBuildings => _dynamicBuildings;

        #endregion

        #region constructors

        protected DynamicBuildingList(params DynamicBuilding[] dl)
        {
            _dynamicBuildings = new List<DynamicBuilding>(dl);
        }

        #endregion

        #region methodes

        public virtual void AddBuild(DynamicBuilding building)
        {
            _dynamicBuildings.Add(building);
        }

        public abstract void UpdateGroup();
        
        public IEnumerator<DynamicBuilding> GetEnumerator() => _dynamicBuildings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region fields
        
        private readonly List<DynamicBuilding> _dynamicBuildings;
        
        #endregion
    }
}