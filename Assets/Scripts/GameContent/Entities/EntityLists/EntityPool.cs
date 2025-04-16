using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.EntityLists
{
    public class EntityPool<T> : IEnumerable<T> where T : Entity
    {
        #region properties

        public Queue<T> EntityQueue { get; private set; }
        
        public int Count => EntityQueue.Count;
        
        #endregion
        
        #region constructors
        
        public EntityPool(EntityPoolData<T> poolData, Transform parentTransform = null)
        {
            _unitPrefab = poolData.entityPrefab;
            _poolCount = poolData.poolCount;
            _pooledPosition = poolData.pooledPosition;
            _parentTransform = parentTransform;
            
            Init();
        }
        
        #endregion
        
        #region methodes

        private void Init()
        {
            EntityQueue = new Queue<T>();
            
            for (var i = 0; i < _poolCount; i++)
            {
                var o = Object.Instantiate(_unitPrefab, _pooledPosition, Quaternion.identity, _parentTransform);
                o.Generated();
                o.InitialPosition = _pooledPosition;
                EntityQueue.Enqueue(o);
            }
        }

        public void Enqueue(T o) => EntityQueue.Enqueue(o);
        
        public T Dequeue() => EntityQueue.Dequeue();

        public T Pool()
        {
            var e = Dequeue();
            Enqueue(e);

            return e;
        }
        
        public IEnumerator<T> GetEnumerator() => EntityQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        #region fields
        
        private readonly T _unitPrefab;
        
        private readonly int _poolCount;
        
        private readonly Vector3 _pooledPosition;
        
        private readonly Transform _parentTransform;
        
        #endregion
    }

    [System.Serializable]
    public struct EntityPoolData<T> where T : Entity
    {
        public EntityPoolData(T entityPrefab, int poolCount, Vector3 pooledPosition)
        {
            this.entityPrefab = entityPrefab;
            this.poolCount = poolCount;
            this.pooledPosition = pooledPosition;
        }

        public T entityPrefab;
        
        public int poolCount;
        
        public Vector3 pooledPosition;
    }
}