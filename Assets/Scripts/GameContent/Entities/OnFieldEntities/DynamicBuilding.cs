using TMPro;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities
{
    public class DynamicBuilding : Building
    {
        #region properties

        public sbyte ConveyorGroupId { get; set; }

        #endregion
        
        #region methodes
        
        public void SetDebugId(sbyte i) => conveyorGroupId.text = i.ToString();
        
        public void SetDebugId() => conveyorGroupId.text = "";
        
        #endregion
        
        #region fields
        
        [SerializeField] private TMP_Text conveyorGroupId;
        
        #endregion
    }
}