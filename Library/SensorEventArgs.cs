using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class SensorEventArgs : EventArgs
    {
        public SensorEventArgs(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}
