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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BalanceTuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM _Vm = new MainVM();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _Vm;
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            _Vm.Stop();
        }
    }
}
