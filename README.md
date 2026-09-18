# ORT International Hotel — Ejemplo MVC (Model First + Scaffolding)

Proyecto de ejemplo para la cátedra: una cadena de hoteles ficticia ("ORT International
Hotel", al estilo Sheraton) con sedes en Buenos Aires, Bahamas y Madrid. Se usa para enseñar
el flujo completo de **ASP.NET Core MVC + Entity Framework Core**, desde el modelo de datos
hasta un CRUD (ABM) funcionando contra SQL Server, pasando por migraciones y scaffolding.

- **Framework**: ASP.NET Core MVC (.NET 8)
- **ORM**: Entity Framework Core 8 (Code First / *Model First* desde el punto de vista del
  alumno: se arranca dibujando el modelo relacional y de ahí se generan las clases)
- **Base de datos**: SQL Server LocalDB
- **Patrón de trabajo**: Modelo → Migración (`Add-Migration` / `dotnet ef migrations add`) →
  Base de datos (`Update-Database` / `dotnet ef database update`) → Scaffolding de
  Controllers + Views (ABM)

---

## 1. El dominio: 4 entidades relacionadas

| Entidad | Descripción |
|---|---|
| **Hotel** | Una sede de la cadena (Buenos Aires, Bahamas, Madrid, etc.) |
| **Pasajero** | Un huésped que puede reservar en cualquier sede de la cadena |
| **Habitación** | Pertenece a **un** Hotel. Tiene tipo, capacidad y precio por noche |
| **Reserva** | La entidad "puente": une a un Pasajero con un Hotel y una Habitación en un rango de fechas |

Un pasajero puede tener **varias reservas**, en **distintas fechas**, en **distintos
hoteles** de la cadena. Por eso `Reserva` es la entidad central del modelo, con tres claves
foráneas.

### 1.1 Diagrama Entidad-Relación

![Diagrama entidad-relación de ORT International Hotel](docs/diagrama-er.svg)

> **Convención de nombres usada en el modelo**: toda clave primaria se llama `Id`. Toda
> clave foránea se llama `NombreDeLaEntidad` + `Id` (por ejemplo, en `Reserva`:
> `HotelId`, `PasajeroId`, `HabitacionId`). Esto es lo que Entity Framework espera "por
> convención" para descubrir automáticamente las relaciones sin tener que configurarlas
> a mano con Fluent API.

### 1.2 Multiplicidad de las relaciones

- Un **Hotel** tiene muchas **Habitaciones** (1 a N)
- Un **Hotel** tiene muchas **Reservas** (1 a N)
- Un **Pasajero** tiene muchas **Reservas** (1 a N)
- Una **Habitación** tiene muchas **Reservas** en distintas fechas (1 a N)
- Una **Reserva** pertenece a exactamente un Hotel, un Pasajero y una Habitación

---

## 2. Estructura del proyecto

```
MVC-Ejemplo/
├── ORTInternationalHotel.sln
├── README.md
└── src/
    └── ORTInternationalHotel.Web/
        ├── Models/
        │   ├── Hotel.cs
        │   ├── Pasajero.cs
        │   ├── Habitacion.cs
        │   ├── Reserva.cs
        │   ├── TipoHabitacion.cs      (enum)
        │   ├── EstadoReserva.cs       (enum)
        │   └── FechaPosteriorAAttribute.cs   (validación propia, ver Paso 4b)
        ├── Data/
        │   └── ORTInternationalHotelContext.cs   (DbContext + datos semilla)
        ├── Migrations/                            (generadas por EF Core)
        ├── Controllers/
        │   ├── HotelesController.cs
        │   ├── PasajerosController.cs
        │   ├── HabitacionesController.cs
        │   └── ReservasController.cs
        ├── Views/
        │   ├── Hoteles/       (Index, Create, Edit, Details, Delete)
        │   ├── Pasajeros/     (Index, Create, Edit, Details, Delete)
        │   ├── Habitaciones/  (Index, Create, Edit, Details, Delete)
        │   └── Reservas/      (Index, Create, Edit, Details, Delete)
        ├── appsettings.json    (connection string a LocalDB)
        └── Program.cs
```

Este repositorio **ya incluye** el modelo, la migración inicial y el scaffolding
generado, para que puedan ver el resultado final. La Parte 3 explica cómo se hizo, paso a
paso, para que lo puedan reproducir ustedes mismos desde cero (por ejemplo, en un proyecto
en blanco durante la clase).

---

## 3. Guía paso a paso para los alumnos (Model First → CRUD)

### Paso 0 — Requisitos

- Visual Studio 2022 (17.8+) con la carga de trabajo **ASP.NET y desarrollo web**
- SQL Server Express LocalDB (se instala junto con Visual Studio)
- .NET 8 SDK

### Paso 1 — Crear el proyecto

En Visual Studio: **Archivo → Nuevo → Proyecto → Aplicación web ASP.NET Core (Modelo-Vista-Controlador)**.
Elegir .NET 8, sin autenticación.

Equivalente en consola (lo que se usó para armar este repo):

```bash
dotnet new mvc -n ORTInternationalHotel.Web
```

### Paso 2 — Instalar los paquetes NuGet de Entity Framework Core

Desde la **Consola del Administrador de paquetes** (`Tools → NuGet Package Manager →
Package Manager Console`) o por CLI:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

- `SqlServer`: el proveedor de base de datos.
- `Tools`: habilita `Add-Migration` / `Update-Database` desde la Package Manager Console.
- `Design`: habilita los mismos comandos desde la CLI (`dotnet ef`).

### Paso 3 — Diseñar el diagrama relacional (a mano, en papel o en un editor)

Antes de escribir una sola clase, dibujen el diagrama como el de la sección 1.1: entidades,
atributos, y para cada relación identifiquen **quién tiene la clave foránea**. Esta es la
parte de "Model First" que hacemos nosotros como diseñadores, aunque después el código lo
escribamos a mano (a diferencia del "Database First", donde el diagrama se dibuja en una
herramienta como el EF Designer y esta genera las clases automáticamente).

Regla práctica para pasar del diagrama al código:

1. Cada entidad del diagrama → una clase en `Models/`.
2. Cada atributo → una propiedad pública.
3. Cada relación "muchos a uno" (ej: "una Reserva tiene un Hotel") → en el lado "muchos"
   (Reserva) se agrega una **propiedad relacional** `int HotelId` y una propiedad de
   navegación `Hotel? Hotel`.
4. En el lado "uno" (Hotel) se agrega una **colección de navegación**:
   `ICollection<Reserva> Reservas`.

#### ¿Qué es la "propiedad relacional" (`HotelId`)?

Es el campo que en la base de datos se va a convertir en la **Foreign Key (FK)**: un número
entero que guarda el `Id` del hotel al que pertenece esa habitación o esa reserva. Por
ejemplo, si `Habitacion.HotelId = 1`, esa habitación es del hotel cuyo `Id` es 1
(Buenos Aires). Es exactamente lo mismo que en SQL sería una columna `HotelId INT` con una
restricción `FOREIGN KEY` apuntando a la tabla `Hoteles`.

La propiedad `Hotel? Hotel` (con mayúscula, sin `Id`) que va al lado es la **propiedad de
navegación**: no se guarda como columna en la base de datos, es una comodidad de C# para
que, desde código, puedan escribir `habitacion.Hotel.Nombre` y que EF Core traiga
automáticamente el hotel relacionado (por detrás, hace el `JOIN` por ustedes).

#### ¿Qué es `ICollection<Reserva>`?

Para lo que van a usar en este curso, **piensen `ICollection<Reserva>` como si fuera un
`List<Reserva>`**: es una lista de reservas. De hecho, en el código la inicializamos
literalmente con una lista: `new List<Reserva>()`.

La diferencia es que `ICollection<T>` es una **interfaz** (un "contrato" más genérico que
`List<T>`) que dice "esto es una colección a la que puedo agregar y quitar elementos y
recorrer con `foreach`", sin comprometerse a qué tipo de colección es exactamente por
dentro (podría ser una `List<T>`, un `HashSet<T>`, etc.). Entity Framework Core pide que
las colecciones de navegación se declaren con `ICollection<T>` (o similares) en vez de
`List<T>` porque así, cuando ustedes escriben `hotel.Habitaciones`, es EF quien decide
internamente cómo traer y gestionar esa lista de la base de datos — pero para todo lo que
hacen en clase (recorrerla con `foreach`, contarla con `.Count`, etc.) se comporta igual
que cualquier lista que ya conocen.

En resumen:

| Se ve en el modelo | Qué es | ¿Existe como columna en la BD? |
|---|---|---|
| `int HotelId` | Propiedad relacional / futura FK | Sí — es la columna con la clave foránea |
| `Hotel? Hotel` | Propiedad de navegación (1 solo objeto relacionado) | No — la arma EF Core al consultar |
| `ICollection<Reserva> Reservas` | Colección de navegación (lista de objetos relacionados) | No — la arma EF Core al consultar |

### Paso 4 — Crear las clases de modelo

Ejemplo simplificado de `Habitacion.cs` (el proyecto real usa Data Annotations para
validaciones, ver el archivo completo en [`Models/Habitacion.cs`](src/ORTInternationalHotel.Web/Models/Habitacion.cs)):

```csharp
public class Habitacion
{
    public int Id { get; set; }
    public string Numero { get; set; }
    public TipoHabitacion Tipo { get; set; }
    public int Capacidad { get; set; }
    public decimal PrecioPorNoche { get; set; }

    // Propiedad relacional (futura Foreign Key): NombreDelModelo + Id
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
```

Repitan este análisis para `Hotel`, `Pasajero` y `Reserva` (la entidad `Reserva` va a tener
**tres** propiedades FK: `HotelId`, `PasajeroId`, `HabitacionId`).

### Paso 4b — Validaciones con Data Annotations (servidor y navegador)

Los atributos entre corchetes que van arriba de cada propiedad (`[Required]`, `[StringLength]`,
`[Range]`, etc.) se llaman **Data Annotations**. Con una sola declaración en el modelo se
consiguen **tres cosas**:

1. **Validación en el servidor**: el controller la aplica al recibir el formulario. Si algo
   no cumple, `ModelState.IsValid` es `false` y el `Create`/`Edit` vuelve a mostrar el
   formulario con los errores en vez de guardar.
2. **Validación en el navegador (frontend)**: los tag helpers de las vistas
   (`<input asp-for="Nombre" />` y `<span asp-validation-for="Nombre">`) leen las anotaciones
   y generan atributos HTML `data-val-*`. Los scripts **jQuery Validation** y **jQuery
   Validation Unobtrusive** (los que carga `Views/Shared/_ValidationScriptsPartial.cshtml`
   dentro de `@section Scripts`) leen esos atributos y muestran el error **sin recargar la
   página**. Si falta ese partial en una vista, el formulario sigue validando, pero solo en
   el servidor.
3. **Estructura de la base de datos**: algunas anotaciones también las usa EF Core al crear la
   migración (`[StringLength(100)]` → columna `nvarchar(100)`, `[Required]` → columna
   `NOT NULL`, `[Column(TypeName = "decimal(10,2)")]` → precisión del decimal).

Por ejemplo, esta propiedad de `Pasajero`:

```csharp
[Required(ErrorMessage = "El email es obligatorio.")]
[EmailAddress(ErrorMessage = "El formato de email no es válido.")]
[StringLength(120)]
public string Email { get; set; } = string.Empty;
```

se renderiza en el HTML así (se puede ver con "Inspeccionar" en el navegador):

```html
<input type="email" name="Email" data-val="true"
       data-val-required="El email es obligatorio."
       data-val-email="El formato de email no es válido."
       data-val-length-max="120" />
```

Anotaciones usadas en este proyecto:

| Anotación | Qué valida | Dónde se usa |
|---|---|---|
| `[Required]` | El campo no puede quedar vacío | Nombre, Apellido, Email, fechas, etc. |
| `[StringLength(n)]` | Largo máximo de texto | Nombre, Documento, Email, etc. |
| `[Range(min, max)]` | Valor numérico dentro de un rango | `CantidadEstrellas`, `Capacidad`, `PrecioPorNoche` |
| `[EmailAddress]` | Formato de email | `Pasajero.Email` |
| `[Phone]` | Formato de teléfono | `Pasajero.Telefono` |
| `[Display(Name = "...")]` | Texto de la etiqueta en el formulario (no valida) | Todas las propiedades con nombre "lindo" |
| `[FechaPosteriorA(...)]` | Validación propia (ver más abajo) | `Reserva.FechaHasta` |

> **Importante**: la validación del navegador es una comodidad para el usuario, pero **no
> es segura**: cualquiera puede desactivar JavaScript o enviar el formulario a mano. Por eso
> el servidor **siempre** vuelve a validar. Nunca se debe confiar solo en el frontend.

#### Validación propia: la fecha de egreso debe ser al menos un día posterior al ingreso

Las anotaciones de .NET no traen una para "esta fecha debe ser posterior a esa otra", así que
se crea una propia heredando de `ValidationAttribute`
([`Models/FechaPosteriorAAttribute.cs`](src/ORTInternationalHotel.Web/Models/FechaPosteriorAAttribute.cs)) y se usa como cualquier otra:

```csharp
[FechaPosteriorA(nameof(FechaDesde), MinimoDias = 1,
    ErrorMessage = "La fecha de egreso debe ser al menos un día posterior a la de ingreso.")]
public DateTime FechaHasta { get; set; }
```

La clase tiene dos partes:

- **`IsValid(...)`**: la validación del **servidor**. Compara `FechaHasta` con `FechaDesde` y
  falla si la diferencia es menor a `MinimoDias`.
- **`AddValidation(...)`** (interfaz `IClientModelValidator`): agrega al HTML los atributos
  `data-val-fechaposteriora-*`. Como esta regla es nueva, jQuery Validation no la conoce, así
  que hace falta un pequeño script que la enseñe:
  [`wwwroot/js/validaciones.js`](src/ORTInternationalHotel.Web/wwwroot/js/validaciones.js)
  (se carga desde `_ValidationScriptsPartial.cshtml`).

Con eso, al elegir fechas iguales o invertidas, el error aparece al instante debajo del campo,
y aunque se saltee el JavaScript el servidor lo rechaza igual.

### Paso 5 — Crear el `DbContext`

El `DbContext` es la clase que representa la conexión a la base de datos y expone un
`DbSet<T>` por cada entidad (equivalente a una tabla):

```csharp
public class ORTInternationalHotelContext : DbContext
{
    public ORTInternationalHotelContext(DbContextOptions<ORTInternationalHotelContext> options)
        : base(options) { }

    public DbSet<Hotel> Hoteles { get; set; }
    public DbSet<Pasajero> Pasajeros { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
}
```

Ver el archivo completo en
[`Data/ORTInternationalHotelContext.cs`](src/ORTInternationalHotel.Web/Data/ORTInternationalHotelContext.cs):
además define con Fluent API el comportamiento de borrado (`OnDelete`) de cada FK de
`Reserva`, y carga datos de ejemplo con `HasData` (seed).

### Paso 6 — Registrar el `DbContext` y el connection string

En `appsettings.json`:

```json
"ConnectionStrings": {
  "ORTInternationalHotelContext": "Server=(localdb)\\mssqllocaldb;Database=ORTInternationalHotelDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

En `Program.cs`, antes de `builder.Build()`:

```csharp
builder.Services.AddDbContext<ORTInternationalHotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ORTInternationalHotelContext")));
```

### Paso 7 — Crear la migración inicial (`Add-Migration`)

Una **migración** es el "diff" entre el modelo de clases (C#) y el esquema real de la base
de datos. EF Core la traduce a SQL automáticamente.

**Desde Visual Studio** (Package Manager Console, con el proyecto Web seleccionado como
"Default project"):

```powershell
Add-Migration InitialCreate
```

**Desde la terminal** (lo que se usó en este repo):

```bash
dotnet ef migrations add InitialCreate -o Migrations
```

Esto genera 3 archivos en `Migrations/`:

- `..._InitialCreate.cs`: el método `Up()` (qué SQL correr para crear las tablas) y
  `Down()` (cómo deshacerlo).
- `..._InitialCreate.Designer.cs`: metadata que usa EF internamente.
- `ORTInternationalHotelContextModelSnapshot.cs`: una "foto" del modelo actual, para que la
  próxima migración sepa qué cambió.

**Importante para explicar en clase**: en este punto **todavía no existe la base de
datos**. Solo se generó el script en C#/SQL que la va a crear.

### Paso 8 — Aplicar la migración (`Update-Database`)

**Desde Visual Studio**:

```powershell
Update-Database
```

**Desde la terminal**:

```bash
dotnet ef database update
```

Esto sí crea la base `ORTInternationalHotelDb` en LocalDB, con las 4 tablas, sus claves
foráneas, índices, y los datos de ejemplo (seed). Pueden verificarlo en Visual Studio desde
**View → SQL Server Object Explorer → (localdb)\MSSQLLocalDB → Databases**.

> Cuando en una clase futura agreguen una propiedad nueva a un modelo (por ejemplo,
> `Pasajero.FechaNacimiento`), el flujo se repite: `Add-Migration AgregarFechaNacimiento`
> y después `Update-Database`. Nunca se edita la base de datos a mano.

### Paso 9 — Scaffolding: generar el ABM (CRUD) automáticamente

Con el modelo y la base ya creados, Visual Studio puede **generar automáticamente** el
Controller y las 5 Vistas (Index, Create, Edit, Details, Delete) de cada entidad.

**Desde Visual Studio**: clic derecho sobre la carpeta `Controllers` → **Agregar →
Controlador nuevo → Controlador MVC con vistas, usando Entity Framework**. Elegir el
modelo (por ejemplo `Reserva`) y el `DbContext` (`ORTInternationalHotelContext`).

**Desde la terminal** (herramienta `dotnet-aspnet-codegenerator`, lo que se usó en este
repo):

```bash
dotnet tool install -g dotnet-aspnet-codegenerator

dotnet aspnet-codegenerator controller \
  -name ReservasController \
  -m ORTInternationalHotel.Web.Models.Reserva \
  -dc ORTInternationalHotel.Web.Data.ORTInternationalHotelContext \
  -outDir Controllers \
  --useDefaultLayout \
  --referenceScriptLibraries
```

Repitan el comando para `Hotel`, `Pasajero` y `Habitacion`. El generador es lo bastante
inteligente para detectar las propiedades FK (`HotelId`, `PasajeroId`, `HabitacionId`) y
arma automáticamente `<select>` (dropdowns) en las vistas `Create`/`Edit`, poblados con
`ViewBag.HotelId = new SelectList(_context.Hoteles, "Id", "Nombre")`, etc.

#### Ajuste manual necesario: los `enum` salen con el combo vacío

Con las propiedades de tipo `enum` (en este proyecto `Reserva.Estado` y `Habitacion.Tipo`)
el generador **no completa las opciones**. En las vistas `Create.cshtml` y `Edit.cshtml`
deja un `<select>` sin contenido:

```html
<select asp-for="Estado" class="form-control"></select>
```

El resultado es un combo vacío: no se puede elegir ni editar el valor. Se soluciona
agregando `asp-items` con `Html.GetEnumSelectList<T>()`, que arma las opciones a partir de
los valores del `enum`:

```html
<select asp-for="Estado" class="form-control"
        asp-items="Html.GetEnumSelectList<EstadoReserva>()"></select>
```

Hay que hacerlo en `Create.cshtml` **y** en `Edit.cshtml` de cada entidad que tenga un
`enum`:

| Vista | Cambio |
|---|---|
| `Views/Reservas/Create.cshtml` y `Edit.cshtml` | `asp-items="Html.GetEnumSelectList<EstadoReserva>()"` |
| `Views/Habitaciones/Create.cshtml` y `Edit.cshtml` | `asp-items="Html.GetEnumSelectList<TipoHabitacion>()"` |

En `Edit`, el valor guardado queda seleccionado automáticamente. Las vistas `Index`,
`Details` y `Delete` no necesitan cambios, porque muestran el nombre del `enum` sin
problemas.

#### Ajuste manual necesario: los campos `decimal` fallan con la coma decimal

Si Windows está configurado en español (Argentina), el servidor muestra los decimales con
**coma** (`600,00`), pero la validación del navegador (jQuery Validation) solo acepta
**punto**. Resultado: al editar una reserva aparece un error en el campo `MontoTotal`, y solo
se puede guardar si se borra la coma y los decimales (`600`).

La solución es fijar una cultura única para toda la aplicación en `Program.cs`, así el
servidor y el navegador coinciden en cualquier computadora:

```csharp
using System.Globalization;
using Microsoft.AspNetCore.Localization;

// ...después de var app = builder.Build();
var cultura = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});
```

Con esto los decimales se escriben con punto (`600.00`) en `MontoTotal` y `PrecioPorNoche`.

### Paso 10 — Correr la aplicación

```bash
dotnet run --project src/ORTInternationalHotel.Web
```

Y navegar a `https://localhost:xxxx/Reservas` (o cualquiera de los otros controllers). El
menú de navegación del layout ya tiene links a los 4 ABMs.

