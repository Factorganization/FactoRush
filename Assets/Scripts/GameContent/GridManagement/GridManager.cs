using System;
using System.Collections;
using System.Collections.Generic;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.GridManagement
{
    public sealed class GridManager : MonoBehaviour
    {
        #region properties

        public static GridManager Manager { get; private set; }

        public GridLockMode CurrentLockMode
        {
            get => _currentGridLockMode;
            set
            {
                _currentGridLockMode = value;
                UpdateGrid();
            }
        }

        public Dictionary<byte, List<DynamicBuilding>> ConveyorGroups { get; private set; }
        
        #endregion
        
        #region methodes

        #region Unity events

        private void Awake()
        {
            if (Manager is not null)
                throw new Exception("Only one instance of a GridManager can be active at a time.");
            
            Manager = this;
        }
        
        private void Start()
        {
            _grid = new Dictionary<Vector2Int, Tile>();
            _staticGroups = new Dictionary<byte, List<Tile>>();
            ConveyorGroups = new Dictionary<byte, List<DynamicBuilding>>();
            _adding = new HashSet<Vector2Int>();
            _removing = new HashSet<Vector2Int>();
            _toAdd = new Dictionary<Vector2Int, Building>();
            _toRemove = new Dictionary<Vector2Int, Building>();
            
            StartCoroutine(InitGrid());
        }
        
        #endregion
        
        #region grid
        
        private IEnumerator InitGrid()
        {
            CurrentLockMode = GridLockMode.Locked;
            
            var grid = MapParser.ParseMap(mapPath);
            
            for (var i = grid.Length - 1; i >= 0 ; i--)
            {
                for (var j = 0; j <  grid[i].Length ; j++)
                {
                    yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
                    yield return new WaitForEndOfFrame(); //that's legit, dont question it
#endif
                    
                    var tId = new Vector2Int(grid.Length - 1 - i, j);
                    var tPos = new Vector3(j, 0, grid.Length - 1 - i); //inverted i j for pos /!\
                    
                    switch (grid[i][j])
                    {
                        case 0:
                            var dBt = Instantiate(dynamicBuildTile, transform);
                            _grid.Add(tId, dBt);
                            dBt.Added(this, tId, tPos, TileType.DynamicTile);
                            break;
                        
                        case > 0:
                            var sBt = Instantiate(staticBuildTile, transform);
                            _grid.Add(tId, sBt);
                            
                            if (!_staticGroups.ContainsKey(grid[i][j]))
                                _staticGroups.Add(grid[i][j], new List<Tile>());
                            
                            _staticGroups[grid[i][j]].Add(sBt);
                            
                            sBt.Added(this, tId, tPos, GetType(_staticGroups[grid[i][j]].Count));
                            sBt.StaticGroup = grid[i][j];
                            break;
                    }
                }
            }
            
            _currentGridLockMode = GridLockMode.Unlocked;
        }

        private static TileType GetType(int n) => n switch
        {
            1 or 3 or 7 or 9 => TileType.CornerStaticTile, //magiiiiic
            2 or 4 or 6 or 8 => TileType.SideStaticTile,
            5 => TileType.CenterStaticTile,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null) // this is pure magic don't question it
        };
        
        private void UpdateGrid()
        {
            if (_currentGridLockMode is GridLockMode.Locked or GridLockMode.BuildingLocked)
                return;
            
            if (_adding.Count > 0)
            {
                byte i = 0;
    
                while (ConveyorGroups.ContainsKey(i) && ConveyorGroups[i] != null)
                    i++;
                
                if (!ConveyorGroups.ContainsKey(i))
                    ConveyorGroups.Add(i, new List<DynamicBuilding>());
                else
                    ConveyorGroups[i] = new List<DynamicBuilding>();
                
                foreach (var b in _toAdd)
                {
                    switch (b.Value)
                    {
                        case StaticBuilding:
                            ConveyorGroups.Remove(i);
                            break;
                        
                        case DynamicBuilding db:
                            ConveyorGroups[i].Add(db);
                            db.ConveyorGroupId = i;
                            db.SetDebugId(i);
                            break;
                    }
                    PlaceBuildingAt(b.Key, b.Value);
                }
            }
            _adding.Clear();
            _toAdd.Clear();

            if (_removing.Count > 0)
            {
                foreach (var b in _toRemove)
                {
                    RemoveBuildingAt(b.Key, b.Value);
                }
            }
            _removing.Clear();
            _toRemove.Clear();
        }

        public void TryAddBuildingAt(BuildingType type, Vector2Int index, Vector3 pos)
        {
            if (!_adding.Add(index))
                return;

            switch (type)
            {
                case BuildingType.Conveyor:
                    var b1 = Instantiate(dynamicGenericBuild, _grid[index].ETransform);
                    _toAdd.Add(index, b1);
                    b1.TargetPosition = pos;
                    b1.Position = pos;
                    break;
                
                case BuildingType.StaticBuild:
                    var b2 = Instantiate(staticGenericBuild, _grid[index].ETransform);
                    _toAdd.Add(index, b2);
                    b2.TargetPosition = pos;
                    b2.Position = pos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void CancelAdding()
        {
            foreach (var b in _toAdd)
            {
                Destroy(b.Value.gameObject);
            }
            
            _adding.Clear();
            _toAdd.Clear();
        }
        
        public void TryRemoveBuildingAt(BuildingType type, Vector2Int index)
        {
            if (!_removing.Add(index))
                return;

            switch (type)
            {
                case BuildingType.Conveyor:
                    var b = _grid[index].CurrentBuildingRef as DynamicBuilding;
                    var i = b!.ConveyorGroupId;
                    foreach (var b2 in ConveyorGroups[i])
                    {
                        _toRemove.Add(b2.TileRef.Index, b2);
                    }

                    ConveyorGroups.Remove(i);
                    break;
                
                case BuildingType.StaticBuild:
                    _toRemove.Add(index, _grid[index].CurrentBuildingRef);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void PlaceBuildingAt(Vector2Int index, Building building)
        {
            _grid[index].CurrentBuildingRef = building;
            building.Added(_grid[index]);
        }

        private void RemoveBuildingAt(Vector2Int index, Building building)
        {
            _grid[index].CurrentBuildingRef = null;
            Destroy(building.gameObject);
        }

        #endregion

        #endregion

        #region fields

        [SerializeField] private string mapPath;
        
        [SerializeField] private DynamicBuildingTile dynamicBuildTile;
        
        [SerializeField] private StaticBuildingTile staticBuildTile;

        [SerializeField] private DynamicBuilding dynamicGenericBuild;
        
        [SerializeField] private StaticBuilding staticGenericBuild;
        
        private Dictionary<Vector2Int,Tile> _grid;
        
        private Dictionary<byte, List<Tile>> _staticGroups;
        
        private HashSet<Vector2Int> _adding;
        
        private HashSet<Vector2Int> _removing;
        
        private Dictionary<Vector2Int, Building> _toAdd;
        
        private Dictionary<Vector2Int, Building> _toRemove;

        private GridLockMode _currentGridLockMode;
        
        #endregion
    }
}