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

        public void SetCardData(CardData cardData)
        {
            factoryData = cardData.factoryData;
            spriteRenderer.sprite = cardData.cardSprite;
        }
        
        #endregion
        
        #region fields

        [SerializeField] private FactoryData factoryData;
        
        [SerializeField] private Vector2 posOffset;

        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Vector3 _originPos;
        
        private Vector3 _offsetPos;
        
        private bool _selected;

        #endregion
    }
}

/*
// Transport IDs
00 - transportPropeller
01 - transportThornmail
02 - transportAccumulator
03 - transportDrill
04 - transportInsulatingWheels
05 - transportSlider
06 - transportTwinBoots

// Weapon IDs
07 - weaponArtillery
08 - weaponC4
09 - weaponCanon
10 - weaponMinigun
11 - weaponRailgun
12 - weaponShield
13 - weaponSpear
14 - weaponSpinningBlade
15 - weaponSymbioticRifle
16 - weaponWarhammer
*/
