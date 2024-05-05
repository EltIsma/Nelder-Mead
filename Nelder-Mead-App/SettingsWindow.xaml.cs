using Nelder_Mead_App.ViewModels;
using NelderMeadLib.Models;
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
using System.Windows.Shapes;

namespace Nelder_Mead_App
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public AlgorithmParameters ResultParameters { get; private set; }

        public SettingsWindow(AlgorithmParameters? parameters = null)
        {
            InitializeComponent();

            DataContext = new SettingsWindowViewModel(parameters);
            (DataContext as SettingsWindowViewModel).SaveAction += SaveParameters;
        }

        private void SaveParameters(AlgorithmParameters parameters)
        {
            ResultParameters = parameters;
            DialogResult = true;
            Close();
        }
    }
}
