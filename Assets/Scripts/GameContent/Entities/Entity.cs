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
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
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

        protected virtual void OnFixedUpdate()
        {
        }
        
        public void SetGraph(Vector3 pos, Quaternion rot)
        {
            graphTransform.position = pos;
            graphTransform.rotation = rot;
        }
        
        protected static float SquareDistance(Vector3 a, Vector3 b) => (b- a).x * (b -a).x + (b - a).y * (b - a).y + (b - a).z * (b - a).z;
        
        #endregion
        
        #region fields
        
        [SerializeField] protected Transform graphTransform;
        
        #endregion
    }
}