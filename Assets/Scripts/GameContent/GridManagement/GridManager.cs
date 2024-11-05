using System.Collections.Generic;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.GridManagement
{
    public sealed class GridManager : MonoBehaviour
    {
        #region methodes

        #region Unity events
        
        private void Start()
        {
            _grid = new Dictionary<Vector2Int, Tile>();
            _adding = new Dictionary<Vector2Int, Tile>();
            _removing = new Dictionary<Vector2Int, Tile>();
            
            InitGrid();
        }

        private void Update()
        {
            UpdateGrid();
        }
        
        #endregion
        
        #region grid
        
        private void InitGrid()
        {
            _currentGridLockMode = GridLockMode.Unlocked;

            var grid = MapParser.ParseMap(mapPath);
            
            for (var i = 0; i < grid.Length; i++)
            {
                for (var j = 0; j < grid[i].Length; j++)
                {
                    var tId = new Vector2Int(i, j);
                    var tPos = new Vector3(j, 0, i); //inverted i j for pos /!\
                    
                    switch (grid[i][j])
                    {
                        case 0:
                            var dBt = Instantiate(dynamicBuildTile, transform);
                            _grid.Add(tId, dBt);
                            dBt.Added(this, tId, tPos);
                            break;
                        
                        case > 0:
                            var sBt = Instantiate(staticBuildTile, transform);
                            _grid.Add(tId, sBt);
                            sBt.Added(this, tId, tPos);
                            sBt.StaticGroup = grid[i][j];
                            break;
                    }
                }
            }
            
            _currentGridLockMode = GridLockMode.Locked;
        }

        private void UpdateGrid()
        {
            if (_currentGridLockMode is GridLockMode.Locked)
                return;

            if (_adding.Count > 0)
            {
                foreach (var t in _adding)
                {
                    
                    _grid.Add(t.Key, t.Value);
                }
            }
            _adding.Clear();

            if (_removing.Count > 0)
            {
                foreach (var t in _removing)
                {
                    Destroy(_grid[t.Key].gameObject);
                    _grid.Remove(t.Key);
                }
            }
            _removing.Clear();
        }

        private void AddTileAt(Tile tile, Vector2Int index, Vector3 pos)
        {
            _adding.TryAdd(index, tile);
        }

        private void PlaceTileAt(Tile tile, Vector3 pos) => tile.Position = pos;

        private void RemoveTileAt(Tile tile, Vector2Int index) => _removing.TryAdd(index, tile);
        
        public void AddBuildingAt(Building building, Tile tile)
        {
            tile.CurrentOnTopBuilding = building;
            building.Added(tile);
        }
        
        public void AddBuildingAt(Building building, Vector2Int index)
        {
            _grid[index].CurrentOnTopBuilding = building;
            building.Added(_grid[index]);
        }

        #endregion
        
        #endregion
        
        #region fields

        [SerializeField] private string mapPath;
        
        [SerializeField] private DynamicBuildingTile dynamicBuildTile;
        
        [SerializeField] private StaticBuildingTile staticBuildTile;
        
        private Dictionary<Vector2Int,Tile> _grid;
        
        private Dictionary<Vector2Int,Tile> _adding;
        
        private Dictionary<Vector2Int,Tile> _removing;

        private GridLockMode _currentGridLockMode;
        
        #endregion
    }
}