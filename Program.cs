using Supabase;
using VIP_Planning.Models;

var builder = WebApplication.CreateBuilder(args);

// Supabase Koppeling
var url = "https://sohmawglapyvhuwjfrvz.supabase.co";
var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNvaG1hd2dsYXB5dmh1d2pmcnZ6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ0NjAxNzcsImV4cCI6MjA5MDAzNjE3N30.3D5aDHqWzy46eInAPWOf31hUDjndgf1oI_Wd7tQ";
var options = new SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();