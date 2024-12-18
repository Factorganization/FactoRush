using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    public class EnemyBase : Base
    {
        public static EnemyBase Instance;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            if (Instance == null)
            {
                Instance = this;
            }
            else 
            {
                Destroy(gameObject);
            }
        }
        
        protected override void Die()
        {
            Debug.Log("Enemy is dead");
        }

        public override void TakeDamage(float damage)
        {
            Debug.Log("Enemy took " + damage + " damage");
            base.TakeDamage(damage);
            Debug.Log("Enemy has " + health + " health left");
        }
    }
}