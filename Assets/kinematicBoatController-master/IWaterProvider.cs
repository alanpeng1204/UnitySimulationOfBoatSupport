namespace KinematicVehicleSystem
{
    public interface IWaterProvider
    {
        float GetWaterLevel(float x, float z);
        float GetStaticWaterLevel();
    }
}
