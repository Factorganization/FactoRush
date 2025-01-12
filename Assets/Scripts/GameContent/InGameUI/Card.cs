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
            _offsetPos = _originPos + new Vector3(0, posOffset, posOffset);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position = _selected switch
            {
                true when Vector3.Distance(Position, _offsetPos) > 0.1f
                    => Vector3.MoveTowards(Position, _offsetPos, Time.deltaTime * 10f),
                false when Vector3.Distance(Position, _originPos) > 0.1f 
                    => Vector3.MoveTowards(Position, _originPos, Time.deltaTime * 10f),
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
        
        [SerializeField] private float posOffset;

        private Vector3 _originPos;
        
        private Vector3 _offsetPos;
        
        private bool _selected;

        #endregion
    }
}