### Paso 11 (extra) — El buscador de las tablas: filtro en el navegador, sin controller

Los listados (Hoteles, Pasajeros, Habitaciones y Reservas) tienen una caja de búsqueda arriba
de la tabla. **No usa ningún controller ni consulta a la base de datos**: es JavaScript que
oculta las filas que no coinciden con lo escrito, sobre la tabla que el controller ya
entregó completa. Por eso filtra al instante y sin recargar la página.

Tiene tres piezas, las mismas en las 4 vistas `Index.cshtml` (por ejemplo
[`Views/Pasajeros/Index.cshtml`](src/ORTInternationalHotel.Web/Views/Pasajeros/Index.cshtml)):

**1. Un `<input>` que indica qué tabla filtrar**, con el atributo `data-table-search` (el valor
es el `id` de la tabla):

```html
<input type="search" class="form-control"
       placeholder="Buscar por nombre, documento o email..."
       data-table-search="tablaPasajeros" />
```

**2. La tabla con ese `id`, y cada fila marcada con `data-row`** (para que el script sepa
cuáles filas puede ocultar). Al final hay una fila oculta con el mensaje "No se encontraron...":

```html
<table id="tablaPasajeros" class="table ...">
    ...
    @foreach (var item in Model) {
        <tr data-row> ... </tr>
    }
    <tr class="empty-row is-hidden"><td colspan="7">No se encontraron pasajeros.</td></tr>
</table>
```

