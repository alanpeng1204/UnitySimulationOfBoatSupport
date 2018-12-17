using UnityEngine;

namespace KinematicVehicleSystem
{
    public class FollowCamBasic : MonoBehaviour
    {
        private static Vector3 WaterRotation = new Vector3(30f, 150f, 0f);
        public float Distance = 20f;

        public float zoomSpeed = 100.0f;
        public float mouseZoomMultiplier = 5.0f;
        public float minZoomDistance = 20.0f;
        public float maxZoomDistance = 200.0f;

        public Transform Target;
        public bool LocalMovement;


        public void SetTarget(Transform target)
        {
            Target = target;
        }

        private void Awake()
        {
            transform.localEulerAngles = WaterRotation;
        }

        private void LateUpdate()
        {
            if (Target == null)
            {
                return;

            }

            transform.position = Target.position;
            UpdateZooming();
            transform.Translate(Vector3.back * Distance);

            if (Input.GetMouseButton(1))
            {
                transform.LookAt(Target);
                transform.RotateAround(Target.position, Vector3.up, Input.GetAxis("Mouse X") * 5f);

            }

        }

        private void UpdateZooming()
        {
            float deltaZoom = 0.0f;
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            deltaZoom -= scroll * mouseZoomMultiplier;
            Distance = Mathf.Max(minZoomDistance, Mathf.Min(maxZoomDistance, Distance + deltaZoom * Time.deltaTime * zoomSpeed));
        }

    }
}
