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

        public Dictionary<sbyte, ConveyorGroup> ConveyorGroups { get; private set; }

        public Dictionary<Vector2Int, Tile> Grid { get; private set; }

        #endregion
        
        #region methodes

        #region Unity events

        private void Awake()
        {
            if (Manager is not null)
                throw new Exception("Only one instance of a GridManager can be active at a time.");
            
            Manager = this;
        }
        
        [ContextMenu("Start")]
        private void Start()
        {
            _currentPath = new List<Tile>();
            
            Grid = new Dictionary<Vector2Int, Tile>();
            _staticGroups = new Dictionary<byte, StaticTileGroup>();
            ConveyorGroups = new Dictionary<sbyte, ConveyorGroup>();
            
            _addingDynamic = new HashSet<Vector2Int>();
            _pathExceed = new HashSet<Vector2Int>();
            _addingStatic = new HashSet<Vector2Int>();
            _removing = new HashSet<Vector2Int>();
            _toAddStatic = new Dictionary<Vector2Int, StaticBuilding>();
            _toRemove = new Dictionary<Vector2Int, Building>();
            
            StartCoroutine(InitGrid());
        }

        private void Update()
        {
            if (ConveyorGroups.Count <= 0)
                return;
            
            foreach (var group in ConveyorGroups.Values)
            {
                group.UpdateGroup();
            }
        }
        
        #endregion
        
        #region grid
        
        /// <summary>
        /// 01 - 09 : mining build
        /// 10 - 19 : refinery build
        /// 20 - 29 : assembly build
        /// </summary>
        /// <returns></returns>
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
                    
                    var tId = new Vector2Int(j, grid.Length - 1 - i);
                    var tPos = new Vector3(j, 0, grid.Length - 1 - i); //inverted i j for pos /!\
                    
                    switch (grid[i][j])
                    {
                        case 0:
                            var dBt = Instantiate(dynamicBuildTile, transform);
                            Grid.Add(tId, dBt);
                            dBt.Added(this, tId, tPos, TileType.DynamicTile);
                            break;
                        
                        case >= 20:
                            var mT = Instantiate(mineTile, transform);
                            Grid.Add(tId, mT);
                            mT.Added(this, tId, tPos, TileType.MineTile);
                            break;
                        
                        case >= 10 when grid[i][j] % 2 == 0:
                            var tTt = Instantiate(transTargetTile, transform);
                            Grid.Add(tId, tTt);
                            tTt.Added(this, tId, tPos, TileType.TransTarget);
                            break;
                        
                        case >= 10 when grid[i][j] % 2 != 0:
                            var wTt = Instantiate(weaponTargetTile, transform);
                            Grid.Add(tId, wTt);
                            wTt.Added(this, tId, tPos, TileType.WeaponTarget);
                            break;
                        
                        case > 0:
                            if (!_staticGroups.ContainsKey(grid[i][j]))
                                _staticGroups.Add(grid[i][j], new StaticTileGroup());
                            
                            var c = _staticGroups[grid[i][j]].Count + 1;
                            var sBt = Instantiate(c == 3 ? centerStaticBuildTile : sideStaticBuildTile, transform);
                            
                            Grid.Add(tId, sBt);
                            _staticGroups[grid[i][j]].AddTile(sBt);
                            
                            sBt.Added(this, tId, tPos, GetStaticType(c));
                            sBt.InitStaticTile(grid[i][j], GetStaticRotation(c));
                            break;
                    }
                }
            }
            
            _currentGridLockMode = GridLockMode.Unlocked;
        }

        /*[DllImport("libGridGenSL", EntryPoint = "GetStaticRotation")]
        private static extern int GetStaticRotation(int n);
        
        [DllImport("libGridGenSL", EntryPoint = "GetStaticType")]
        private static extern int GetStaticType(int n);

        [DllImport("libGridGenSL", EntryPoint = "GetStaticTypeObsolete")]
        private static extern int GetStaticTypeObsolete(int n);*/
        
        private static int GetStaticRotation(int n) => n switch
        {
            1 or 3 => 0, // Call me Houdini
            2 => 90,
            4 => -90,
            5 => 180,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
        };
        
        private static TileType GetStaticType(int n) => n switch
        {
            1 or 2 or 4 or 5 => TileType.SideStaticTile, // magical numbers everywhere
            3 => TileType.CenterStaticTile,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
        };
        
        private void UpdateGrid()
        {
            if (_currentGridLockMode is GridLockMode.Locked or GridLockMode.BuildingLocked)
                return;
            
            if (_addingDynamic.Count > 0)
            {
                sbyte i = 0;
    
                while (ConveyorGroups.ContainsKey(i) && ConveyorGroups[i] != null)
                    i++;
                
                if (!ConveyorGroups.ContainsKey(i))
                    ConveyorGroups.Add(i, new ConveyorGroup());
                else
                    ConveyorGroups[i] = new ConveyorGroup();
                
                foreach (var id in _addingDynamic)
                {
                    var b = InstantiateBuildingAt(dynamicGenericBuild, Grid[id].ETransform) as DynamicBuilding;
                    ConveyorGroups[i].AddBuild(b);
                    
                    if (b is null)
                        continue;
                    
                    b.ConveyorGroupId = i;
                    b.SetDebugId(i);
                    PlaceBuildingAt(id, b);
                    
                    Grid[id].IsSelected = false;
                }
                
                ConveyorGroups[i].Init();
            }
            _addingDynamic.Clear();

            if (_addingStatic.Count > 0)
            {
                foreach (var b in _toAddStatic)
                {
                    PlaceBuildingAt(b.Key, b.Value);
                }
            }
            _addingStatic.Clear();
            _toAddStatic.Clear();

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
        
        #endregion
        
        #region grid modifiers
        
        public bool TryAddDynamicBuildingAt(Vector2Int from, Vector2Int previous, Vector2Int index) // yes. bool.
        {
            if (Grid[index].CurrentBuildingRef is not null || (previous == index && Grid[index].IsSelected))
                return true;
            
            if (IsSpecTile(_currentSelectedTile) && !Grid[index].IsSelected && _addingDynamic.Count > 1)
                return false;

            if (Grid[index] is CenterStaticBuildingTile) // Dark Magic Happening here
            {
                _currentSelectedTile = Grid[index];
                return false;
            }
            
            _currentSelectedTile = Grid[index];
            
            var distance = Vector2Int.Distance(index, previous);
            switch (distance)
            {
                case > 1 and < 2 when !TryCompletePath(previous, index):
                    CancelAdding();
                    return false;
                case >= 2:
                    return false;
            }

            _addingDynamic.Add(index);
            Grid[index].IsSelected = true;

            _currentPath = PathFinder.FindPath(Grid[from], Grid[index]);

            foreach (var i in _addingDynamic)
            {
                if (_currentPath.Contains(Grid[i]))
                    continue;
                
                Grid[i].IsSelected = false;
                _pathExceed.Add(i);
            }
            
            if (_pathExceed.Count <= 0)
                return true;
            
            foreach (var i in _pathExceed)
            {
                _addingDynamic.Remove(i);
            }
            _pathExceed.Clear();
            
            return _addingDynamic.Count > 0;
        }

        private bool TryCompletePath(Vector2Int previous, Vector2Int index)
        {
            var rawPi = index - previous;
            var pi = Vector2Int.one * new Vector2Int((int)Mathf.Sign(rawPi.x), (int)Mathf.Sign(rawPi.y));

            var c1 = previous + pi * Vector2Int.right;
            var t1 = Grid[c1];
            if (IsAvailable(t1))
            {
                t1.IsSelected = true;
                return _addingDynamic.Add(c1);
            }
                
            var c2 = previous + pi * Vector2Int.up;
            var t2 = Grid[c2];
            if (IsAvailable(t2))
            {
                t2.IsSelected = true;
                return _addingDynamic.Add(c2);
            }

            return false;
        }
        
        public bool TryAddStaticBuildingAt(Vector2Int index, Vector3 pos) // bool again
        {
            if (!_addingStatic.Add(index))
                return false;
            
            var b = Instantiate(staticGenericBuild, Grid[index].ETransform);
            _toAddStatic.Add(index, b);
            b.TargetPosition = pos;
            b.Position = pos;
            return true;
        }
        
        public void CancelAdding()
        {
            _currentSelectedTile = null;
            
            if (_addingDynamic.Count <= 0)
                return;
            
            foreach (var i in _addingDynamic)
            {
                Grid[i].IsSelected = false;
            }
            
            _addingDynamic.Clear();
            _currentPath.Clear();
            _pathExceed.Clear();
        }

        public void TryRemoveDynamicBuildingAt(Vector2Int index)
        {
            if (!_removing.Add(index))
                return;
            
            var b = Grid[index].CurrentBuildingRef as DynamicBuilding;
            var i = b!.ConveyorGroupId;
                    
            foreach (var b2 in ConveyorGroups[i])
            {
                _removing.Add(b2.TileRef.Index);
                _toRemove.Add(b2.TileRef.Index, b2);
            }
            ConveyorGroups.Remove(i);
        }

        public void TryRemoveStaticBuildingAt(Vector2Int index)
        {
            if (!_removing.Add(index))
                return;
            
            _toRemove.Add(index, Grid[index].CurrentBuildingRef);
        }

        #region placings
        
        private static Building InstantiateBuildingAt(Building build, Transform trans)
        {
            var b = Instantiate(build, trans);
            b.TargetPosition = trans.position + Vector3.up / 2;
            b.Position = trans.position + Vector3.up / 2;
            return b;
        }
        
        private void PlaceBuildingAt(Vector2Int index, Building building)
        {
            Grid[index].CurrentBuildingRef = building;
            building.Added(Grid[index]);
        }

        private void RemoveBuildingAt(Vector2Int index, Building building)
        {
            Grid[index].CurrentBuildingRef = null;
            Destroy(building.gameObject);
        }

        #endregion
        
        #endregion

        #region path find
        
        [Obsolete]
        public void SetPath()
        {
            foreach (var t in _currentPath)
            {
                //TryAddDynamicBuildingAt(t.Index);
            }
            
            CancelPrePath();
        }

        [Obsolete]
        public List<Tile> GetPath(Vector2Int from, Vector2Int to) => PathFinder.FindPath(Grid[from], Grid[to]);
        
        [Obsolete]
        public void PrePathFind(Vector2Int from, Vector2Int to)
        {
            var c = PathFinder.FindPath(Grid[from], Grid[to]);

            foreach (var t in _currentPath)
            {
                if (!c.Contains(t))
                    t.IsSelected = false;
            }

            foreach (var t in c)
            {
                if (!t.IsSelected)
                    t.IsSelected = true;
            }

            _currentPath = c;
        }

        [Obsolete]
        public void CancelPrePath()
        {
            if (_currentPath.Count <= 0)
                return;
            
            foreach (var t in _currentPath)
            {
                t.IsSelected = false;
            }
            
            _currentPath.Clear();
        }
        
        #endregion

        #region checkers
        
        private static bool IsAvailable(Tile i) =>
            !i.IsBlocked && !i.IsSelected;

        private static bool IsSpecTile(Tile i) =>
            i is MineTile or WeaponTargetTile or TransTargetTile or CenterStaticBuildingTile;
        
        #endregion

        #endregion

        #region fields
        
        #region instantation
        
        [SerializeField] private string mapPath;
        
        [SerializeField] private DynamicBuildingTile dynamicBuildTile;
        
        [SerializeField] private StaticBuildingTile centerStaticBuildTile;
        
        [SerializeField] private StaticBuildingTile sideStaticBuildTile;
        
        [SerializeField] private MineTile mineTile;

        [SerializeField] private WeaponTargetTile weaponTargetTile;
        
        [SerializeField] private TransTargetTile transTargetTile;
        
        //TODO a mettre dans atlas séparé

        [SerializeField] private DynamicBuilding dynamicGenericBuild;
        
        [SerializeField] private StaticBuilding staticGenericBuild;
        
        //End TODO
        
        #endregion

        private Dictionary<byte, StaticTileGroup> _staticGroups;
        
        #region grid modif
        
        private HashSet<Vector2Int> _addingDynamic; // hashSet go brrrrrrrrr

        private HashSet<Vector2Int> _pathExceed;
        
        private HashSet<Vector2Int> _addingStatic;
        
        private HashSet<Vector2Int> _removing;
        
        private Dictionary<Vector2Int, StaticBuilding> _toAddStatic;
        
        private Dictionary<Vector2Int, Building> _toRemove;

        private GridLockMode _currentGridLockMode;
        
        #endregion
        
        #region path find

        private List<Tile> _currentPath;

        private Tile _currentSelectedTile;

        #endregion

        #endregion
    }
}