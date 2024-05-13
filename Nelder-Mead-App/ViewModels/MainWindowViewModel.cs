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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
    Visibility busyIndicatorVisibility = Visibility.Visible;

    [ObservableProperty]
    Visibility visualizerVisibility = Visibility.Collapsed;

    [ObservableProperty]
    string solutionValue = "";

    [ObservableProperty]
    string solutionCoordinates = "";

    [ObservableProperty]
    string function = "";

    [ObservableProperty]
    PlotModel plotModel;

    bool isCorrectInput;

    private NelderMeadAlgorithmBuilder _algorithmBuilder;

    private CancellationTokenSource _cts;

    private CustomLogger _customLogger;

    public MainWindowViewModel()
    {
        PlotModel = new PlotModel();

        _cts = new CancellationTokenSource();

        algorithmParameters = new();
        logDocument = new FlowDocument();

        _algorithmBuilder = new NelderMeadAlgorithmBuilder();
        //_algorithmBuilder.UseFlowDocumentLogger(logDocument);
        _customLogger = new CustomLogger();
        _algorithmBuilder.SetLogger(_customLogger);

    }

    [RelayCommand]
    private async Task RunAlgorithm()
    {
        try
        {
            var func = new Function(Function);

            _algorithmBuilder.SetFunction(func)
                .SetMaxIterations(AlgorithmParameters.MaxIterations)
                .SetExpansionCoef(AlgorithmParameters.ExpansionCoef)
                .SetContractionCoef(AlgorithmParameters.ContractionCoef)
                .SetShrinkCoef(AlgorithmParameters.ShrinkCoef)
                .SetReflectionCoef(AlgorithmParameters.ReflectionCoef)
                .SetSolutionPrecision(AlgorithmParameters.SolutionPrecision);

            _customLogger.Clear();

            if (func.GetArgumentsNumber() == 2)
            {
                VisualizerVisibility = Visibility.Visible;
                BusyIndicatorVisibility = Visibility.Collapsed;
            }
            else
            {
                VisualizerVisibility = Visibility.Collapsed;
                BusyIndicatorVisibility = Visibility.Visible;
            }

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

                AddLogToFlowDocument();
                await VisualizeSteps();

                SolutionValue = res.Value.ToString();
                SolutionCoordinates = Helpers.FormatCoordinates(res.Coordinates);

                await Helpers.DisplayMessageDialog("Алгоритм завершил выполнение", "Инфо");
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsAlgorithmRunning = false;
            }
        }
        catch (CantParseExpressionException)
        {
            await Helpers.DisplayMessageDialog("Проверьте правильность ввода", "Ошибка");
        }
    }

    private void AddLogToFlowDocument()
    {
        var flowDocumentLogger = new FlowDocumentLogger(LogDocument);
        int steps = 0;
        foreach (var value in _customLogger.Steps)
        {
            flowDocumentLogger.LogStep(value, steps++);
        }
        flowDocumentLogger.LogSolution(_customLogger.Solution);
    }

    private async Task VisualizeSteps()
    {
        PlotModel.Axes.Clear();
        PlotModel.Axes.Add(new LinearAxis()
        {
            Position = AxisPosition.Bottom,
            Minimum = -10,
            Maximum = 10
        });
        PlotModel.Axes.Add(new LinearAxis()
        {
            Position = AxisPosition.Left,
            Minimum = -10,
            Maximum = 10
        });

        var func = new Function(Function);

        if (func.GetArgumentsNumber() != 2)
        {
            return;
        }
        PlotModel.Series.Clear();

        DrawFunction(func);

        var s1 = new LineSeries()
        {
            Color = OxyColors.Black,
            MarkerType = MarkerType.Circle,
            MarkerSize = 3,
            MarkerFill = OxyColors.Green,
            MarkerStrokeThickness = 1
        };

        PlotModel.Series.Add(s1);

        foreach (var value in _customLogger.Steps)
        {
            s1.Points.Add(new DataPoint(value.Highest.Coordinates[0], value.Highest.Coordinates[1]));
            s1.Points.Add(new DataPoint(value.NextHighest.Coordinates[0], value.NextHighest.Coordinates[1]));
            s1.Points.Add(new DataPoint(value.Lowest.Coordinates[0], value.Lowest.Coordinates[1]));
            s1.Points.Add(new DataPoint(value.Highest.Coordinates[0], value.Highest.Coordinates[1]));

            PlotModel.InvalidatePlot(true);
            await Task.Delay(200);

            s1.Points.Clear();
        }
    }

    private void DrawFunction(Function func)
    {
        if (func.GetArgumentsNumber() != 2)
        {
            return;
        }

        double x0 = -10;
        double x1 = 10;
        double y0 = -10;
        double y1 = 10;

        var xx = ArrayBuilder.CreateVector(x0, x1, 100);
        var yy = ArrayBuilder.CreateVector(y0, y1, 100);
        var zz = ArrayBuilder.CreateVector(-10, 10, 10);

        var peaksData = new double[100, 100];
        for (int i = 0; i < 100; i++)
            for (int j = 0; j < 100; j++)
                peaksData[i, j] = func.Calculate(new double[] { xx[i], yy[j] });

        var cs = new ContourSeries
        {
            ColumnCoordinates = xx,
            RowCoordinates = yy,
            Data = peaksData,
            ContourLevels = zz
        };

        PlotModel.Series.Add(cs);
        PlotModel.InvalidatePlot(true);
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
