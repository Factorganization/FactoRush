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
            var grid = MapParser.ParseMap(mapPath);
            
            for (var i = grid.Length - 1; i >= 0 ; i--)
            {
                for (var j = 0; j <  grid[i].Length ; j++)
                {
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();//oui c'est legit
                    
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
            if (_currentGridLockMode is GridLockMode.Locked)
                return;

            if (_adding.Count > 0)
            {
                foreach (var b in _toAdd)
                {
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
            
            var b = Instantiate(genericBuild, _grid[index].ETransform);
            _toAdd.Add(index, b);
            b.TargetPosition = pos;
            b.Position = pos;
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
            
            _toRemove.Add(index, _grid[index].CurrentBuildingRef);
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

        [SerializeField] private Building genericBuild;
        
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