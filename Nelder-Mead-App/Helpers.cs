using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelder_Mead_App;

public static class Helpers
{
    public static async Task<ContentDialogResult> DisplayMessageDialog(string message, string header)
    {
        MyContentDialog dialog = new MyContentDialog()
        {
            Content = message,
            PrimaryButtonText = "Ок",
            SecondaryButtonText = "Отмена",
            Title = header
        };
        await ContentDialogMaker.CreateContentDialogAsync(dialog, true);
        return ContentDialogMaker.Result;
    }
}
