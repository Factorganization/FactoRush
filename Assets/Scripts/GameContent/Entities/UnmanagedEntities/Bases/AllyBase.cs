namespace GameContent.Entities.UnmanagedEntities.Bases
{
    public class AllyBase : Base
    {
        public static AllyBase Instance;
        
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
            //Debug.Log("Ally is dead");
            GameManager.Instance.EnemyWin();

        }

        public override void TakeDamage(float dmg)
        {
            //Debug.Log("Ally took " + dmg + " damage");
            base.TakeDamage(dmg);
            //Debug.Log("Ally has " + health + " health left");
        }
    }
}