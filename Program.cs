using Supabase;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(_ => new Supabase.Client(
    builder.Configuration["Supabase:Url"],
    builder.Configuration["Supabase:Key"],
    new SupabaseOptions { AutoConnectRealtime = true }
));

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(8);
});

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();