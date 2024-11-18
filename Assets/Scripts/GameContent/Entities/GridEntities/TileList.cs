using System;
using System.Collections;
using System.Collections.Generic;

namespace GameContent.Entities.GridEntities
{
    public abstract class TileList : IEnumerable<Tile>
    {
        #region properties

        public int Count => _tiles.Count;
        
        public Tile this[int index]
        {
            get
            {
                if (index < 0 || index >= _tiles.Count)
                    throw new IndexOutOfRangeException();

                return _tiles[index];
            }
            set => _tiles[index] = value;
        }

        #endregion
        
        #region constructors

        protected TileList(params Tile[] tiles)
        {
            _tiles = new List<Tile>(tiles);
        }

        #endregion
        
        #region methodes

        public virtual void AddTile(Tile tile)
        {
            _tiles.Add(tile);
        }
        
        public IEnumerator<Tile> GetEnumerator() => _tiles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion
        
        #region fields
        
        private readonly List<Tile> _tiles;
        
        #endregion
    }
}