using System.ComponentModel.DataAnnotations;

namespace ORTInternationalHotel.Web.Models
{
    public class Pasajero
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(80)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Documento / Pasaporte")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de email no es válido.")]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El formato de teléfono no es válido.")]
        [StringLength(30)]
        public string? Telefono { get; set; }

        [StringLength(60)]
        [Display(Name = "País de origen")]
        public string? PaisOrigen { get; set; }

        // Navegación: un Pasajero puede tener muchas Reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
