using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
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
    /// Логика взаимодействия для CreateSimplexDialog.xaml
    /// </summary>
    public partial class CreateSimplexDialog : ContentDialog
    {
        public IReadOnlyCollection<NelderMeadLib.Models.Point> Simplex { get; private set; }

        public CreateSimplexDialog(int points, int arguments)
        {
            InitializeComponent();
            DataContext = new CreateSimplexDialogViewModel(points, arguments);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Simplex = (DataContext as CreateSimplexDialogViewModel).Simplex;
        }
    }
}
