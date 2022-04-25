var builder = WebApplication.CreateBuilder(args);

//add services to containter

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(c =>{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

//Configure HTTP request pipeline
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(policy => policy.AllowCredentials().AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:4200"));

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index","Fallback");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager,roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex,"An error ocurred during migration");
}

await app.RunAsync();

// namespace API
// {
//     public class Program
//     {
//         public static async Task Main(string[] args)
//         {
//             AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);
//             var host = CreateHostBuilder(args).Build();
//             using var scope = host.Services.CreateScope();
//             var services = scope.ServiceProvider;
//             try
//             {
//                 var context = services.GetRequiredService<DataContext>();
//                 var userManager = services.GetRequiredService<UserManager<AppUser>>();
//                 var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
//                 await context.Database.MigrateAsync();
//                 await Seed.SeedUsers(userManager,roleManager);
//             }
//             catch (Exception ex)
//             {
//                 var logger = services.GetRequiredService<ILogger<Program>>();
//                 logger.LogError(ex,"An error ocurred during migration");
//             }
//             await host.RunAsync();
//         }

//         public static IHostBuilder CreateHostBuilder(string[] args) =>
//             Host.CreateDefaultBuilder(args)
//                 .ConfigureWebHostDefaults(webBuilder =>
//                 {
//                     webBuilder.UseStartup<Startup>();
//                 });
//     }
// }
