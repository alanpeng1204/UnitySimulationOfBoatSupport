using UnityEngine;

namespace KinematicVehicleSystem
{
    public class WaterProviderBasic : MonoBehaviour, IWaterProvider
    {
        void Start()
        {

        }

        public float GetWaterLevel(float x, float z)
        {
            return 0f;
        }

        public float GetStaticWaterLevel()
        {
            return 0f;
        }
    }
}
