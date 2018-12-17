using UnityEngine;

namespace KinematicVehicleSystem
{
    public class KinematicBasicVehicle : MonoBehaviour, IKinematicVehicle
    {
        [Range(0, 1)]
        public float MotorForwardAcceleration = 0.5f;
        [Range(0, 1)]
        public float MotorDeceleration = 1f;
        [Range(0, 1)]
        public float MotorReverseAcceleration = 0.25f;
        [Range(0, 1)]
        public float SteerAcceleration = 0.5f;
        [Range(0, 1)]
        public float SteerDeceleration = 0.5f;
        [Range(0, 1)]
        public float MotorPower = 0.4f;
        [Range(0, 1)]
        public float SteerPower = 0.5f;

        public float NetworkInterval = 0.05f;
        public float LastNetworkUpdate;

        public float Haxis;
        public float Vaxis;
        public float Heading;
        public Vector3 RemotePosition;
        public float RemoteHeading;

        public KinematicController KinematicController { get; protected set; }

        private float Motor;
        private float Steer;

        [SerializeField]
        private VehicleControlType CurrentControlType;

        private Collider Collider;
        private Rigidbody Rb;
        private BoatAiBasic BoatAi;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            Rb = GetComponent<Rigidbody>();

            KinematicController = GetComponent<KinematicController>();

            //FollowCamBasic cam = FindObjectOfType<FollowCamBasic>();
            //cam.SetTarget(transform);
        }

        private void Start()
        {
            CurrentControlType = GetControlType();
        }

        public float CurrentSpeed
        {
            get { return Motor; }
        }

        public Vector3 GetMoveVector()
        {
            switch (CurrentControlType)
            {
                case VehicleControlType.LocalAi:
                    return transform.position + (Motor * MotorPower * transform.forward);
                case VehicleControlType.Remote:
                    if (RemotePosition == Vector3.zero)
                    {
                        return transform.position;
                    }
                    else
                    {
                        return RemotePosition;
                    }

                case VehicleControlType.LocalPlayer:
                    return transform.position + (Motor * MotorPower * transform.forward);
                default:
                    return transform.position;
            }

        }

        public Quaternion GetSteerRotation()
        {

            switch (CurrentControlType)
            {
                case VehicleControlType.LocalAi:
                    return Quaternion.Euler(new Vector3(0f, BoatAi.TargetRotation.eulerAngles.y, 0f));
                case VehicleControlType.Remote:

                    if (RemoteHeading == 0)
                    {
                        return Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
                    }
                    else
                    {
                        return Quaternion.Euler(new Vector3(0f, RemoteHeading, 0f));
                    }

                case VehicleControlType.LocalPlayer:
                    float steer = Steer * (Motor * SteerPower);
                    return Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y + steer, 0f));
                default:
                    return Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
            }

        }

        public VehicleControlType GetControlType()
        {
            return VehicleControlType.LocalPlayer;
        }

        private bool CanMove()
        {
            return true;
        }

        private void Update()
        {
            CurrentControlType = GetControlType();

            switch (CurrentControlType)
            {
                case VehicleControlType.Remote:
                    return;
                case VehicleControlType.LocalAi:
                    break;
                case VehicleControlType.LocalPlayer:
                    Vaxis = Input.GetAxis("Vertical");
                    Haxis = Input.GetAxis("Horizontal");
                    break;
            }

            Vector3 forward = transform.forward;
            forward.y = 0;
            Heading = Quaternion.LookRotation(forward).eulerAngles.y;

            UpdateRemote();

            if (!CanMove())
            {
                return;
            }

            switch (CurrentControlType)
            {
                case VehicleControlType.LocalAi:
                    UpdateAiInput();
                    break;
                case VehicleControlType.LocalPlayer:
                    UpdatePlayerInput();
                    break;
            }
        }

        private void UpdateAiInput()
        {
            Motor = Mathf.Lerp(Motor, BoatAi.Speed, MotorForwardAcceleration * Time.deltaTime);

            Vector3 direction = (BoatAi.TargetPosition - transform.position);
            Steer = Mathf.Lerp(Steer, BoatAi.Speed, SteerAcceleration * Time.deltaTime);
            float finalSteer = Steer * (Motor * SteerPower);
            BoatAi.TargetRotation = KVSUtil.RotateTowards(transform.rotation, direction, finalSteer);
        }

        private void UpdatePlayerInput()
        {
            if (Vaxis > 0f)
            {
                Motor = Mathf.Lerp(Motor, Vaxis, MotorForwardAcceleration * Time.deltaTime);
            }
            else if (Vaxis < 0f)
            {
                Motor = Mathf.Lerp(Motor, Vaxis, MotorReverseAcceleration * Time.deltaTime);
            }
            else
            {
                Motor = Mathf.Lerp(Motor, Vaxis, MotorDeceleration * Time.deltaTime);
            }

            if (Haxis == 0f)
            {
                Steer = Mathf.Lerp(Steer, Haxis, SteerDeceleration * Time.deltaTime);
            }
            else
            {
                Steer = Mathf.Lerp(Steer, Haxis, SteerAcceleration * Time.deltaTime);
            }
        }

        private void UpdateRemote()
        {
            if ((Time.time - LastNetworkUpdate) >= NetworkInterval)
            {
                LastNetworkUpdate = Time.time;
                // Send network update here
            }
        }

    }
}
