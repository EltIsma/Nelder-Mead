using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
using Nelder_Mead_App.Models;
using NelderMeadLib;
using NelderMeadLib.Exceptions;
using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;
using NelderMeadLib.Realisations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Nelder_Mead_App.ViewModels;

public partial class MainWindowViewModel :  ObservableObject
{
    [ObservableProperty]
    FlowDocument logDocument;

    [ObservableProperty]
    AlgorithmParameters algorithmParameters;

    [ObservableProperty]
    string function;

    bool isCorrectInput;

    private NelderMeadAlgorithmBuilder _algorightmBuilder;

    public MainWindowViewModel()
    {
        algorithmParameters = new();
        logDocument = new FlowDocument();

        _algorightmBuilder = new NelderMeadAlgorithmBuilder();
        _algorightmBuilder.UseFlowDocumentLogger(logDocument);

        PropertyChanged += MainWindowViewModel_PropertyChanged;
    }

    private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(Function))
        {
            try
            {
                var func = new Function(Function);
                isCorrectInput = true;
            }
            catch(CantParseExpressionException ex)
            {
                isCorrectInput = false;
                Debug.WriteLine(ex);
            }
        }
    }

    [RelayCommand]
    private async Task RunAlgorithm()
    {
        if (isCorrectInput)
        {
            _algorightmBuilder.SetFunction(Function)
                .SetMaxIterations(AlgorithmParameters.MaxIterations)
                .SetExpansionCoef(AlgorithmParameters.ExpansionCoef)
                .SetContractionCoef(AlgorithmParameters.ContractionCoef)
                .SetShrinkCoef(AlgorithmParameters.ShrinkCoef)
                .SetReflectionCoef(AlgorithmParameters.ReflectionCoef)
                .SetSolutionPrecision(AlgorithmParameters.SolutionPrecision);

            var algorithm = _algorightmBuilder.Build();

            if (AlgorithmParameters.UseUserSimplex)
            {
                var simplexDialog = new CreateSimplexDialog(
                    algorithm.GetSimplexSize(),
                    algorithm.GetArgumentsNumber());
                await ContentDialogMaker.CreateContentDialogAsync(simplexDialog, true);
                if (ContentDialogMaker.Result == ContentDialogResult.Primary)
                {
                    await algorithm.RunAsync(algorithm.CreateSimplex(simplexDialog.Simplex.ToArray()), default);
                }
                else
                {
                    return;
                }
            }
            else
            {
                await algorithm.RunAsync(default);
            }
        }
        else
        {
            await Helpers.DisplayMessageDialog("Проверьте правильность ввода", "Ошибка");
        }
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var settings = new SettingsWindow(AlgorithmParameters);
        if (settings.ShowDialog() == true)
        {
            AlgorithmParameters = settings.ResultParameters;
        }
    }

    [RelayCommand]
    private void ClearLog()
    {
        LogDocument.Blocks.Clear();
    }

    [RelayCommand]
    private async Task SaveLog()
    {
        try
        {
            TextRange range;
            System.IO.FileStream fStream;
            range = new TextRange(LogDocument.ContentStart, LogDocument.ContentEnd);
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = $"Log-{Function}";
            saveFile.Filter = "txt file(*.txt) | *.txt";
            if (saveFile.ShowDialog() == true)
            {
                fStream = new System.IO.FileStream(saveFile.FileName, System.IO.FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
            }

        }
        catch (Exception ex)
        {
            await Helpers.DisplayMessageDialog("Что-то пошло не так...", "Ошибка");
        }
    }
}
