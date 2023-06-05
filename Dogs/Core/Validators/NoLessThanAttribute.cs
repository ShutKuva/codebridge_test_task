using System.ComponentModel.DataAnnotations;

namespace Core.Validators
{
    public class NoLessThanAttribute : ValidationAttribute
    {
        private readonly int _number;

        public NoLessThanAttribute(int number)
        {
            _number = number;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not int)
            {
                return new ValidationResult("Value is not an integer.");
            }

            int valueInt = (int)value;

            if (valueInt < _number)
            {
                return new ValidationResult($"Number cannot be less than {_number}.");
            }

            return ValidationResult.Success;
        }
    }
}