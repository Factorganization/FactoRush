using UnityEngine;
using System;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using GameContent.GridManagement;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace InputManagement
{
    public class InputManager : MonoBehaviour
    {
        #region methodes
        
        private void Awake()
        {
            EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            if (Touch.activeTouches.Count <= 0)
                return;
            
            HandleTouch();
        }

        private void HandleTouch()
        {
            var touch = Touch.activeTouches[0]; 
            
            var ray = isoCamera.ScreenPointToRay(touch.screenPosition);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GridManager.Manager.CurrentLockMode = GridLockMode.Locked; //Lock grid to prevent abusive list update
                    HandleTouchBegan(ray);
                    break;
                
                case TouchPhase.Moved:
                    HandleTouchMoved(ray);
                    break;
                
                case TouchPhase.Ended:
                    HandleTouchEnded(ray);
                    GridManager.Manager.CurrentLockMode = GridLockMode.Unlocked; //Unlock grid
                    break;
                
                case TouchPhase.None:
                case TouchPhase.Canceled: //TODO soft cancel current actions
                case TouchPhase.Stationary:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(touch.phase), touch.phase, null);
            }
        }
        
        #region touch handling

        private void HandleTouchBegan(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t)) //start drag if hit side of static
                return;
                        
            _startingHitType = GetHitType(t.Type);

            switch (_startingHitType)
            {
                case HitGridType.CenterStaticHit:
                    //prepare to open build selection panel : feedback
                    break;
                            
                case HitGridType.None:
                case HitGridType.DynamicHit:
                case HitGridType.SideStaticHit:
                default:
                    break;
            }
        }

        private void HandleTouchMoved(Ray ray)
        {
            switch (_startingHitType)
            {
                case HitGridType.CenterStaticHit:
                    //action cut if (isOnStatic) move out of static
                    //other action if not on dynamic to keep preview conveyor
                    break;
                        
                case HitGridType.SideStaticHit:
                    HandleFromSideStaticMove(ray);
                    break;
                        
                case HitGridType.DynamicHit:
                    break;
                        
                case HitGridType.None:
                default:
                    break;
            }
        }
        
        private void HandleTouchEnded(Ray ray)
        {
            switch (_startingHitType)
            {
                case HitGridType.SideStaticHit:
                    HandleSideStaticEnding(ray);
                    break;
                
                case HitGridType.CenterStaticHit:
                    HandleCenterStaticEnding(ray);
                    break;
                
                case HitGridType.DynamicHit:
                case HitGridType.None:
                default:
                    break;
            }
                        
            _startingHitType = HitGridType.None;
        }
        
        private void HandleFromSideStaticMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.DynamicTile:
                    GridManager.Manager.TryAddBuildingAt(BuildingType.Conveyor, t.Index, t.Position + Vector3.up / 2); //can try pose build if hit dynamic tile
                    // other action if lag and skip dynamic to check if dist between two dynamic is ok and complete if not
                    //TODO path find to complete conveyor path if skip dynamic tile
                    break;
                
                case TileType.CenterStaticTile:
                case TileType.SideStaticTile:
                case TileType.Default:
                case TileType.CornerStaticTile:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }
        
        private void HandleSideStaticEnding(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return; //TODO find a way to cancel drag

            switch (t.Type)
            {
                case TileType.CornerStaticTile:
                case TileType.CenterStaticTile:
                case TileType.DynamicTile:
                    GridManager.Manager.CancelAdding();
                    break;
                                
                case TileType.SideStaticTile:
                    //TODO connection logic
                    break;
                                
                case TileType.Default:
                default:
                    break;
            }
        }

        private void HandleCenterStaticEnding(Ray ray)
        {
            //TODO open build selection panel
        }
        
        #endregion
        
        private static HitGridType GetHitType(TileType t) => t switch
        {
            TileType.DynamicTile => HitGridType.DynamicHit,
            TileType.CenterStaticTile or TileType.CornerStaticTile => HitGridType.CenterStaticHit,
            TileType.SideStaticTile => HitGridType.SideStaticHit,
            TileType.Default => HitGridType.None,
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
        
        #endregion
        
        #region fields
        
        [SerializeField] private Camera isoCamera;

        private HitGridType _startingHitType;
        
        #endregion
    }
}
