using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NelderMeadLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelder_Mead_App.ViewModels;

public partial class SettingsWindowViewModel : ObservableObject
{
    [ObservableProperty]
    AlgorithmParameters algorithmParameters;

    [ObservableProperty]
    bool isSaveButtonEnabled = true;

    public Action<AlgorithmParameters> SaveAction;

    public SettingsWindowViewModel(AlgorithmParameters? _parameters = null)
    {
        AlgorithmParameters = new AlgorithmParameters();

        if (_parameters != null)
        {
            AlgorithmParameters.MaxIterations = _parameters.MaxIterations;
            AlgorithmParameters.SolutionPrecision = _parameters.SolutionPrecision;
            AlgorithmParameters.ExpansionCoef = _parameters.ExpansionCoef;
            AlgorithmParameters.ReflectionCoef = _parameters.ReflectionCoef;
            AlgorithmParameters.ContractionCoef = _parameters.ContractionCoef;
            AlgorithmParameters.ShrinkCoef = _parameters.ShrinkCoef;
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveAction?.Invoke(AlgorithmParameters);
    }

    [RelayCommand]
    private void DisableSaveButton() => IsSaveButtonEnabled = false;
}
