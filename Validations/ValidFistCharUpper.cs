using System.ComponentModel.DataAnnotations;

namespace apiVS.Validations
{
    //Validation level attribute
    public class ValidFistCharUpper : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var character = value.ToString()[0].ToString();
            if (character != character.ToUpper())
            {
                return new ValidationResult("La primer letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
