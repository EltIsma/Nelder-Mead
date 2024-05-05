using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nelder_Mead_App.Validations;

public class StringToCoordinatesValidationRule : ValidationRule
{
    public int Arguments { get; set; } = 0;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        try
        {
            var split = ((string)value).Split(';');
            if(split.Length < Arguments)
            {
                return new ValidationResult(false, "Недостаточное число координат");
            }
            if (split.Length > Arguments)
            {
                return new ValidationResult(false, "Слишком большое число координат");
            }

            foreach(var item in split)
            {
                double.Parse(item);
            }
        }
        catch
        {
            return new ValidationResult(false,
                "Координаты должны быть числами с плавающей запятой");
        }

        return ValidationResult.ValidResult;
    }
}
