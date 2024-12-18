using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class Base : MonoBehaviour
    {
        public float Health;
        public float Damage;
        public bool IsDead;
        
        private void Awake()
        {
            OnAwake();
        }
        
        protected virtual void OnAwake()
        {
            Health = 100;
            Damage = 10;
            IsDead = false;
        }
        
        public virtual void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }
        }
        
        protected virtual void Die()
        {
            IsDead = true;
        }
        
        
    }
}