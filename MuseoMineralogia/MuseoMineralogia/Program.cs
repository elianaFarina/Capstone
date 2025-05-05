using MuseoMineralogia.Data;
using Microsoft.EntityFrameworkCore;
using MuseoMineralogia.Models;
using Microsoft.AspNetCore.Identity;
using MuseoMineralogia.Services;
using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddControllersWithViews();
// Configura DbContext
builder.Services.AddDbContext<MuseoContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("MuseoConnection")));
// Configura Identity
builder.Services.AddIdentity<Utente, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<MuseoContext>()
.AddDefaultTokenProviders();
// Configurazione della durata del token per il reset della password
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(2));
// Configura Cookie
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
});
// Configura il servizio email
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Configura Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

var app = builder.Build();
// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configurazione global di Stripe
var stripeOptions = app.Services.GetRequiredService<IOptions<StripeSettings>>();
StripeConfiguration.ApiKey = stripeOptions.Value.SecretKey;

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");
// Seeding dei ruoli e dell'account admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Utente>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<MuseoContext>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("Utente"))
        {
            await roleManager.CreateAsync(new IdentityRole("Utente"));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@museo.it");
        if (adminUser == null)
        {
            var admin = new Utente
            {
                UserName = "admin@museo.it",
                Email = "admin@museo.it",
                Nome = "Amministratore",
                Cognome = "Sistema",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Seeding dei tipi di biglietto
        if (!context.TipiBiglietto.Any())
        {
            context.TipiBiglietto.AddRange(
                new TipoBiglietto { Nome = "Biglietto Intero", Prezzo = 10.00m },
                new TipoBiglietto { Nome = "Biglietto Ridotto", Prezzo = 5.00m }
            );
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Si è verificato un errore durante il seeding del DB.");
    }
}
app.Run();