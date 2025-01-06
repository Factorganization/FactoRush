using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities
{
    public class DynamicBuilding : Building, IEnumerable<sbyte>
    {
        #region properties

        public List<sbyte> ConveyorGroupIds { get; } = new();
        
        #endregion
        
        #region methodes
        
        public void AddConveyorGroupId(sbyte conveyorId) => ConveyorGroupIds.Add(conveyorId);
        
        public void RemoveConveyorGroupId(sbyte conveyorId) => ConveyorGroupIds.Remove(conveyorId);
        
        public void SetDebugId(sbyte i) => conveyorGroupId.text = i.ToString();
        
        public void SetDebugId() => conveyorGroupId.text = "";
        
        public IEnumerator<sbyte> GetEnumerator() => ConveyorGroupIds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion
        
        #region fields
        
        [SerializeField] private TMP_Text conveyorGroupId;
        
        #endregion
    }
}