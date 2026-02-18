using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.Validators
{
    public sealed class OnlyPastDate : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly date)
            {
                bool result = date <= DateOnly.FromDateTime(DateTime.Now);
                return result;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture,
                $"The field {name} must be in the past.");
        }
    }
}