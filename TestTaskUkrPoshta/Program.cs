using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Entities;
using TestTaskUkrPoshta.Repositories;
using TestTaskUkrPoshta.Repositories.Interfaces;
using TestTaskUkrPoshta.Services;
using TestTaskUkrPoshta.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<ISqlRepository, SqlRepository>(opts => new SqlRepository(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddScoped<ISqlManager, SqlManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await FillTables(app);

app.Run();

async Task FillTables(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var serviceProvider = scope.ServiceProvider;

    var sqlManager = serviceProvider.GetRequiredService<ISqlManager>();

    var company = await sqlManager.GetCompany();
    if (company.Id == default)
    {
        await sqlManager.CreateDataBase();
        await sqlManager.InitializeData();
    }
}

