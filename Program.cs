using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Services;
using HelpdeskBlazor.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddDbContext<HelpdeskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IDocumentRequestService, DocumentRequestService>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ISessionService, SessionService>();

builder.Services.AddLogging(builder => builder.AddConsole());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HelpdeskDbContext>();
        try
        {
            var canConnect = context.Database.CanConnect();
            if (canConnect)
            {
                Console.WriteLine("Database connection successful!");
            }
            else
            {
                Console.WriteLine("Cannot connect to database.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
        }
    }
}

app.Run();