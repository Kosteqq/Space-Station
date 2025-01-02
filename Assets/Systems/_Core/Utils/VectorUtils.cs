using UnityEngine;

namespace SpaceStation.Utils
{
    public static class VectorUtils
    {
        public static Vector2 XY(this Vector3 p_vector)
        {
            return new Vector2(p_vector.x, p_vector.y);
        }
        
        public static Vector2 XZ(this Vector3 p_vector)
        {
            return new Vector2(p_vector.x, p_vector.z);
        }

        public static Vector3 AsXY(this Vector2 p_vector, float z = 0f)
        {
            return new Vector3(p_vector.x, p_vector.y, z);
        }

        public static Vector3 AsXZ(this Vector2 p_vector, float y = 0f)
        {
            return new Vector3(p_vector.x, y, p_vector.y);
        }
    }
}