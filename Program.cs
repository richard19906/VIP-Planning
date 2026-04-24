using Supabase;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
});

string url = "https://sohmawglapyvhuwjfrvz.supabase.co";
string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNvaG1hd2dsYXB5dmh1d2pmcnZ6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ0NjAxNzcsImV4cCI6MjA5MDAzNjE3N30.3D5aDHqWzy46eInAPWOf31hUDjndgf1oI_Wd7tQIRiY";
builder.Services.AddScoped(_ => new Supabase.Client(url, key, new SupabaseOptions { AutoConnectRealtime = true }));

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// DE STARTPAGINA IS NU ALTIJD HET DASHBOARD
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");