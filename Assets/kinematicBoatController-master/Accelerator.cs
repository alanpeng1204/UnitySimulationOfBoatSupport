using UnityEngine;

namespace KinematicVehicleSystem
{
    public class Accelerator
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float Current { get; set; }
        public float Deceleration { get; set; }

        public Accelerator(float min, float max, float decel)
        {
            Min = min;
            Max = max;
            Deceleration = decel;
        }

        public void Accelerate(float accel)
        {
            Current = Current + (accel * Time.deltaTime);
            Clamp();
        }

        public void Decelerate(float decel)
        {
            Current = Current - (decel * Time.deltaTime);
            Clamp();
        }

        private void Clamp()
        {
            Current = Mathf.Clamp(Current, Min, Max);
        }
    }
}
