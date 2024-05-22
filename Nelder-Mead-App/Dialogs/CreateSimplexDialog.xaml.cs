using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
using System.Collections.Generic;

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
