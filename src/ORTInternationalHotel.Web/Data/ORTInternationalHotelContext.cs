using Microsoft.EntityFrameworkCore;
using ORTInternationalHotel.Web.Models;

namespace ORTInternationalHotel.Web.Data
{
    public class ORTInternationalHotelContext : DbContext
    {
        public ORTInternationalHotelContext(DbContextOptions<ORTInternationalHotelContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hoteles { get; set; } = default!;
        public DbSet<Pasajero> Pasajeros { get; set; } = default!;
        public DbSet<Habitacion> Habitaciones { get; set; } = default!;
        public DbSet<Reserva> Reservas { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restringimos borrado en cascada múltiple: Reserva depende de 3 entidades,
            // así que dejamos Restrict en Hotel y Pasajero y Cascade solo en Habitacion.
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Pasajero)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.PasajeroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Habitacion)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.HabitacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Datos de ejemplo (seed) ----

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { Id = 1, Nombre = "ORT International Hotel - Buenos Aires", Pais = "Argentina", Ciudad = "Buenos Aires", Direccion = "Av. Libertador 1234", CantidadEstrellas = 5 },
                new Hotel { Id = 2, Nombre = "ORT International Hotel - Bahamas", Pais = "Bahamas", Ciudad = "Nassau", Direccion = "Ocean Drive 500", CantidadEstrellas = 5 },
                new Hotel { Id = 3, Nombre = "ORT International Hotel - Madrid", Pais = "España", Ciudad = "Madrid", Direccion = "Gran Vía 88", CantidadEstrellas = 4 }
            );

            modelBuilder.Entity<Pasajero>().HasData(
                new Pasajero { Id = 1, Nombre = "Karina", Apellido = "Auday", Documento = "30111222", Email = "karina.auday@ort.edu.ar", Telefono = "+54 11 4000-1111", PaisOrigen = "Argentina" },
                new Pasajero { Id = 2, Nombre = "John", Apellido = "Smith", Documento = "US4455667", Email = "john.smith@example.com", Telefono = "+1 305-555-0110", PaisOrigen = "Estados Unidos" },
                new Pasajero { Id = 3, Nombre = "Lucía", Apellido = "Fernández", Documento = "28999111", Email = "lucia.fernandez@example.com", Telefono = "+54 11 4000-2222", PaisOrigen = "Argentina" }
            );

            modelBuilder.Entity<Habitacion>().HasData(
                new Habitacion { Id = 1, Numero = "101", Tipo = TipoHabitacion.Doble, Capacidad = 2, PrecioPorNoche = 120.00m, HotelId = 1 },
                new Habitacion { Id = 2, Numero = "102", Tipo = TipoHabitacion.Suite, Capacidad = 4, PrecioPorNoche = 260.00m, HotelId = 1 },
                new Habitacion { Id = 3, Numero = "201", Tipo = TipoHabitacion.Individual, Capacidad = 1, PrecioPorNoche = 90.00m, HotelId = 2 },
                new Habitacion { Id = 4, Numero = "202", Tipo = TipoHabitacion.Suite, Capacidad = 3, PrecioPorNoche = 340.00m, HotelId = 2 },
                new Habitacion { Id = 5, Numero = "301", Tipo = TipoHabitacion.Familiar, Capacidad = 5, PrecioPorNoche = 210.00m, HotelId = 3 }
            );

            modelBuilder.Entity<Reserva>().HasData(
                new Reserva { Id = 1, PasajeroId = 1, HotelId = 1, HabitacionId = 1, FechaDesde = new DateTime(2026, 10, 10), FechaHasta = new DateTime(2026, 10, 15), Estado = EstadoReserva.Confirmada, MontoTotal = 600.00m },
                new Reserva { Id = 2, PasajeroId = 1, HotelId = 2, HabitacionId = 3, FechaDesde = new DateTime(2026, 12, 20), FechaHasta = new DateTime(2026, 12, 27), Estado = EstadoReserva.Pendiente, MontoTotal = 630.00m },
                new Reserva { Id = 3, PasajeroId = 2, HotelId = 2, HabitacionId = 4, FechaDesde = new DateTime(2026, 11, 1), FechaHasta = new DateTime(2026, 11, 5), Estado = EstadoReserva.Confirmada, MontoTotal = 1360.00m },
                new Reserva { Id = 4, PasajeroId = 3, HotelId = 3, HabitacionId = 5, FechaDesde = new DateTime(2027, 1, 5), FechaHasta = new DateTime(2027, 1, 10), Estado = EstadoReserva.Pendiente, MontoTotal = 1050.00m }
            );
        }
    }
}
