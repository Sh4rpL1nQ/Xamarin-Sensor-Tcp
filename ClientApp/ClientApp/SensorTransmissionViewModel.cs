using Library;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace ClientApp
{
    public class SensorTransmissionViewModel : PropertyChangedBase
    {
        private float x, y, z;

        public Client Client { get; set; }

        public SensorTransmissionViewModel(Client client) : this()
        {
            Client = client;
            OrientationSensor.Start(SensorSpeed.Game);
            OrientationSensor.ReadingChanged += Accelerometer_ReadingChanged;
        }

        private void Accelerometer_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            var res = SystemConversion.FromQ2(data.Orientation);
            X = res.X;
            Y = res.Y;
            Z = res.Z;
            Client.Send(PackageType.Sensor, string.Format("{0}|{1}|{2}", X, Y, Z));
        }

        public SensorTransmissionViewModel()
        {

        }

        public float X
        {
            get { return x; }
            set { RaisePropertyChanged(ref x, value); }
        }

        public float Y
        {
            get { return y; }
            set { RaisePropertyChanged(ref y, value); }
        }

        public float Z
        {
            get { return z; }
            set { RaisePropertyChanged(ref z, value); }
        }
    }
}
