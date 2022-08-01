using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiVS.Models
{
    public class Author : IValidatableObject
    {
        [Required(ErrorMessage = "El campo es requerido")]
        public int Id { get; set; }

        //[StringLength(maximumLength: 5, MinimumLength = 2, ErrorMessage = "El nombre debe de contener entre {2} y {1} caracteres")]
        //[ValidFistCharUpper] level attribute
        public string Name { get; set; }
        public List<AuthorsBooks> AuthorsBooks { get; set; }


        //Validations level model
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var character = Name[0].ToString();
                if (character != character.ToUpper())
                {
                    yield return new ValidationResult("", new string[] { nameof(Name)});
                }

            }
            //Puedo agregar otra validacion
        }
    }
}
