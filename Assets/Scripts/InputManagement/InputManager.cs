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
    /// <summary>
    /// welcome in the absolute horror of sequence input management
    /// </summary>
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
            _startingIndex = t.Index;
            _currentIndex = t.Index;
            
            switch (_startingHitType)
            {
                case HitGridType.SideStaticHit:
                    var sst = t as StaticBuildingTile;
                    //checker
                    _currentStaticGroup = sst!.StaticGroup;
                    break;
                
                case HitGridType.CenterStaticHit:
                    var cst = t as StaticBuildingTile;
                    _currentStaticGroup = cst!.StaticGroup;
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
                    HandleFromCenterStaticMove(ray);
                    break;
                        
                case HitGridType.SideStaticHit:
                    HandleFromSideStaticMove(ray);
                    break;
                        
                case HitGridType.DynamicHit:
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
                    return; // yes. return.
                
                case DynamicBuilding:
                    GridManager.Manager.TryRemoveDynamicBuildingAt(t.Index);
                    break;
                
                case StaticBuilding:
                    GridManager.Manager.TryRemoveStaticBuildingAt(t.Index);
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
                    GridManager.Manager.CancelPrePath();
                    break;
                
                case HitGridType.CenterStaticHit:
                case HitGridType.DynamicHit:
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
                    _currentIndex = t.Index;
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.PrePathFind(_startingIndex, _currentIndex);
                    }
                    break;
                
                case TileType.SideStaticTile:
                    _currentIndex = t.Index;
                    GridManager.Manager.PrePathFind(_startingIndex, t.Index); //Path find before check to properly delete paths 
                    if (t.IsBlocked && t.Index == _startingIndex)
                    {
                        GridManager.Manager.CancelPrePath();
                        _startingHitType = HitGridType.None; 
                    }
                    break;
                    
                case TileType.CenterStaticTile:
                case TileType.Default:
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleFromCenterStaticMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.SideStaticTile:
                    var sst = t as StaticBuildingTile;
                    if (sst!.IsBlocked || sst!.StaticGroup != _currentStaticGroup)
                    {
                        _startingHitType = HitGridType.None;
                    }
                    else if (sst!.StaticGroup == _currentStaticGroup)
                    {
                        _startingHitType = HitGridType.SideStaticHit;
                        _startingIndex = sst.Index;
                    }
                    break;
                
                case TileType.CenterStaticTile:
                case TileType.DynamicTile:
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                case TileType.Default:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleSideStaticEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return; //TODO find a way to cancel drag

            switch (t.Type)
            {
                case TileType.CenterStaticTile:
                    var cst = t as StaticBuildingTile;
                    if (GridManager.Manager.Grid[_currentIndex] is SideStaticBuildingTile && 
                        _currentStaticGroup != cst!.StaticGroup)
                    {
                        GridManager.Manager.SetPath();
                    }
                    break;
                
                case TileType.DynamicTile:
                case TileType.Default:
                    GridManager.Manager.CancelPrePath();
                    break;
                                
                case TileType.SideStaticTile:
                    if (t is not StaticBuildingTile st2)
                        return;

                    if (_currentStaticGroup == st2.StaticGroup)
                    {
                        GridManager.Manager.CancelPrePath();
                    }
                    else
                    {
                        GridManager.Manager.SetPath();
                    }
                    break;

                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    //TODO
                    //faire des trucs
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
                        GridManager.Manager.TryAddStaticBuildingAt(st.Index, st.Position + Vector3.up / 2);
                    //TODO open build selection panel
                    
                    break;

                case TileType.DynamicTile:
                case TileType.SideStaticTile:
                case TileType.Default:
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }
        
        #endregion
        
        private static HitGridType GetHitType(TileType t) => t switch //TODO passer en dll ?
        {
            TileType.DynamicTile => HitGridType.DynamicHit,
            TileType.CenterStaticTile => HitGridType.CenterStaticHit,
            TileType.SideStaticTile => HitGridType.SideStaticHit,
            TileType.Default => HitGridType.None,
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
        
        #endregion
        
        #region fields
        
        [SerializeField] private Camera isoCamera;

        private HitGridType _startingHitType;

        private Vector2Int _startingIndex;

        private Vector2Int _currentIndex;
        
        private bool _hasDragged;

        private byte _currentStaticGroup;
        
        private float _hitTimerCounter;

        private const float StationaryTargetTime = 0.5f;

        #endregion
    }
}
