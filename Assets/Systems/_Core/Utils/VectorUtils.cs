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
    }
}