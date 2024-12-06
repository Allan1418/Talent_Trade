using MongoDB.Bson;
using MongoDB.Driver;

using Talent_Trade.Models;
using Talent_Trade.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Prueba Mongo
try
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    var client = new MongoClient(connectionString);

    var database = client.GetDatabase("Talent_Hub");

    bool isMongoLive = database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

    if (!isMongoLive)
    {
        throw new TimeoutException();
    }

    Console.WriteLine($"***Conexion exitosa a la base de datos: {database.DatabaseNamespace} ***");
}
catch (Exception ex)
{
    Console.WriteLine($"***Error al conectar a la base de datos: {ex.Message} ***");
    Environment.Exit(1);
}


//ejemplo Prueba Usuario Service
Usuario nuevoUsuario = new Usuario
{
    NombreCompleto = "Juan Pérez2",
    UserName = "juanperez",
    Email = "juan.perez@example.com",
    Password = "contraseñaSegura123",
    FechaRegistro = DateTime.Now,
    // Id, ImagePerfil, IdDeCreador se asignarán automáticamente o después 
    // Suscripciones y Facturas se pueden inicializar como listas vacías o con valores
    Suscripciones = new List<string>(),
    Facturas = new List<string>()
};

IConfiguration config = builder.Configuration;
UsuarioServices usuarioService = new UsuarioServices(config);


//nuevoUsuario = usuarioService.Create(nuevoUsuario);


//usuarioService.Remove("6752af02bb165974e70076a0");


//nuevoUsuario.NombreCompleto = "aaaaaaaaaaaa";
//nuevoUsuario.Id = "67512b12ad47458b206fdd1f";

//usuarioService.Update("67512b12ad47458b206fdd1f", nuevoUsuario);



//------------------------------------------------------------------------------


//ejemplo Prueba Creador Service
Creador nuevoCreador = new Creador
{
    IdUser = "6752af25da5bad086bc3af71",
    ShortDescripcion = "Short description of the creator",
    AcercaDe = "Detailed information about the creator",
    Niveles = new List<Creador.Nivel>
    {
        new Creador.Nivel { IdNiveles = ObjectId.GenerateNewId().ToString() },
        new Creador.Nivel { IdNiveles = ObjectId.GenerateNewId().ToString() }
    }
};

CreadorService creadorService = new CreadorService(config);


//nuevoCreador = creadorService.Create(nuevoCreador);

//string idCreadorAEliminar = "6752b10be809e6c7bda89d40";
//creadorService.Remove(idCreadorAEliminar);


app.Run();
