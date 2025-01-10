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

        private void Start()
        {
            _hasDragged = false;
            _needDeletion = false;
            _hitTimerCounter = 0;
            _currentConveyorGroup = -1;
        }
        
        private void Update()
        {
            SetDeleteTimer();
            
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
                    HandleTouchBegan(ray);
                    break;
                
                case TouchPhase.Moved:
                    HandleTouchMoved(ray);
                    break;
                
                case TouchPhase.Ended:
                    HandleTouchEnded(ray);
                    break;
                
                case TouchPhase.Stationary when !_hasDragged:
                    //HandleTouchStationary(ray);
                    break;
                
                case TouchPhase.Stationary:
                case TouchPhase.None:
                    break;
                
                case TouchPhase.Canceled:
                    HandleTouchCancelled();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(touch.phase), touch.phase, null);
            }
        }
        
        #region touch handling

        private void HandleTouchBegan(Ray ray)
        {
            GridManager.Manager.CurrentLockMode = GridLockMode.BuildingLocked; //Lock grid to prevent abusive list update
            
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t)) //start drag if hit side of static
                return;
            
            _startingHitType = GetHitType(t.Type);
            _startingIndex = t.Index;
            _currentIndex = t.Index;
            _lastIndex = t.Index;
            
            switch (_startingHitType)
            {
                case HitGridType.SideStaticHit:
                    var sst = t as StaticBuildingTile;
                    //checker
                    _currentStaticGroup = sst!.StaticGroup;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    break;
                
                case HitGridType.CenterStaticHit:
                    var cst = t as StaticBuildingTile;
                    _currentStaticGroup = cst!.StaticGroup;
                    
                    if (cst!.CurrentBuildingRef is not null)
                    {
                        if (_needDeletion && _hitTimerCounter >= 0 && _currentStaticGroup == cst.StaticGroup)
                        {
                            GridManager.Manager.TryRemoveStaticBuildingAt(t.Index);
                            _needDeletion = false;
                            _startingHitType = HitGridType.None;
                            break;
                        }

                        if (!_needDeletion)
                        {
                            _hitTimerCounter = Constants.DeleteTimer;
                            _needDeletion = true;
                        }
                    }
                    break;
                
                case HitGridType.DynamicHit:
                    var dt = t as DynamicBuildingTile;
                    if (dt!.CurrentBuildingRef is not null)
                    {
                        var db = dt.CurrentBuildingRef as DynamicBuilding;
                        
                        if (_needDeletion && _hitTimerCounter >= 0 && _currentConveyorGroup == db!.ConveyorGroupIds[0])
                        {
                            GridManager.Manager.TryRemoveDynamicBuildingAt(t.Index);
                            _needDeletion = false;
                            _startingHitType = HitGridType.None;
                            break;
                        }
                        
                        if (!_needDeletion)
                        {
                            _hitTimerCounter = Constants.DeleteTimer;
                            _needDeletion = true;
                        }
                        
                        _currentConveyorGroup = db!.ConveyorGroupIds[0];
                        GridManager.Manager.MarkAddonFlag(_currentConveyorGroup);
                    }
                    break;        
                    
                case HitGridType.MineTile:
                case HitGridType.TransTarget:
                case HitGridType.WeaponTarget:
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    break;
                
                case HitGridType.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleTouchMoved(Ray ray)
        {
            _hasDragged = true;
            
            switch (_startingHitType)
            {
                case HitGridType.CenterStaticHit:
                    HandleFromCenterStaticMove(ray);
                    break;
                        
                case HitGridType.SideStaticHit:
                    HandleFromSideStaticMove(ray);
                    break;

                case HitGridType.MineTile:
                    HandleFromMineMove(ray);
                    break;
                
                case HitGridType.TransTarget:
                    HandleFromTransMove(ray);
                    break;
                
                case HitGridType.WeaponTarget:
                    HandleFromWeaponMove(ray);
                    break;
                
                case HitGridType.DynamicHit:
                    HandleFromDynamicMove(ray);
                    break;
                
                case HitGridType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
        }

        [Obsolete]
        private void HandleTouchStationary(Ray ray)
        {
            if (_startingHitType is HitGridType.None)
                return;
            
            //_hitTimerCounter += Time.deltaTime;
            
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

            //_hitTimerCounter = 0;
            _startingHitType = HitGridType.None; //force the switch to No input to prevent new inputs 
            
            GridManager.Manager.CurrentLockMode = GridLockMode.Unlocked;
        }
        
        private void HandleTouchEnded(Ray ray)
        {
            _hasDragged = true;
            
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

                case HitGridType.MineTile:
                    HandleMineEnding(hit);
                    break;
                
                case HitGridType.TransTarget:
                    HandleTransEnding(hit);
                    break;
                
                case HitGridType.WeaponTarget:
                    HandleWeaponEnding(hit);
                    break;
                
                case HitGridType.DynamicHit:
                    HandleDynamicEnding(hit);
                    break;
                    
                case HitGridType.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
            
            GridManager.Manager.CurrentLockMode = GridLockMode.Unlocked; //Unlock grid
            
            GridManager.Manager.CancelAdding();
            
            _startingHitType = HitGridType.None;
            _currentStaticGroup = 0;
        }

        /// <summary>
        /// can be used in specific cases, if touch raycast is not hitting any part of the grid
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void HandleTouchCancelled()
        {
            switch (_startingHitType)
            {
                case HitGridType.MineTile:
                case HitGridType.DynamicHit:
                case HitGridType.TransTarget:
                case HitGridType.WeaponTarget:
                case HitGridType.SideStaticHit:
                    GridManager.Manager.CancelAdding();
                    break;
                
                case HitGridType.CenterStaticHit:
                case HitGridType.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_startingHitType), _startingHitType, null);
            }
        }
        
        #endregion
        
        #region touch handling events
        
        #region Handle Moves

        private void HandleFromDynamicMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.DynamicTile:
                    if (t.Index == _currentIndex)
                    {
                        if (t.Index == _startingIndex)
                            GridManager.Manager.SetSelected(_startingIndex, true);
                        break;
                    }

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }

                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                    
                case TileType.SideStaticTile:
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.CenterStaticTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex); // AAAAAAAAAAAAAAAAAAAAAAAAA
                    break;
                    
                case TileType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleFromSideStaticMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;
            
            switch (t.Type)
            {
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                case TileType.DynamicTile:
                case TileType.MineTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }

                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.SideStaticTile:
                    if (t.IsBlocked && t.Index == _startingIndex)
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                        break;
                    }
                    
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);

                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                    
                case TileType.CenterStaticTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex); // Dark Magic of Chaos
                    break;
                
                case TileType.Default:
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

        private void HandleFromMineMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.TransTarget:
                case TileType.WeaponTarget:
                case TileType.DynamicTile:
                case TileType.SideStaticTile:
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.MineTile:
                    if (t.IsBlocked && t.Index == _startingIndex)
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                        break;
                    }
                    
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;

                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.CenterStaticTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex); // Dark Magic of Chaos again
                    break;
                
                case TileType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleFromWeaponMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.MineTile:
                case TileType.TransTarget:
                case TileType.DynamicTile:
                case TileType.SideStaticTile:
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.WeaponTarget:
                    if (t.IsBlocked && t.Index == _startingIndex)
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                        break;
                    }
                    
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;

                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.CenterStaticTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex); // Dark Magic of Chaos again
                    break;
                
                case TileType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleFromTransMove(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Tile")))
                return;
            
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.MineTile:
                case TileType.DynamicTile:
                case TileType.WeaponTarget:
                case TileType.SideStaticTile:
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    
                    if (_currentIndex != _startingIndex)
                    {
                        GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    }
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.TransTarget:
                    if (t.IsBlocked && t.Index == _startingIndex)
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                        break;
                    }
                    
                    if (t.Index == _currentIndex)
                        break;
                    
                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;

                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex);
                    
                    if (GridManager.Manager.IsEmptyPath())
                    {
                        GridManager.Manager.CancelAdding();
                        _startingHitType = HitGridType.None;
                    }
                    break;
                
                case TileType.CenterStaticTile:
                    if (t.Index == _currentIndex)
                        break;

                    _lastIndex = _currentIndex;
                    _currentIndex = t.Index;
                    GridManager.Manager.TryAddDynamicBuildingAt(_startingIndex, _lastIndex, _currentIndex); // Dark Magic of Chaos again
                    break;
                
                case TileType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion
        
        #region Handle Endings
        
        // /!\ /!\ /!\ Attention aux noms des fonctions dans cette section, les noms des tiles sont des refs au noms de la tile de départ du déplacement, pas la tile d'arrivée !!!
        // Ex : pour un drag and drop d'une tile side static à une mine, la fonction sera HandleSideStaticEnding

        private void HandleDynamicEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.SideStaticTile:
                case TileType.DynamicTile:
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex) || t.IsBlocked)
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;
                
                case TileType.CenterStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex))
                    {
                        //GridManager.Manager.CancelAdding();
                    }
                    break;
                
                case TileType.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleSideStaticEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.CenterStaticTile: 
                case TileType.SideStaticTile:
                case TileType.Default:
                    GridManager.Manager.CancelAdding();
                    break;
                    
                case TileType.MineTile: //Apparemment ca marche ¯\_(ツ)_/¯
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                    if (/*!GridManager.Manager.IsLastSelectedTile(_currentIndex) || */ t.IsBlocked)
                    {
                        GridManager.Manager.CancelAdding();
                        break;
                    }
                    GridManager.Manager.SetSelected(_currentIndex, true);
                    break;

                case TileType.DynamicTile:
                    GridManager.Manager.CancelAdding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleCenterStaticEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.CenterStaticTile:
                    if (t is not StaticBuildingTile st)
                        break;
                    
                    if(st.CurrentBuildingRef is not null)
                        break;
                    
                    if (st.StaticGroup == _currentStaticGroup)
                        GridManager.Manager.TryAddStaticBuildingAt(st.Index, st.Position + Vector3.up / 2, _currentStaticGroup);
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

        private void HandleMineEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;

            switch (t.Type)
            {
                case TileType.MineTile:
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                case TileType.Default:
                    GridManager.Manager.CancelAdding();
                    break;
                
                case TileType.CenterStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex))
                    {
                        //GridManager.Manager.CancelAdding();
                    }
                    break;
                    
                case TileType.SideStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex) || t.IsBlocked)
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;

                case TileType.DynamicTile:
                    GridManager.Manager.CancelAdding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleWeaponEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;
            
            switch (t.Type)
            {
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                case TileType.MineTile:
                case TileType.Default:
                    GridManager.Manager.CancelAdding();
                    break;
                
                case TileType.CenterStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex))
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;
                
                case TileType.SideStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex) || t.IsBlocked)
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;

                case TileType.DynamicTile:
                    GridManager.Manager.CancelAdding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }

        private void HandleTransEnding(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Tile t))
                return;
            
            switch (t.Type)
            {
                case TileType.WeaponTarget:
                case TileType.TransTarget:
                case TileType.MineTile:
                case TileType.Default:
                    GridManager.Manager.CancelAdding();
                    break;
                
                case TileType.CenterStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex))
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;
                    
                case TileType.SideStaticTile:
                    if (!GridManager.Manager.IsLastSelectedTile(_currentIndex) || t.IsBlocked)
                    {
                        GridManager.Manager.CancelAdding();
                    }
                    break;

                case TileType.DynamicTile:
                    GridManager.Manager.CancelAdding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t.Type), t.Type, null);
            }
        }
        
        #endregion

        private void SetDeleteTimer()
        {
            if (_hitTimerCounter >= 0 && _needDeletion)
                _hitTimerCounter -= Time.deltaTime;
            
            if (_hitTimerCounter < 0 && _needDeletion)
                _needDeletion = false;
        }

        #endregion
        
        private static HitGridType GetHitType(TileType t) => t switch
        {
            TileType.DynamicTile => HitGridType.DynamicHit,
            TileType.CenterStaticTile => HitGridType.CenterStaticHit,
            TileType.SideStaticTile => HitGridType.SideStaticHit,
            TileType.MineTile => HitGridType.MineTile,
            TileType.TransTarget => HitGridType.TransTarget,
            TileType.WeaponTarget => HitGridType.WeaponTarget,
            TileType.Default => HitGridType.None,
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
        
        #endregion
        
        #region fields

        [SerializeField] private Camera isoCamera;

        private HitGridType _startingHitType;

        private Vector2Int _startingIndex;

        private Vector2Int _currentIndex;

        private Vector2Int _lastIndex;

        private bool _hasDragged;

        private byte _currentStaticGroup;

        private sbyte _currentConveyorGroup;

        private float _hitTimerCounter;

        private bool _needDeletion;

        private const float StationaryTargetTime = 0.5f;

        #endregion
    }
}
