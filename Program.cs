using MongoDB.Bson;
using MongoDB.Driver;

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



app.Run();
