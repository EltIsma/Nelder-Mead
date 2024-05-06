using System.Globalization;
using System.Windows.Controls;

namespace Nelder_Mead_App.Validations;

public class DoubleRangeValidationRule : ValidationRule
{
    public double Min { get; set; } = double.NegativeInfinity;
    public double Max { get; set; } = double.PositiveInfinity;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        double val = 0;
        try
        {
            val = double.Parse((string)value, cultureInfo);
        }
        catch
        {
            return new ValidationResult(false,
                "Введите число с плавающей точкой");
        }

        if ((val <= Min) || (val > Max))
        {
            return new ValidationResult(false,
              $"Введите число в диапазоне: {Min}-{Max}.");
        }

        return ValidationResult.ValidResult;
    }
}