**3. El script**, en [`wwwroot/js/site.js`](src/ORTInternationalHotel.Web/wwwroot/js/site.js) (se
carga en todas las páginas desde `_Layout.cshtml`). Es un solo script genérico: sirve para
cualquier tabla que siga el patrón anterior. Cada vez que se escribe en la caja (evento `input`):

1. Toma el texto escrito y lo normaliza (minúsculas y sin acentos, así "fernandez" encuentra
   "Fernández").
2. Recorre las filas `tr[data-row]` de esa tabla y, si el texto de la fila contiene lo escrito,
   la deja visible; si no, le agrega la clase `is-hidden` (que en `site.css` es
   `display: none`).
3. Si no quedó ninguna fila visible, muestra la fila `empty-row`.

Fragmento central de `site.js`:

```javascript
filas.forEach(function (fila) {
    var texto = normalizar(fila.textContent || "");
    var coincide = termino === "" || texto.indexOf(termino) !== -1;
    fila.classList.toggle("is-hidden", !coincide);
});
```

> **Limitación para comentar en clase**: como filtra en el navegador, solo busca entre las
> filas que **ya se cargaron**. Con miles de pasajeros habría que hacer la búsqueda en el
> servidor: el formulario enviaría el texto al `Index` del controller (por ejemplo
> `Index(string buscar)`) y este filtraría con LINQ (`_context.Pasajeros.Where(...)`) antes de
> devolver la vista. Es un buen ejercicio para los alumnos.

