using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORTInternationalHotel.Web.Models
{
    public class Habitacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio.")]
        [StringLength(10)]
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo de habitación")]
        public TipoHabitacion Tipo { get; set; }

        [Range(1, 10, ErrorMessage = "La capacidad debe estar entre 1 y 10 personas.")]
        public int Capacidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 100000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Display(Name = "Precio por noche (USD)")]
        public decimal PrecioPorNoche { get; set; }

        // Propiedad relacional (futura Foreign Key): NombreModelo + Id
        [Required]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        // Navegación: una Habitación puede tener muchas Reservas (en distintas fechas)
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
