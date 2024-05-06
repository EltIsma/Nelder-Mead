using System.Globalization;
using System.Windows.Controls;

namespace Nelder_Mead_App.Validations;

public class IntRangeValidationRule : ValidationRule
{
    public int Min { get; set; } = int.MinValue;
    public int Max { get; set; } = int.MaxValue;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        int val = 0;
        try
        {
            val = int.Parse((string)value);
        }
        catch
        {
            return new ValidationResult(false,
                "Введите целое число точкой");
        }

        if ((val < Min) || (val > Max))
        {
            return new ValidationResult(false,
              $"Введите число в диапазоне: {Min}-{Max}.");
        }

        return ValidationResult.ValidResult;
    }
}
