using Nelder_Mead_App.ViewModels;
using System.Windows;

namespace Nelder_Mead_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}