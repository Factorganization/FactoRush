using UnityEngine;

namespace GameContent.Entities
{
    public abstract class Entity :  MonoBehaviour
    {
        #region properties
        
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        #endregion
    }
}