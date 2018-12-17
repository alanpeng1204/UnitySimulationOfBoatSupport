using System.Collections.Generic;
using UnityEngine;

namespace KinematicVehicleSystem
{
    public class KinematicManager : MonoBehaviour
    {
        public static KinematicManager Instance;
        private const float CollisionOffset = 0.001f;

        public float BuoyancyRange = 400f;
        public LayerMask PenetrationTestMask;
        public LayerMask InteractiveObjectMask;
        public float OverlapCheckInterval = 0.1f;
        public int TestSpawnCount;
        public GameObject TestPrefab;

        public IWaterProvider WaterProvider { get; private set; }

        private float LastOverlapCheck;
        private List<KinematicController> Controllers = new List<KinematicController>();
        

        public void Register(KinematicController controller)
        {
            Controllers.Add(controller);
        }

        public void Unregister(KinematicController controller)
        {
            Controllers.Remove(controller);
        }

        public bool CalculateDepenetration(KinematicController controller, out Vector3 adjustedPosition, out bool foundInteractiveObject)
        {
            bool adjustPosition = false;
            adjustedPosition = controller.transform.position;
            foundInteractiveObject = false;
            for (int i = 0; i < controller.NeighborResultCount; ++i)
            {
                var collider = controller.Neighbors[i];

                if (collider == null || collider == controller.Collider)
                {
                    continue;
                }

                Vector3 otherPosition = collider.gameObject.transform.position;
                Quaternion otherRotation = collider.gameObject.transform.rotation;

                Vector3 resolutionDirection;
                float resolutionDistance;

                bool overlapped = Physics.ComputePenetration(
                        controller.Collider, controller.transform.position, controller.transform.rotation,
                        collider, otherPosition, otherRotation,
                        out resolutionDirection, out resolutionDistance
                        );

                if (overlapped)
                {
                    Vector3 resolutionMovement = resolutionDirection * (resolutionDistance + CollisionOffset);
                    adjustedPosition += resolutionMovement;
                    adjustPosition = true;

                    if (InteractiveObjectMask.Contains(collider.gameObject.layer))
                    {
                        foundInteractiveObject = true;
                    }
                }
            }

            return adjustPosition;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            WaterProvider = GetComponent<IWaterProvider>();
            Test();
        }

        private void Test()
        {
            for(int i=0;i<TestSpawnCount;i++)
            {
                GameObject ship = Instantiate(TestPrefab);
                ship.transform.position = KVSUtil.RandomPointAtHeight(Vector3.zero, 1000f);
            }
        }

        private void SetControllerNeighbors(KinematicController controller)
        {
            controller.NeighborResultCount = Physics.OverlapSphereNonAlloc(controller.transform.position, controller.NeighborSearchRadius,
                controller.Neighbors, PenetrationTestMask, QueryTriggerInteraction.Ignore);
        }

        private void FixedUpdate()
        {
            bool overlapCheck = false;
            if ((Time.time - LastOverlapCheck) >= OverlapCheckInterval)
            {
                LastOverlapCheck = Time.time;
                overlapCheck = true;
            }

            for (int i = 0;i<Controllers.Count;i++)
            {
                var controller = Controllers[i];

                if (controller == null)
                {
                    Controllers.Remove(controller);
                    continue;
                }

                if (Camera.current != null)
                {
                    controller.DistanceToCamera = Vector3.Distance(Camera.current.transform.position, controller.transform.position);
                }

                if (overlapCheck)
                {
                    SetControllerNeighbors(controller);
                }

                controller.CalculateDepenetration();
                controller.AddMoveVector();
                controller.CalculateMovement();
                controller.Move();
            }

            //Physics.SyncTransforms();
        }

        
    }
}
