using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using Nelder_Mead_App.Dialogs;
using NelderMeadLib;
using NelderMeadLib.Exceptions;
using NelderMeadLib.Interfaces;
using NelderMeadLib.Models;
using NelderMeadLib.Realisations;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Nelder_Mead_App.ViewModels;

public record MyLine()
{
    public double X1 { get; set; }
    public double X2 { get; set; }
    public double Y1 { get; set; }
    public double Y2 { get; set; }
}


public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    FlowDocument logDocument;

    [ObservableProperty]
    Visibility logVisibility = Visibility.Collapsed;

    [ObservableProperty]
    AlgorithmParameters algorithmParameters;

    [ObservableProperty]
    bool isAlgorithmRunning = false;

    [ObservableProperty]
    string solutionValue = "";

    [ObservableProperty]
    string solutionCoordinates = "";

    [ObservableProperty]
    string function = "";

    bool isCorrectInput;

    private NelderMeadAlgorithmBuilder _algorithmBuilder;

    private CancellationTokenSource _cts;

    public MainWindowViewModel()
    {
        _cts = new CancellationTokenSource();

        algorithmParameters = new();
        logDocument = new FlowDocument();

        _algorithmBuilder = new NelderMeadAlgorithmBuilder();
        _algorithmBuilder.UseFlowDocumentLogger(logDocument);

        PropertyChanged += MainWindowViewModel_PropertyChanged;
    }

    private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Function))
        {
            try
            {
                var func = new Function(Function);
                isCorrectInput = true;
            }
            catch (CantParseExpressionException ex)
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
            _algorithmBuilder.SetFunction(Function)
                .SetMaxIterations(AlgorithmParameters.MaxIterations)
                .SetExpansionCoef(AlgorithmParameters.ExpansionCoef)
                .SetContractionCoef(AlgorithmParameters.ContractionCoef)
                .SetShrinkCoef(AlgorithmParameters.ShrinkCoef)
                .SetReflectionCoef(AlgorithmParameters.ReflectionCoef)
                .SetSolutionPrecision(AlgorithmParameters.SolutionPrecision);

            var algorithm = _algorithmBuilder.Build();

            Simplex? simplex = null;
            if (AlgorithmParameters.UseUserSimplex)
            {
                simplex = await GetUserSimplex(algorithm);
            }

            if (AlgorithmParameters.UseUserSimplex && simplex is null)
            {
                return;
            }

            try
            {
                IsAlgorithmRunning = true;
                var res = await algorithm.RunAsync(simplex, _cts.Token);
                SolutionValue = res.Value.ToString();
                SolutionCoordinates = Helpers.FormatCoordinates(res.Coordinates);
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsAlgorithmRunning = false;
            }
        }
        else
        {
            await Helpers.DisplayMessageDialog("Проверьте правильность ввода", "Ошибка");
        }
    }

    private async Task<Simplex?> GetUserSimplex(INelderMeadAlgorithm algorithm)
    {
        var simplexDialog = new CreateSimplexDialog(
                        algorithm.GetSimplexSize(),
                        algorithm.GetArgumentsNumber());
        await ContentDialogMaker.CreateContentDialogAsync(simplexDialog, true);
        if (ContentDialogMaker.Result == ContentDialogResult.Primary)
        {
            return algorithm.CreateSimplex(simplexDialog.Simplex.ToArray());
        }

        return null;
    }

    [RelayCommand]
    private async Task BreakAlgorithmRun()
    {
        if (IsAlgorithmRunning)
        {
            var result = await Helpers.DisplayQuestionDialog(
                "Прервать вычисление?",
                "Вопрос",
                "Прервать",
                "Отмена");
            if (result == ContentDialogResult.Primary)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            return;
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
    private void ShowHideLog()
    {
        if (LogVisibility == Visibility.Visible)
        {
            LogVisibility = Visibility.Collapsed;
        }
        else
        {
            LogVisibility = Visibility.Visible;
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
            Debug.WriteLine(ex);
            await Helpers.DisplayMessageDialog("Что-то пошло не так...", "Ошибка");
        }
    }
}
