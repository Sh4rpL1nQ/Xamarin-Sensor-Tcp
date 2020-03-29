using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel main;

        public MainWindow()
        {
            InitializeComponent();

            main = new MainViewModel();
            main.Server.OnSensorDataChanged += Server_OnSensorDataChanged;
            DataContext = main;
        }

        private void Server_OnSensorDataChanged(object sender, EventArgs e)
        {
            var sensArgs = e as SensorEventArgs;
            main.DispatcherObject?.Invoke(() =>
            {               
                cube1.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), sensArgs.X));
                cube1.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), sensArgs.Y));
                cube1.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), sensArgs.Z));
            });
            
        }
    }
}
