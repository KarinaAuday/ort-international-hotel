using System.ComponentModel.DataAnnotations;

namespace ORTInternationalHotel.Web.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la sede es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre de la sede")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es obligatorio.")]
        [StringLength(60)]
        public string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(60)]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Direccion { get; set; }

        [Range(1, 10, ErrorMessage = "La cantidad de estrellas debe estar entre 1 y 10.")]
        [Display(Name = "Estrellas")]
        public int CantidadEstrellas { get; set; }

        // Navegación: un Hotel tiene muchas Habitaciones
        public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();

        // Navegación: un Hotel tiene muchas Reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
