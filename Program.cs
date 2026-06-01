using Microsoft.EntityFrameworkCore;
using MeterTracker.Data;
using QuestPDF.Infrastructure;

// QuestPDF community licence (free for open/internal use)
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// .NET 10 compatibility: opt in to UTC DateTime handling for SQLite
AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue31776", true);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "metertracker.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// Ensure database is created and migrations applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Readings}/{action=Index}/{id?}");

app.Run();
