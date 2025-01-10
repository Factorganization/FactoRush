namespace GameContent.Entities.OnFieldEntities
{
    public abstract class StaticBuilding : Building
    {
        #region methodes

        private void Update()
        {
            OnUpdate();
        }

        protected override void OnUpdate()
        {
        }

        #endregion
    }
}