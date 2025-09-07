using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.Validators
{
    [AttributeUsage(AttributeTargets.Property |
  AttributeTargets.Field, AllowMultiple = false)]
    public sealed class OnlyFutureDate : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture,
                $"The field {name} must be a future date.");
        }
    }


}