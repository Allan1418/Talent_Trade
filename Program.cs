using MongoDB.Bson;
using MongoDB.Driver;

using Talent_Trade.Models;
using Talent_Trade.Services;

using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;

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


//Identity configuration
builder.Services.AddIdentityMongoDbProvider<Usuario, MongoRole>(identity =>
{
    // Opciones de Identity (opcional)
    identity.SignIn.RequireConfirmedEmail = false;
    identity.User.RequireUniqueEmail = true;
},
mongo =>
{
    mongo.ConnectionString = builder.Configuration.GetConnectionString("MongoDB");
    // ... otras opciones de MongoDB ...
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // ... opciones de cookies ...
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








//ejemplo Prueba Usuario Service
//Usuario nuevoUsuario = new Usuario
//{
//    NombreCompleto = "Juan Pérez2",
//    UserName = "juanperez",
//    Email = "juan.perez@example.com",
//    PasswordHash = "contraseñaSegura123",
//    FechaRegistro = DateTime.Now,
//    // Id, ImagePerfil, IdDeCreador se asignarán automáticamente o después 
//    // Suscripciones y Facturas se pueden inicializar como listas vacías o con valores
//    Suscripciones = new List<string>(),
//    Facturas = new List<string>()
//};

//IConfiguration config = builder.Configuration;
//UsuarioService usuarioService = new UsuarioService(config);

//Console.WriteLine("a--- " + nuevoUsuario.Id);
//nuevoUsuario = usuarioService.Create(nuevoUsuario);
//Console.WriteLine("d--- " + nuevoUsuario.Id);


//usuarioService.Remove("67512a48660674cf1dd5b1d9");


//nuevoUsuario.NombreCompleto = "aaaaaaaaaaaa";
//nuevoUsuario.Id = "67512b12ad47458b206fdd1f";

//usuarioService.Update("67512b12ad47458b206fdd1f", nuevoUsuario);


//ejemplo de Usuario listo para logeo
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<MongoRole>>();

//    if (!await roleManager.RoleExistsAsync("usuario"))
//    {
//        await roleManager.CreateAsync(new MongoRole("usuario"));
//    }

//    if (!await roleManager.RoleExistsAsync("creador"))
//    {
//        await roleManager.CreateAsync(new MongoRole("creador"));
//    }

//    // Ejemplo Prueba Usuario Service
//    Usuario nuevoUsuario = new Usuario
//    {
//        NombreCompleto = "Juan Pérez2",
//        UserName = "juanperez",
//        Email = "juan.perez@example.com",
//        FechaRegistro = DateTime.Now,
//        // ... otras propiedades ...
//    };

//    // Obtén una instancia del UserManager
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

//    // Genera un nuevo SecurityStamp
//    nuevoUsuario.SecurityStamp = userManager.GenerateNewAuthenticatorKey();

//    IConfiguration config = builder.Configuration;
//    UsuarioService usuarioService = new UsuarioService(config);

//    Console.WriteLine("a--- " + nuevoUsuario.Id);

//    // Crea el usuario en la base de datos (sin contraseña)
//    nuevoUsuario = usuarioService.Create(nuevoUsuario);

//    Console.WriteLine("d--- " + nuevoUsuario.Id);

//    // Genera el hash de la contraseña
//    var passwordHash = userManager.PasswordHasher.HashPassword(nuevoUsuario, "contraseñaSegura123");

//    // Actualiza el usuario con el hash de la contraseña
//    nuevoUsuario.PasswordHash = passwordHash;

//    // Agrega el rol al usuario (usa await)
//    await userManager.AddToRoleAsync(nuevoUsuario, "usuario");

//    usuarioService.Update(nuevoUsuario.Id, nuevoUsuario);
//}

app.Run();
