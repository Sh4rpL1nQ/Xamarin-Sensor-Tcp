using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Library
{
    public static class SystemConversion
    {
        public static Vector3 FromQ2(Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.W - q1.Y * q1.Z;
            Vector3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.Y = ToFloat(2f * Math.Atan2(q1.Y, q1.X));
                v.X = ToFloat(Math.PI / 2);
                v.Z = 0;
                return NormalizeAngles(v * Rad2Deg);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.Y = ToFloat(-2f * Math.Atan2(q1.Y, q1.X));
                v.X = ToFloat(-Math.PI / 2);
                v.Z = 0;
                return NormalizeAngles(v * Rad2Deg);
            }
            Quaternion q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch
            v.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));      // Roll
            return NormalizeAngles(v * Rad2Deg);
        }

        static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.X = NormalizeAngle(angles.X);
            angles.Y = NormalizeAngle(angles.Y);
            angles.Z = NormalizeAngle(angles.Z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        static float ToFloat(double input)
        {
            float result = (float)input;
            if (float.IsPositiveInfinity(result))
                return float.MaxValue;
            else if (float.IsNegativeInfinity(result))
                return float.MinValue;
            else
                return result;
        }
    }
}
