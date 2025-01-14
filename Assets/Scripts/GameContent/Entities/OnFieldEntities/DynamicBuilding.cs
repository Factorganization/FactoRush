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

        protected override void OnStart()
        {
            base.OnStart();
            
            _removing = new HashSet<sbyte>();
        }

        public static float GetRotation(Vector2Int relativePos) => (relativePos.x, relativePos.y) switch
        {
            (1, 0) => -90f,
            (-1, 0) => 90f,
            (0, 1) => 180f,
            (0, -1) => 0f,
            _ => 0
        };

        public void UpdateBuild()
        {
            foreach (var i in _removing)
            {
                ConveyorGroupIds.Remove(i);
            }
            _removing.Clear();
        }
        
        public void AddConveyorGroupId(sbyte conveyorId) => ConveyorGroupIds.Add(conveyorId);
        
        public void RemoveConveyorGroupId(sbyte conveyorId) => _removing.Add(conveyorId);
        
        public void SetDebugId(sbyte i) => conveyorGroupId.text += i.ToString();
        
        public void SetDebugId() => conveyorGroupId.text = "";
        
        public IEnumerator<sbyte> GetEnumerator() => ConveyorGroupIds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion
        
        #region fields
        
        [SerializeField] private TMP_Text conveyorGroupId;

        private HashSet<sbyte> _removing = new();

        #endregion
    }
}