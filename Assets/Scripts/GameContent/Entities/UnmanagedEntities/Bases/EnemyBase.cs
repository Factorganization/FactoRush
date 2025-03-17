namespace GameContent.Entities.UnmanagedEntities.Bases
{
    public class EnemyBase : Base
    {
        public static EnemyBase Instance;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            if (Instance is null)
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
            //Debug.Log("Enemy is dead");
            GameManager.Instance.AllyWin();
        }

        public override void TakeDamage(float dmg)
        {
            //Debug.Log("Enemy took " + dmg + " damage");
            base.TakeDamage(dmg);
            //Debug.Log("Enemy has " + health + " health left");
        }
    }
}