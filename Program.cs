using MongoDB.Bson;
using MongoDB.Driver;

using Talent_Trade.Models;
using Talent_Trade.Services;

using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);


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

    Console.WriteLine($"*** Conexion exitosa a la base de datos: {database.DatabaseNamespace} ***");
}
catch (Exception ex)
{
    Console.WriteLine($"***Error al conectar a la base de datos: {ex.Message} ***");
    Environment.Exit(1);
}
//Fin prueba mongo


// Add services to the container.
builder.Services.AddControllersWithViews();

// add services
builder.Services.AddScoped<CreadorServices>();
builder.Services.AddScoped<GridFSService>();
builder.Services.AddScoped<PublicacionServices>();
builder.Services.AddScoped<NivelSuscripcionServices>();
builder.Services.AddScoped<SuscripcionServices>();
builder.Services.AddScoped<FacturaServices>();
builder.Services.AddScoped<GananciaService>();
builder.Services.AddScoped<MesGananciaService>();
builder.Services.AddScoped<ComentarioServices>();
builder.Services.AddScoped<RespuestaServices>();


//Identity configuration
builder.Services.AddIdentityMongoDbProvider<Usuario, MongoRole>(identity =>
{
    // Opciones de Identity (opcional)
    identity.SignIn.RequireConfirmedEmail = false;
    identity.User.RequireUniqueEmail = true;

    // Configuracion del password
    identity.Password.RequiredLength = 3;
    identity.Password.RequireNonAlphanumeric = false;
    identity.Password.RequireLowercase = false;
    identity.Password.RequireUppercase = false;
    identity.Password.RequireDigit = false;

},
mongo =>
{
    mongo.ConnectionString = builder.Configuration.GetConnectionString("MongoDB");
    mongo.UsersCollection = "usuarios";
    // ... otras opciones de MongoDB ...
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // ... opciones de cookies ...
    //options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Home/Login";
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();






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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<MongoRole>>();

    if (!await roleManager.RoleExistsAsync("usuario"))
    {
        await roleManager.CreateAsync(new MongoRole("usuario"));
    }

    if (!await roleManager.RoleExistsAsync("creador"))
    {
        await roleManager.CreateAsync(new MongoRole("creador"));
    }
}

app.UseAuthorization();
app.UseAuthentication();





//-----------------------------------------
//-----------------------------------------
//-----------------------------------------
//ejemplo de Usuario listo para logeo

async void crearUser()
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<MongoRole>>();

        if (!await roleManager.RoleExistsAsync("usuario"))
        {
            await roleManager.CreateAsync(new MongoRole("usuario"));
        }

        if (!await roleManager.RoleExistsAsync("creador"))
        {
            await roleManager.CreateAsync(new MongoRole("creador"));
        }

        // Ejemplo Prueba Usuario Service
        Usuario nuevoUsuario = new Usuario
        {
            NombreCompleto = "Juan Pérez",
            UserName = "juanperez",
            Email = "juan.perez@example.com",
            FechaRegistro = DateTime.Now,
            // ... otras propiedades ...
        };

        // Obtén una instancia del UserManager
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        // Genera un nuevo SecurityStamp
        nuevoUsuario.SecurityStamp = userManager.GenerateNewAuthenticatorKey();

        IConfiguration config = builder.Configuration;
        UsuarioService usuarioService = new UsuarioService(config);

        // Crea el usuario en la base de datos (sin contraseña)
        nuevoUsuario = usuarioService.Create(nuevoUsuario);

        // Genera el hash de la contraseña
        var passwordHash = userManager.PasswordHasher.HashPassword(nuevoUsuario, "1234");

        // Actualiza el usuario con el hash de la contraseña
        nuevoUsuario.PasswordHash = passwordHash;

        // Agrega el rol al usuario (usa await)
        await userManager.AddToRoleAsync(nuevoUsuario, "usuario");

        usuarioService.Update(nuevoUsuario.Id, nuevoUsuario);
    }
}





async void resetearBD()
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    var client = new MongoClient(connectionString);
    var url = MongoUrl.Create(connectionString);
    var database = client.GetDatabase(url.DatabaseName);

    var json = File.ReadAllText("comandosMongo.json");
    var comandos = JArray.Parse(json);

    database.DropCollection("fs.files");
    database.DropCollection("fs.chunks");
    database.DropCollection("usuarios");

    foreach (var comando in comandos)
    {

        var commandDocument = BsonDocument.Parse(comando.ToString());
        await database.RunCommandAsync<BsonDocument>(commandDocument);
    }
    crearUser();

    Console.WriteLine("--- Base de Datos Reseteada ---");
}



//*******************************************************************************
//*   ______   ___   _   _  _____  _____ ______    ______ _____  _   _  _____   *
//*   |  _  \ / _ \ | \ | ||  __ \|  ___|| ___ \  |___  /|  _  || \ | ||  ___|  *
//*   | | | |/ /_\ \|  \| || |  \/| |__  | |_/ /     / / | | | ||  \| || |__    *
//*   | | | ||  _  || . ` || | __ |  __| |    /     / /  | | | || . ` ||  __|   *
//*   | |/ / | | | || |\  || |_\ \| |___ | |\ \   ./ /___\ \_/ /| |\  || |___   *
//*   |___/  \_| |_/\_| \_/ \____/\____/ \_| \_|  \_____/ \___/ \_| \_/\____/   *
//*                                                                             *
//*******************************************************************************                                                                          
//
//Descomentar la linea de abajo para Resetear la Base de Datos de Mongo y crear un usuario por defecto

//resetearBD();


//-*volver a comentarla despues de iniciar el servidor una vez







//inicio Aplicacion
app.Run();


