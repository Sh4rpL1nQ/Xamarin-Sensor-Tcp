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
            Accelerometer.Start(SensorSpeed.Game);
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            X = data.Acceleration.X;
            Y = data.Acceleration.Y;
            Z = data.Acceleration.Z;
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
