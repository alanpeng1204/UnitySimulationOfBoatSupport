using UnityEngine;

namespace KinematicVehicleSystem
{
    public interface IKinematicVehicle
    {

        Vector3 GetMoveVector();
        Quaternion GetSteerRotation();
    }
}
