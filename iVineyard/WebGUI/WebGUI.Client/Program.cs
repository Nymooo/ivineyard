using WebGUI.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WebGUI.Client.ClientServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped(sp => new HttpClient());

/* CLIENT-SERVICES */
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VineyardService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<WorkinformationService>();
builder.Services.AddScoped<BookingObjectService>();
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<VineyardHasStatusService>();
builder.Services.AddScoped<MachineService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<MachineHasStatusService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();