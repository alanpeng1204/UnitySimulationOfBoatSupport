using System.Collections;
using UnityEngine;

namespace KinematicVehicleSystem
{
    [RequireComponent(typeof(KinematicController))]
    public class KinematicBuoyancy : MonoBehaviour
    {
        public float Width = 6f;
        public float Length = 14f;

        public Vector3 Gravity = new Vector3(0, -9.81f, 0);
        public float AccelerationMulti = 1f;
        public float MaxAccel = 10f;
        public float AccelerationSmooth = 1f;
        public float DecelerationConstant = 0.5f;
        public float RotationSpeed = 4f;
        public float RotationAngleMulti = 6f;
        public float ResetForce = 0.2f;
       
        private bool Resetting;
        private Accelerator Accelerator;

        // For debugging
        [SerializeField]
        private Vector3 Front;
        [SerializeField]
        private Vector3 Back;
        [SerializeField]
        private Vector3 Left;
        [SerializeField]
        private Vector3 Right;

        private void Awake()
        {
            Accelerator = new Accelerator(0f, 50f, DecelerationConstant);
            Accelerator.Current = ResetForce;
        }

        public float GetHeight(Vector3 position, bool staticHeight = false)
        {
            if (staticHeight)
            {
                return GetStaticWaterLevel();
            } else
            {
                return CalculateVerticalVelocity(position.y, GetWaterLevel(position.x, position.z));
            }
        }

        public Quaternion GetRotation(Vector3 position)
        {
            float halfLength = Length / 2f;
            float halfWidth = Width / 2f;
            Front = new Vector3(position.x, position.y, position.z + halfLength);
            Front.y = GetWaterLevel(Front.x, Front.z);
            Back = new Vector3(position.x, position.y, position.z - halfLength);
            Back.y = GetWaterLevel(Back.x, Back.z);
            Left = new Vector3(position.x - halfWidth, position.y, position.z);
            Left.y = GetWaterLevel(Left.x, Left.z);
            Right = new Vector3(position.x + halfWidth, position.y, position.z);
            Right.y = GetWaterLevel(Right.x, Right.z);

            float zangle = 0f;
            float xangle = 0f;
            if (Front.y > Back.y)
            {
                zangle = -(Front.y - Back.y);
            }
            else if (Back.y > Front.y)
            {
                zangle = (Back.y - Front.y);
            }
            else
            {
                zangle = 0f;
            }

            if (Left.y > Right.y)
            {
                xangle = -(Left.y - Right.y);
            }
            else if (Right.y > Left.y)
            {
                xangle = (Right.y - Left.y);
            }
            else
            {
                xangle = 0f;
            }

            Vector3 targetRot = new Vector3(xangle * RotationAngleMulti, 0f, zangle * RotationAngleMulti);
            Vector3 originRot = new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
            Quaternion rot = Quaternion.Slerp(Quaternion.Euler(originRot), Quaternion.Euler(targetRot), RotationSpeed * Time.deltaTime);
            return rot;
        }

        public void ResetToHeight()
        {
            Accelerator.Current = 0f;
            Resetting = true;
            StartCoroutine(ResetTimeout());
        }

        private float GetWaterLevel(float x, float z)
        {
            return KinematicManager.Instance.WaterProvider.GetWaterLevel(x, z);
        }

        private float GetStaticWaterLevel()
        {
            return KinematicManager.Instance.WaterProvider.GetStaticWaterLevel();
        }

        private IEnumerator ResetTimeout()
        {
            yield return new WaitForSeconds(1f);
            Resetting = false;
            Accelerator.Current = ResetForce;
        }

        private float CalculateVerticalVelocity(float current, float waterLevel)
        {
            if (Resetting)
            {
                return Mathf.Lerp(current, waterLevel, 1f * Time.deltaTime);
            }

            float height = current;
            Accelerator.Max = MaxAccel;
            height += Gravity.y * Time.deltaTime;
            
            if (height < waterLevel)
            {
                float force = (waterLevel - height) * Mathf.Exp(AccelerationMulti * Time.deltaTime);
                Accelerator.Accelerate(force);
            }
            Accelerator.Decelerate(DecelerationConstant);


            height += Accelerator.Current;

            height = Mathf.Lerp(height, waterLevel, AccelerationSmooth * Time.deltaTime);
           
            return height;
        }
    }
}
