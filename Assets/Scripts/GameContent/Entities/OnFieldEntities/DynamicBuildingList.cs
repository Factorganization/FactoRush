using System;
using System.Collections;
using System.Collections.Generic;

namespace GameContent.Entities.OnFieldEntities
{
    public abstract class DynamicBuildingList : IEnumerable<DynamicBuilding>
    {
        #region properties
        
        public int Count => DynamicBuildings.Count;
        
        public DynamicBuilding this[int index]
        {
            get
            {
                if (index < 0 || index >= DynamicBuildings.Count)
                    throw new IndexOutOfRangeException();

                return DynamicBuildings[index];
            }
            set => DynamicBuildings[index] = value;
        }

        protected List<DynamicBuilding> DynamicBuildings { get; }

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

        public abstract void UpdateGroup();
        
        public IEnumerator<DynamicBuilding> GetEnumerator() => DynamicBuildings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region fields

        #endregion
    }
}