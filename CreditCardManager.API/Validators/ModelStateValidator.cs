using System.ComponentModel.DataAnnotations;

namespace CreditCardManager.Validators
{
    public static class ModelStateValidator
    {
        public static List<ValidationResult> Validate<T>(T model)
        {
            List<ValidationResult> results = new();

            if (model == null)
            {
                results.Add(new ValidationResult("Model cannot be null."));
                return results;
            }

            ValidationContext context = new(model);

            Validator.TryValidateObject(model, context, results);

            return results;
        }
    }
}