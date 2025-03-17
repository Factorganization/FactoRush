using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Bases
{
    public abstract class Base : MonoBehaviour
    {
        public float health;
        public bool isDead;
        
        [SerializeField] public Transform spawnPoint;
        
        private void Awake()
        {
            OnAwake();
        }
        
        protected virtual void OnAwake()
        {
            health = 100;
            isDead = false;
        }
        
        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0 && !isDead)
            {
                Die();
            }
        }
        
        protected virtual void Die()
        {
            isDead = true;
        }
        
        
    }
}