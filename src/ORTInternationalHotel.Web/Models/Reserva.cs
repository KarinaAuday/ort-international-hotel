using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORTInternationalHotel.Web.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de ingreso")]
        public DateTime FechaDesde { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de egreso")]
        [FechaPosteriorA(nameof(FechaDesde), MinimoDias = 1,
            ErrorMessage = "La fecha de egreso debe ser al menos un día posterior a la de ingreso.")]
        public DateTime FechaHasta { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public EstadoReserva Estado { get; set; } = EstadoReserva.Pendiente;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Monto total (USD)")]
        public decimal MontoTotal { get; set; }

        // ---- Propiedades relacionales (futuras Foreign Keys): NombreModelo + Id ----

        [Required]
        [Display(Name = "Pasajero")]
        public int PasajeroId { get; set; }

        [ForeignKey(nameof(PasajeroId))]
        public Pasajero? Pasajero { get; set; }

        [Required]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        [Required]
        [Display(Name = "Habitación")]
        public int HabitacionId { get; set; }

        [ForeignKey(nameof(HabitacionId))]
        public Habitacion? Habitacion { get; set; }
    }
}
