using Nelder_Mead_App.ViewModels;
using NelderMeadLib.Models;
using System.Windows;

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
