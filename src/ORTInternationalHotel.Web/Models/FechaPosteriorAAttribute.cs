using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ORTInternationalHotel.Web.Models
{
    // Valida que la fecha de esta propiedad sea al menos MinimoDias posterior a otra propiedad de la misma clase.
    // Se ejecuta en el servidor (IsValid) y tambien en el navegador (AddValidation + wwwroot/js/validaciones.js).
    [AttributeUsage(AttributeTargets.Property)]
    public class FechaPosteriorAAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PropiedadFechaInicial { get; }
        public int MinimoDias { get; set; } = 1;

        public FechaPosteriorAAttribute(string propiedadFechaInicial)
        {
            PropiedadFechaInicial = propiedadFechaInicial;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime fechaFinal)
            {
                return ValidationResult.Success;
            }

            var propiedad = validationContext.ObjectType.GetProperty(PropiedadFechaInicial);
            if (propiedad?.GetValue(validationContext.ObjectInstance) is not DateTime fechaInicial)
            {
                return ValidationResult.Success;
            }

            if ((fechaFinal.Date - fechaInicial.Date).TotalDays < MinimoDias)
            {
                return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-fechaposteriora"] = ErrorMessage ?? "Fecha invalida.";
            context.Attributes["data-val-fechaposteriora-propiedad"] = PropiedadFechaInicial;
            context.Attributes["data-val-fechaposteriora-dias"] = MinimoDias.ToString();
        }
    }
}