### Resumen del orden Model First

```
Diagrama relacional (papel/pizarrón)
        │
        ▼
Clases de Modelo (Models/*.cs)
        │
        ▼
DbContext (Data/*.cs) + connection string
        │
        ▼
Add-Migration  (genera el script de creación de tablas)
        │
        ▼
Update-Database  (crea/actualiza la base de datos real)
        │
        ▼
Scaffolding de Controllers + Views  (genera el CRUD/ABM)
        │
        ▼
dotnet run  (¡a probar el ABM!)
```

---

## 4. Cómo correr este proyecto ya armado

```bash
# 1. Clonar el repo
git clone <url-del-repo>
cd MVC-Ejemplo

# 2. Restaurar paquetes
dotnet restore

# 3. Crear la base de datos local (si no existe todavía)
cd src/ORTInternationalHotel.Web
dotnet ef database update

# 4. Ejecutar
dotnet run
```

Abrir la URL que muestra la consola (por ejemplo `http://localhost:5080`) y navegar por
Hoteles, Pasajeros, Habitaciones y Reservas.

Si algún alumno no tiene LocalDB, puede cambiar el connection string en
`appsettings.json` por el de su instancia de SQL Server (Express, Developer, o incluso un
contenedor Docker).

---

## 5. Ideas para que los alumnos extiendan el ejemplo

- Agregar validación de **fechas superpuestas**: que no se pueda reservar la misma
  habitación en fechas que se solapan con otra reserva.
- Calcular `MontoTotal` automáticamente a partir de `PrecioPorNoche` y la cantidad de
  noches, en vez de cargarlo a mano.
- Agregar una nueva entidad `Servicio` (desayuno, spa, cochera) en relación N a N con
  `Reserva` (tabla intermedia `ReservaServicio`).
- Agregar un `Empleado` por Hotel, y mostrar en el Index de Reservas quién la gestionó.
- Pasar las vistas a Bootstrap con tarjetas (`card`) en vez de tablas simples.

Cualquier mejora que se les ocurra, es bienvenida — este proyecto está pensado como punto
de partida, no como versión final.
