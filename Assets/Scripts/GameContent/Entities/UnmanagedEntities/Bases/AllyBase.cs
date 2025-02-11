using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Bases
{
    public class AllyBase : Base
    {
        public static AllyBase Instance;
        
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
            Debug.Log("Ally is dead");
            GameManager.Instance.EnemyWin();
            health = 9999999999;

        }

        public override void TakeDamage(float damage)
        {
            Debug.Log("Ally took " + damage + " damage");
            base.TakeDamage(damage);
            Debug.Log("Ally has " + health + " health left");
        }
    }
}