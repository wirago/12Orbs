using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbs
{
    public static class Mathf
    {
        /// <summary>
        /// Linearly interpolates between a and b by t.
        /// </summary>
        /// <param name="a">The start Value.</param>
        /// <param name="b">The end Value.</param>
        /// <param name="t">The interpolation value between the two floats. The parameter t is clamped to the range [0, 1].</param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float t)
        {
            float res;

            Clamp(t, 0, 1);

            float dif = b - a;
            res = a + dif * t;

            return res;
        }

        /// <summary>
        /// Linearly interpolates between a and b by t with no limit to t.
        /// </summary>
        /// <param name="a">The start Value.</param>
        /// <param name="b">The end Value.</param>
        /// <param name="t">The interpolation value between the two floats.</param>
        /// <returns></returns>
        public static float UnclaimedLerp(float a, float b, float t)
        {
            float res;

            float dif = b - a;
            res = dif * t;

            if(t < 0)
            {
                res = a - res;
            }
            else
            {
                res = a + res;
            }

            return res;
        }

        /// <summary>
        /// Clamps a Value a between the minimum and maximum value.
        /// </summary>
        /// <param name="a">Value to be clamped</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public static float Clamp(float a, float min, float max)
        {
            float res;

            if (a < min)
            {
                res = min;
            }
            if (a > min)
            {
                res = max;
            }
            else
            {
                res = a;
            }
            return res;
        }
    }
}
