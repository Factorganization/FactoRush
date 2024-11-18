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
        
        #region unity events
        
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

        #endregion
        
        private void HandleTouch()
        {
            if (GridManager.Manager.CurrentLockMode is GridLockMode.Locked)
                return;
            
            var touch = Touch.activeTouches[0]; 
            
            var ray = isoCamera.ScreenPointToRay(touch.screenPosition);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    GridManager.Manager.CurrentLockMode = GridLockMode.BuildingLocked; //Lock grid to prevent abusive list update
                    HandleTouchBegan(ray);
                    break;
                
                case TouchPhase.Moved:
                    _hasDragged = true;
                    _hitTimerCounter = 0;
                    
                    HandleTouchMoved(ray);
                    break;
                
                case TouchPhase.Ended:
                    _hasDragged = false;
                    
                    HandleTouchEnded(ray);
                    GridManager.Manager.CurrentLockMode = GridLockMode.Unlocked; //Unlock grid
                    break;
                
                case TouchPhase.Stationary when !_hasDragged:
                    HandleTouchStationary(ray);
                    break;    
                
                case TouchPhase.Stationary:
                case TouchPhase.None:
                    break;    
                
                case TouchPhase.Canceled: //TODO soft cancel current actions
                    HandleTouchCancelled();
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
                case HitGridType.SideStaticHit:
                    if (t is StaticBuildingTile st1)
                        _currentStaticGroup = st1.StaticGroup;
                    break;
                
                case HitGridType.CenterStaticHit:
                    //prepare to open build selection panel : feedback
                    if (t is StaticBuildingTile st2)
                        _currentStaticGroup = st2.StaticGroup;
                    break;
                            
                case HitGridType.None:
                case HitGridType.DynamicHit:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
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
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
        }

        private void HandleTouchStationary(Ray ray)
        {
            if (_startingHitType is HitGridType.None)
                return;
            
            _hitTimerCounter += Time.deltaTime;
            
            if (_hitTimerCounter < StationaryTargetTime)
                return;
            
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;
            
            switch (t.CurrentBuildingRef)
            {
                case null:
                    break;
                
                case DynamicBuilding:
                    GridManager.Manager.TryRemoveBuildingAt(BuildingType.Conveyor, t.Index);
                    //TODO delete full conveyor
                    break;
                
                case StaticBuilding:
                    GridManager.Manager.TryRemoveBuildingAt(BuildingType.StaticBuild, t.Index);
                    //TODO delete building
                    break;
            }

            _hitTimerCounter = 0;
            _startingHitType = HitGridType.None; //force the switch to No input to prevent new inputs 
            
            GridManager.Manager.CurrentLockMode = GridLockMode.Unlocked;
        }
        
        private void HandleTouchEnded(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
            {
                HandleTouchCancelled();
                
                return;
            }
            
            switch (_startingHitType)
            {
                case HitGridType.SideStaticHit:
                    HandleSideStaticEnding(hit);
                    break;
                
                case HitGridType.CenterStaticHit:
                    HandleCenterStaticEnding(hit);
                    break;
                
                case HitGridType.DynamicHit:
                case HitGridType.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
                        
            _startingHitType = HitGridType.None;
            _currentStaticGroup = 0;
            _hitTimerCounter = 0;
        }

        /// <summary>
        /// can be used in specific cases, if touch raycast is not hitting any part of the grid
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void HandleTouchCancelled()
        {
            switch (_startingHitType)
            {
                case HitGridType.SideStaticHit:
                    GridManager.Manager.CancelAdding();
                    break;
                
                case HitGridType.CenterStaticHit:
                    break;
                
                case HitGridType.DynamicHit:
                    break;
                
                case HitGridType.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
        }
        
        #endregion
        
        #region touch handling events
        
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
                
                case TileType.SideStaticTile:
                    /*if (t.CurrentBuildingRef is not null)
                        _startingHitType = HitGridType.None;*/
                    
                    GridManager.Manager.TryAddBuildingAt(BuildingType.Conveyor, t.Index, t.Position + Vector3.up / 2); //can try pose build if hit dynamic tile
                    // other action if lag and skip dynamic to check if dist between two dynamic is ok and complete if not
                    //TODO path find to complete conveyor path if skip dynamic tile
                    break;
                    
                case TileType.CenterStaticTile:
                case TileType.Default:
                case TileType.CornerStaticTile:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }
        
        private void HandleSideStaticEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return; //TODO find a way to cancel drag

            switch (t.Type)
            {
                case TileType.CornerStaticTile:
                case TileType.CenterStaticTile:
                case TileType.DynamicTile:
                case TileType.Default:
                    GridManager.Manager.CancelAdding();
                    break;
                                
                case TileType.SideStaticTile:
                    if (t is not StaticBuildingTile st2)
                        return;
                    
                    if (_currentStaticGroup == st2.StaticGroup)
                        GridManager.Manager.CancelAdding();
                        //TODO connection logic
                        break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleCenterStaticEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return; //TODO find a way to cancel drag

            switch (t.Type)
            {
                case TileType.CenterStaticTile:
                    if (t is not StaticBuildingTile st)
                        break;
                    
                    if(st.CurrentBuildingRef is not null)
                        break;
                    
                    if (st.StaticGroup == _currentStaticGroup)
                        GridManager.Manager.TryAddBuildingAt(BuildingType.StaticBuild, st.Index, st.Position + Vector3.up / 2);
                    //TODO open build selection panel
                    
                    break;

                case TileType.DynamicTile:
                case TileType.SideStaticTile:
                case TileType.CornerStaticTile:
                case TileType.Default:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
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

        private bool _hasDragged;

        private byte _currentStaticGroup;
        
        private float _hitTimerCounter;

        private const float StationaryTargetTime = 0.5f;

        #endregion
    }
}
