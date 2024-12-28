using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.CraftResources
{
    public abstract class BaseResource : MonoBehaviour
    {
        #region properties

        private ConveyorGroup ConveyorRef { get; set; }

        #endregion
        
        #region methodes

        public void Created(ConveyorGroup conveyorRef)
        {
            ConveyorRef = conveyorRef;
            transform.position = ConveyorRef[0].transform.position;
        }

        private void OnMove()
        {
            
        }

        private void SwitchWayPoint()
        {
            
        }

        #endregion
    }
}