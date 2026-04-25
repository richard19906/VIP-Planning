using Supabase;
var builder = WebApplication.CreateBuilder(args);

// Supabase Registratie
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions { AutoConnectRealtime = true }));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();