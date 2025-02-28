﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using GameContent.Entities.OnFieldEntities.Buildings;
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

        public Dictionary<sbyte, AssemblyTileGroup> AssemblyTileGroups { get; private set; }
        
        public Dictionary<byte, FactoryBuilding> FactoryRefs => _factoryRefs;
        
        public Dictionary<Vector2Int, Tile> Grid { get; private set; }
        
        public Dictionary<byte, StaticTileGroup> StaticGroups { get; private set; }
        
        #endregion
        
        #region methodes

        #region Unity events

        private void Awake()
        {
            /*if (Manager is not null)
                throw new Exception("Only one instance of a GridManager can be active at a time.");*/
            
            Manager = this;
        }
        
        [ContextMenu("Start")]
        private void Start()
        {
            mapPath = "MapNiveau/" + GameManager.Instance.LevelToLoad;
            
            _currentPath = new List<Tile>();
            _currentCompletionPath = new List<Tile>();
            _pathAddonIndex = -1;
            
            Grid = new Dictionary<Vector2Int, Tile>();
            StaticGroups = new Dictionary<byte, StaticTileGroup>();
            AssemblyTileGroups = new Dictionary<sbyte, AssemblyTileGroup>();
            ConveyorGroups = new Dictionary<sbyte, ConveyorGroup>();
            _currentConveyorPath = new List<DynamicBuilding>();
            _factoryRefs = new Dictionary<byte, FactoryBuilding>();
            
            _addingDynamic = new HashSet<Vector2Int>();
            _pathExceed = new HashSet<Vector2Int>();
            _addingStatic = new HashSet<Vector2Int>();
            _removing = new HashSet<Vector2Int>();
            _toAddStatic = new Dictionary<Vector2Int, StaticBuilding>();
            _toRemove = new Dictionary<Vector2Int, Building>();
            
            StartCoroutine(InitGrid());
        }
        
        #endregion
        
        #region grid
        
        /// <summary>
        /// <list type = "table">
        /// <listheader>
        /// ID to Tile Type
        /// </listheader>
        /// <item>0 : dynamic build (ground)</item>
        /// <item>01 - 09 : refinery build</item>
        /// <item>10 - 19 : assembly build</item>
        /// <item>20 - 29 : mining build
        /// <list type= "Mine Resources">
        /// <listheader>
        /// Mine Resources
        /// </listheader>
        /// <item>0 : Iron</item>
        /// <item>1 : Copper</item>
        /// <item>2 : Gold</item>
        /// <item>3 : Diamond</item>
        /// </list></item>
        /// </list>
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
                            mT.SetResource(miningResources[grid[i][j] % 20]); // B)
                            break;
                        
                        case >= 10:
                            var k = (sbyte)(grid[i][j] % 20);
                            
                            if (!AssemblyTileGroups.ContainsKey(k))
                            {
                                AssemblyTileGroups.Add(k, new AssemblyTileGroup(k));
                                
                                var wTt = Instantiate(weaponTargetTile, transform);
                                Grid.Add(tId, wTt);
                                AssemblyTileGroups[k].AddTile(wTt);
                                wTt.Added(this, tId, tPos, TileType.WeaponTarget);
                                wTt.InitAssemblyTile(k);
                            }

                            else
                            {
                                var tTt = Instantiate(transTargetTile, transform);
                                Grid.Add(tId, tTt);
                                AssemblyTileGroups[k].AddTile(tTt);
                                tTt.Added(this, tId, tPos, TileType.TransTarget);
                                tTt.InitAssemblyTile(k);
                                tTt.SetWeaponRef(AssemblyTileGroups[k].WeaponTile);
                                tTt.SetBinTileRef(AssemblyTileGroups[k].WeaponTile);
                            }
                            break;
                        
                        case > 0:
                            if (!StaticGroups.ContainsKey(grid[i][j]))
                            {
                                StaticGroups.Add(grid[i][j], new StaticTileGroup());
                                _factoryRefs.Add(grid[i][j], null);
                            }
                            
                            var c = StaticGroups[grid[i][j]].Count + 1;
                            var sBt = Instantiate( centerStaticBuildTile, transform);
                            
                            Grid.Add(tId, sBt);
                            StaticGroups[grid[i][j]].AddTile(sBt);
                            
                            sBt.Added(this, tId, tPos, TileType.CenterStaticTile);
                            sBt.InitStaticTile(grid[i][j], StaticBuildingTile.GetStaticRotation(c));
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
                    ConveyorGroups.Add(i, new ConveyorGroup(i));
                else
                    ConveyorGroups[i] = new ConveyorGroup(i);

                switch (_pathAddonIndex)
                {
                    case < 0:
                        foreach (var t in _currentPath)
                        {
                            if (InstantiateBuildingAt(dynamicGenericBuild, Grid[t.Index].ETransform) is not DynamicBuilding b)
                                continue;
                        
                            ConveyorGroups[i].AddBuild(b);
                            b.AddConveyorGroupId(i);
                            b.SetDebugId(); //i
                            PlaceBuildingAt(t.Index, b);
                        
                            Grid[t.Index].IsSelected = false;
                        }
                    
                        ConveyorGroups[i].Init();
                        break;
                    
                    case >= 0:
                        _currentPath[0].IsSelected = false;

                        if (_currentPath[0].CurrentBuildingRef is not DynamicBuilding db)
                        {
                            CancelAdding();
                            break;
                        }
                        
                        ConveyorGroups[i].AddBuild(db);
                        db.AddConveyorGroupId(i);
                        db.SetDebugId();
                        
                        for (var k = 1; k < _currentPath.Count; k++)
                        {
                            var t = _currentPath[k];
                            
                            if (InstantiateBuildingAt(dynamicGenericBuild, Grid[t.Index].ETransform) is not
                                DynamicBuilding b)
                                continue;

                            ConveyorGroups[i].AddBuild(b);
                            b.AddConveyorGroupId(i);
                            b.SetDebugId(); //i
                            PlaceBuildingAt(t.Index, b);

                            Grid[t.Index].IsSelected = false;
                        }

                        ConveyorGroups[i].InitFromDynamic(_pathAddonIndex);
                        break;
                }  
            }
            _addingDynamic.Clear();
            _currentConveyorPath.Clear();
            _currentPath.Clear();
            _pathAddonIndex = -1;
            
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
        
        public void MarkAddonFlag(sbyte index) => _pathAddonIndex = index;
        
        public void SetSelected(Vector2Int index, bool selected)
        {
            Grid[index].IsSelected = selected;

            switch (selected)
            {
                case true:
                    if (_addingDynamic.Add(index))
                        _currentPath.Add(Grid[index]);
                    break;
                
                case false:
                    _addingDynamic.Remove(index);
                    _currentPath.Remove(Grid[index]);
                    break;
            }
        }

        public void TryCorrectPath(Vector2Int from, Vector2Int to)
        {
            _currentPath = PathFinder.FindPath(Grid[from], Grid[to]);
            foreach (var i in _addingDynamic)
            {
                if (_currentPath.Contains(Grid[i]))
                    continue;
                
                Grid[i].IsSelected = false;
                _pathExceed.Add(i);
            }
            
            if (_pathExceed.Count <= 0)
                return;
            
            foreach (var i in _pathExceed)
            {
                _addingDynamic.Remove(i);
            }
            
            _lastSelectedTile = _currentPath[^2];
            
            _pathExceed.Clear();
        }
        
        public bool TryAddDynamicBuildingAt(Vector2Int from, Vector2Int previous, Vector2Int index) // yes. bool.
        {
            if ((IsSpecTile(Grid[index]) && index != from) || (Grid[index].IsBlocked && Grid[index] is not CenterStaticBuildingTile))
                return false;
            
            if (_lastSelectedTile is not null && Vector2Int.Distance(index, _lastSelectedTile.Index) > 1.1f)
            {
                _currentCompletionPath = PathFinder.FindCompletionPath(Grid[index], _lastSelectedTile);
                foreach (var i in _currentCompletionPath)
                {
                    i.IsSelected = true;
                    _addingDynamic.Add(i.Index);
                }
            }
            
            _lastSelectedTile = Grid[index];
            
            var distance = Vector2Int.Distance(index, previous);
            switch (distance)
            {
                case > 1.1f and < 2 when !TryCompletePath(previous, index):
                    CancelAdding();
                    return false;
                case >= 2:
                    return false;
            }

            _addingDynamic.Add(index);
            _lastSelectedTile.IsSelected = true;

            _currentPath = PathFinder.FindPath(Grid[from], _lastSelectedTile);

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
        
        public bool TryAddStaticBuildingAt(Vector2Int index, Vector3 pos, byte staticGroupId, out StaticBuilding b) // bool again
        {
            b = null;
            
            if (!_addingStatic.Add(index))
                return false;
            
            b = Instantiate(staticGenericBuild, Grid[index].ETransform);
            _factoryRefs[staticGroupId] = b as FactoryBuilding;
            _toAddStatic.Add(index, b);
            b.TargetPosition = pos;
            b.Position = pos;
            return true;
        }
        
        public void CancelAdding()
        {
            _lastSelectedTile = null;
            
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
            
            _toRemove.Add(index, Grid[index].CurrentBuildingRef);
            var b = Grid[index].CurrentBuildingRef as DynamicBuilding;
            var i = new List<sbyte>();
            i.AddRange(b!.ConveyorGroupIds);

            var c = ConveyorGroups[i[0]];
            for (var k = c.Count - 1; k >= 0; k--)
            {
                if (i.Count >= c[k].ConveyorGroupIds.Count)
                    continue;

                var l = new HashSet<float>();
                var inc = c[k].TileRef.Index - c[k - 1].TileRef.Index;
                var next = c[k + 1].TileRef.Index - c[k].TileRef.Index;
                var a = Vector2.SignedAngle(inc, next);

                var cb = c[0].TileRef.GroupRef;

                foreach (var i2 in cb)
                {
                    next = i2[k + 1].TileRef.Index - i2[k].TileRef.Index;
                    var a2 = Vector2.SignedAngle(inc, next);
                    if (Mathf.Approximately(a, a2))
                        continue;
                    
                    l.Add(a2);
                }
                
                if (l.Contains(-90) && l.Contains(90))
                    c[k].SetGraphId(5);
            
                else if (l.Contains(0) && l.Contains(90))
                    c[k].SetGraphId(3);
            
                else if (l.Contains(-90) && l.Contains(0))
                    c[k].SetGraphId(4);
                
                else if (l.Contains(90))
                    c[k].SetGraphId(1);
                
                else if (l.Contains(-90))
                    c[k].SetGraphId(2);
                
                else if (l.Contains(0))
                    c[k].SetGraphId(0);

                var angle = DynamicBuilding.GetRotation(c[k].TileRef.Index - c[k - 1].TileRef.Index);
                if (l.Count == 1 && l.Contains(-90))
                    angle += 90;
                if (l.Count == 1 && l.Contains(90))
                    angle -= 90;
                c[k].SetGraph(c[k].Position, Quaternion.Euler(0, angle, 0));
                
                break;
            }
            
            foreach (var j in i)
            {
                foreach (var b2 in ConveyorGroups[j])
                {
                    if (i.Count >= b2.ConveyorGroupIds.Count)
                    {
                        if (_removing.Add(b2.TileRef.Index))
                            _toRemove.Add(b2.TileRef.Index, b2);
                    }
                    else
                    {
                        b2.RemoveConveyorGroupId(j);
                    }
                }
                ConveyorGroups[j].MarkActive(false);
            }

            foreach (var j in i)
            {
                ConveyorGroups[j].UpdateGroup();
                ConveyorGroups[j].DestroyGroup();
                ConveyorGroups.Remove(j);
            }
        }

        public void TryRemoveStaticBuildingAt(Vector2Int index)
        {
            if (!_removing.Add(index))
                return;
            
            _toRemove.Add(index, Grid[index].CurrentBuildingRef);

            if (Grid[index] is not StaticBuildingTile t)
                return;

            _factoryRefs[t.StaticGroup] = null;
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
            switch (Grid[index])
            {
                case CenterStaticBuildingTile when building is FactoryBuilding:
                    Destroy(building.gameObject);
                    Grid[index].CurrentBuildingRef = null;
                    break;
                
                case CenterStaticBuildingTile when building is DynamicBuilding:
                    Destroy(building.gameObject);
                    break;
                
                default:
                    Destroy(building.gameObject);
                    Grid[index].CurrentBuildingRef = null;
                    break;
                
            }
        }

        #endregion
        
        #endregion

        #region path find
        
        [Obsolete]
        public void SetPath()
        {
            //foreach (var t in _currentPath)
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
        
        public bool IsLastSelectedTile(Vector2Int index) => _lastSelectedTile == Grid[index];
        
        public bool IsEmptyPath() => _addingDynamic.Count == 0;
        
        #endregion

        #endregion

        #region fields
        
        #region instantation
        
        [SerializeField] private string mapPath;
        
        [SerializeField] private DynamicBuildingTile dynamicBuildTile;
        
        [SerializeField] private StaticBuildingTile centerStaticBuildTile;
        
        [SerializeField] private StaticBuildingTile sideStaticBuildTile;
        
        [SerializeField] private MineTile mineTile;

        [SerializeField] private MiningResource[] miningResources;
        
        [SerializeField] private WeaponTargetTile weaponTargetTile;
        
        [SerializeField] private TransTargetTile transTargetTile;

        [SerializeField] private DynamicBuilding dynamicGenericBuild;
        
        [SerializeField] private StaticBuilding staticGenericBuild;
        
        #endregion

        #region groups

        private Dictionary<byte, FactoryBuilding> _factoryRefs;
        
        #endregion
        
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

        private List<Tile> _currentCompletionPath;
        
        private List<DynamicBuilding> _currentConveyorPath;

        private Tile _lastSelectedTile;

        private sbyte _pathAddonIndex;

        #endregion

        #endregion
    }
}