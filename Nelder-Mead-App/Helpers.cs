﻿using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
using System;
using System.Linq;
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

    public static async Task<ContentDialogResult> DisplayQuestionDialog(
        string message,
        string header,
        string primaryButton,
        string secondaryButton)
    {
        MyContentDialog dialog = new MyContentDialog()
        {
            Content = message,
            PrimaryButtonText = primaryButton,
            SecondaryButtonText = secondaryButton,
            Title = header
        };
        await ContentDialogMaker.CreateContentDialogAsync(dialog, true);
        return ContentDialogMaker.Result;
    }

    public static string FormatCoordinates(double[] coordinates)
    {
        return $"({String.Join("; ", coordinates.Select(c => Math.Round(c, 8)))})";
    }
}
