//using Sirenix.OdinInspector;
using UnityEngine;

namespace KinematicVehicleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicController : MonoBehaviour
    {
        public float DistanceToCamera { get; set; }
        public Collider Collider { get; private set; }
        public Collider[] Neighbors { get; private set; }
        public int NeighborResultCount { get; set; }
        public float NeighborSearchRadius = 150f;

        [SerializeField]
        private int MaxNeighbors = 16;
        [SerializeField]
        private float DepenAccel = 2f;
        [SerializeField]
        private float DepenDecel = 1f;
        [SerializeField]
        private float DepenPushSpeed = 3.5f;
        

        [Header("Read Only")]
        [SerializeField]
        private bool IsPenetrating;
        [SerializeField]
        private Vector3 MoveTarget;
        [SerializeField]
        private Quaternion SteerTarget;

        private Vector3 DepenMoveDirection;
        private Vector3 DepenTargetPosition;
        private Vector3 MoveInputVector = Vector3.zero;
        private bool IsInteractiveObject;
        private IKinematicVehicle KinematicVehicle;
        private KinematicBuoyancy KinematicBuoyancy;
        private Rigidbody Rb;
        private bool IsFrozen;
        private Accelerator Accelerator;
       
        void Awake()
        {
            KinematicBuoyancy = GetComponent<KinematicBuoyancy>();
            KinematicVehicle = GetComponent<IKinematicVehicle>();
            if (KinematicVehicle == null)
            {
                Debug.LogError("IKinematicVehicle component not found");
            }
            Collider = GetComponent<Collider>();
            Rb = GetComponent<Rigidbody>();
            Accelerator = new Accelerator(0f, 5f, 2);
            Neighbors = new Collider[MaxNeighbors];
        }

        private void Start()
        {
            KinematicManager.Instance.Register(this);
        }

        private void OnValidate()
        {
            Rb = GetComponent<Rigidbody>();
            Rb.centerOfMass = Vector3.zero;
            Rb.useGravity = true;
            Rb.drag = 0f;
            Rb.angularDrag = 0f;
            Rb.maxAngularVelocity = Mathf.Infinity;
            Rb.maxDepenetrationVelocity = Mathf.Infinity;
            Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rb.isKinematic = false;
            Rb.constraints = RigidbodyConstraints.None;
            Rb.interpolation = RigidbodyInterpolation.None;

#if UNITY_EDITOR
            Rb.hideFlags = HideFlags.NotEditable;
#endif
        }

        //[Button("Freeze")]
        public void Freeze()
        {
            IsFrozen = true;
        }

        //[Button("Unfreeze")]
        public void Unfreeze()
        {
            IsFrozen = false;
            if (KinematicBuoyancy != null)
            {
                KinematicBuoyancy.ResetToHeight();
            }
        }

        public void CalculateDepenetration()
        {
            IsPenetrating = false;
            Vector3 adjustedPosition;
            bool isInteractiveObject;
            if (KinematicManager.Instance.CalculateDepenetration(this, out adjustedPosition, out isInteractiveObject))
            {
                Accelerator.Current = DepenAccel;
                IsPenetrating = true;
                IsInteractiveObject = isInteractiveObject;
                DepenTargetPosition = adjustedPosition;
                DepenTargetPosition.y = transform.position.y;

                DepenMoveDirection = (DepenTargetPosition - transform.position).normalized;

            }
        }

        public void AddMoveVector()
        {
            MoveInputVector = transform.position;

            if (KinematicVehicle != null)
            {
                MoveInputVector = KinematicVehicle.GetMoveVector();
            }

        }

        public void CalculateMovement()
        {
            if (IsPenetrating)
            {
                MoveTarget = DepenTargetPosition;
            }
            else
            {
                MoveTarget = MoveInputVector;
            }

            if (Accelerator.Current > 0f)
            {
                float multi = Mathf.Sin((Accelerator.Current * DepenPushSpeed * Time.deltaTime) * Mathf.PI * 0.5f);
                if (!IsInteractiveObject)
                {
                    multi = Mathf.Sin((Accelerator.Current * 1 * Time.deltaTime) * Mathf.PI * 0.5f);
                    
                }
                MoveTarget += (DepenMoveDirection * multi);
                Accelerator.Decelerate(DepenDecel);
            } else
            {
                IsInteractiveObject = false;
            }
            
            

            if (KinematicBuoyancy != null)
            {
                bool staticWaterHeight = false;
                if (DistanceToCamera > KinematicManager.Instance.BuoyancyRange)
                {
                    staticWaterHeight = true;
                }
                MoveTarget.y = KinematicBuoyancy.GetHeight(transform.position, staticWaterHeight);
            }

            if (KinematicVehicle != null)
            {
                SteerTarget = KinematicVehicle.GetSteerRotation();
            }
            else
            {
                SteerTarget = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
            }
        }

        public void Move()
        {
            if (IsFrozen && !IsPenetrating)
            {
                Quaternion target = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
                Quaternion wanted = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);
                Rb.MoveRotation(wanted);
                return;
            }

            Rb.MovePosition(MoveTarget);

            if (KinematicBuoyancy != null)
            {
                if (DistanceToCamera < KinematicManager.Instance.BuoyancyRange)
                {
                    Quaternion buoyancyRotation = KinematicBuoyancy.GetRotation(MoveTarget);
                    Rb.MoveRotation(SteerTarget * buoyancyRotation);
                } else
                {
                    Rb.MoveRotation(SteerTarget);
                }
            }
            else
            {
                Rb.MoveRotation(SteerTarget);
            }


        }





    }
}
