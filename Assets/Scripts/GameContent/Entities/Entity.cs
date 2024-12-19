using UnityEngine;

namespace GameContent.Entities
{
    public abstract class Entity :  MonoBehaviour
    {
        #region properties

        public Transform ETransform { get; private set; }
        
        public Vector3 Position
        {
            get => ETransform.position;
            set => ETransform.position = value;
        }
        
        public Vector3 TargetPosition { get; set; }
        
        #endregion
        
        #region methodes

        private void Awake()
        {
            OnAwake();
            transform.position = TargetPosition;
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }
        
        protected virtual void OnAwake()
        {
            ETransform = GetComponent<Transform>(); //la tech de Vincent oue oue oue grrr paw
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }
        
        #endregion
        
        #region fields
        
        [SerializeField] protected Transform graphTransform;
        
        #endregion
    }
}