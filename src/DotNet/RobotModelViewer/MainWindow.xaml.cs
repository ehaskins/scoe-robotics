using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Scoe.Robot.Card.Interface.Arduino;
using Scoe.Robot.Card.RobotModel;

namespace RobotModelViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ArduinoInterface _Arduino;
        private CardModelBase _Model;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _Arduino = new ArduinoInterface("COM7", 115200, 20);
            _Model = new CardModelBase(_Arduino);
            _Model.AnalogInputs.Add(new Scoe.Robot.Shared.RobotModel.AnalogInput() { Pin = 0, Value=512});
            _Arduino.Model = _Model; 
            _Arduino.Start();
            this.DataContext = _Model;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _Arduino.Stop();
        }
    }
}
