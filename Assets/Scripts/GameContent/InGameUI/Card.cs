using GameContent.Entities;
using GameContent.Entities.OnFieldEntities.Buildings;
using UnityEngine;

namespace GameContent.InGameUI
{
    public class Card : Entity //yes. Entity.
    {
        #region properties

        public FactoryData Data => factoryData;
        
        public bool WasPlaced { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            _originPos = Position;
            _offsetPos = _originPos + new Vector3(0, posOffset.x, posOffset.y);
        }

        protected override void OnUpdate()
        {
            Position = _selected switch
            {
                true when Vector3.Distance(Position, _offsetPos) > 0.01f
                    => Vector3.MoveTowards(Position, _offsetPos, Time.deltaTime * 20f),
                false when Vector3.Distance(Position, _originPos) > 0.01f 
                    => Vector3.MoveTowards(Position, _originPos, Time.deltaTime * 20f),
                _ => Position
            };
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
        }
        
        #endregion
        
        #region fields

        [SerializeField] private FactoryData factoryData;
        
        [SerializeField] private Vector2 posOffset;

        private Vector3 _originPos;
        
        private Vector3 _offsetPos;
        
        private bool _selected;

        #endregion
    }
}