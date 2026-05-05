using Supabase;

var builder = WebApplication.CreateBuilder(args);

// 1. Supabase Client Setup
builder.Services.AddScoped(_ => new Supabase.Client(
    builder.Configuration["Supabase:Url"],
    builder.Configuration["Supabase:Key"],
    new SupabaseOptions { AutoConnectRealtime = true }
));

// 2. Controllers & Views
builder.Services.AddControllersWithViews();

// 3. Sessiebeheer (8 uur time-out voor de werkgever)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Extra ondersteuning voor het uitlezen van de context
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Middleware configuratie
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Zorg dat UseSession altijd NA UseRouting staat

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();