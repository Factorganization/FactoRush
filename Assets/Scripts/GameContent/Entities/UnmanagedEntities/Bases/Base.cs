using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class Base : MonoBehaviour
    {
        public float health;
        public float damage;
        public bool isDead;
        
        [SerializeField] public Transform spawnPoint;
        
        private void Awake()
        {
            OnAwake();
        }
        
        protected virtual void OnAwake()
        {
            health = 100;
            damage = 10;
            isDead = false;
        }
        
        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
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