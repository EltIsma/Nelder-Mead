using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NelderMeadLib.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nelder_Mead_App.Dialogs;

public partial class CreateSimplexDialogViewModel : ObservableObject
{
    private List<Point> simplex;
    public IReadOnlyCollection<Point> Simplex => simplex;

    [ObservableProperty]
    ObservableCollection<string> points;

    [ObservableProperty]
    string currentPoint;

    [ObservableProperty]
    int arguments;

    [ObservableProperty]
    bool isPrimaryButtonEnabled;

    [ObservableProperty]
    bool isAddButtonEnabled;

    int _points;

    public CreateSimplexDialogViewModel(int points, int arguments)
    {
        Points = new ObservableCollection<string>();
        simplex = new List<Point>();
        _points = points;
        Arguments = arguments;
        PropertyChanged += CreateSimplexDialogViewModel_PropertyChanged;
    }

    private void CreateSimplexDialogViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentPoint))
        {
            try
            {
                var point = StringToCoordinates(CurrentPoint);
                if (point.Length != Arguments)
                {
                    IsAddButtonEnabled = false;
                    return;
                }
                IsAddButtonEnabled = true;
            }
            catch
            {
                IsAddButtonEnabled = false;
            }
        }
    }

    [RelayCommand]
    private void AddPoint()
    {
        var point = StringToCoordinates(CurrentPoint);
        Points.Add(CurrentPoint);
        simplex.Add(new Point() { Coordinates = point });
        CurrentPoint = "";

        IsPrimaryButtonEnabled = Points.Count == _points;
    }

    private double[] StringToCoordinates(string str)
    {
        var split = ((string)str).Split(';');
        if (split.Length != Arguments)
        {
            return new double[0];
        }

        var result = new double[Arguments];
        int i = 0;
        foreach (var item in split)
        {
            result[i++] = double.Parse(item);
        }

        return result;
    }
}
