using TMPro;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities
{
    public class DynamicBuilding : Building
    {
        #region properties

        public byte ConveyorGroupId { get; set; }

        #endregion
        
        #region methodes
        
        public void SetDebugId(byte i) => conveyorGroupId.text = i.ToString();
        
        #endregion
        
        #region fields
        
        [SerializeField] private TMP_Text conveyorGroupId;
        
        #endregion
        //TODO dynamicTileGroup class to pack data like what is conveyed
    }
}