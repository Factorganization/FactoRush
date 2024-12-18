namespace GameContent.Entities.OnFieldEntities
{
    public class StaticBuilding : Building
    {
        #region methodes

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        #endregion
    }
}