using UnityEngine;

namespace KinematicVehicleSystem
{
    public static class KVSUtil
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        public static Quaternion RotateTowards(Quaternion current, Vector3 dir, float speed = 2f)
        {
            Quaternion toTarget = Quaternion.LookRotation(dir);

            Quaternion rot = Quaternion.Slerp(current, toTarget, speed * Time.deltaTime);
            return rot;
        }

        public static Vector3 RandomPointAtHeight(Vector3 center, float range)
        {
            return new Vector3(RandomPoint(center.x, range), center.y, RandomPoint(center.z, range));
        }

        public static float RandomPoint(float p, float range)
        {
            return Random.Range(p - range, p + range);
        }
    }
}
