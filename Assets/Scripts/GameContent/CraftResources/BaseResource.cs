using System.Collections.Generic;
using GameContent.Entities;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.CraftResources
{
    public abstract class BaseResource : Entity
    {
        #region properties

        private ConveyorGroup ConveyorRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            
            OnMove();
        }

        public virtual void Created(ConveyorGroup conveyorRef)
        {
            ConveyorRef = conveyorRef;
            _targetPos = conveyorRef[1].Position + Constants.UpperCorrectionPos;
            _currentTargetId = 1;
        }

        private void OnMove()
        {
            if (SquareDistance(Position, _targetPos) > 0.01f)
            {
                Position = Vector3.MoveTowards(Position, _targetPos, Constants.ResourceSpeed * Time.deltaTime);
            }
            else
                SwitchWayPoint();
        }

        private void SwitchWayPoint()
        {
            _currentTargetId++;
            if (_currentTargetId == ConveyorRef.Count)
            {
                RemoveSelf();
                return;
            }
            
            _targetPos = ConveyorRef[_currentTargetId].Position + Constants.UpperCorrectionPos;
        }

        protected virtual void RemoveSelf()
        {
            ConveyorRef.RemoveResource(this); 
            Destroy(gameObject);
        }

        #endregion

        #region fields
        
        private Vector3 _targetPos;

        private int _currentTargetId;

        #endregion
    }
}
