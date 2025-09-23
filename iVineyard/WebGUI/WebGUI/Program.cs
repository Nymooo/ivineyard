using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Entities.Bookingobjects.Vineyard;
using Models;
using MudBlazor.Services;
using Services.Implementations;
using WebGUI.Client.ClientServices;
using WebGUI.Client.Pages;
using WebGUI.Components;
using WebGUI.Components.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

/* REPOSITORIES */
builder.Services.AddScoped<IBookingObjectRepository, BookingObjectRepository>();
builder.Services.AddScoped<IVineyardRepository, VineyardRepository>();

/* SERVICES */
builder.Services.AddScoped<RegisterModel>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<LoginModel>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<WorkinformationService>();
builder.Services.AddScoped<BookingObjectService>();
builder.Services.AddHttpClient(); // Must be injected because Client Services depend on it
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VineyardService>();
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<VineyardHasStatusService>();
builder.Services.AddScoped<MachineService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<MachineHasStatusService>();
builder.Services.AddScoped<TankService>();
builder.Services.AddScoped<BatchService>();


builder.Services.AddMudServices();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseMySql(
        builder.Configuration.GetConnectionString("DatabaseConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    )
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebGUI.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();