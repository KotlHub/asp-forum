using ASP_201.Data;
using ASP_201.Middleware;
using ASP_201.Services;
using ASP_201.Services.Email;
using ASP_201.Services.Hash;
using ASP_201.Services.Kdf;
using ASP_201.Services.Random;
using ASP_201.Services.Transliterate;
using ASP_201.Services.Validate;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<DateService>();
builder.Services.AddScoped<TimeService>();
builder.Services.AddSingleton<StampService>();

// bind IHashService to Md5HashService
builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IRandomService, RandomServiceV1>();
builder.Services.AddSingleton<IKdfService, HashKdfService>();
builder.Services.AddSingleton<IValidateService, ValidationServiceV1>();
builder.Services.AddSingleton<IEmailService, GmailService>();
builder.Services.AddSingleton<ITransliterationServiceUkr, TransliterationServiceUkr>();

/*
// ðåºñòðàö³ÿ êîíòåêñòó ç ï³äêëþ÷åííÿì äî MS SQL Server
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MsDb")
    )
);
*/

// ðåºñòðàö³ÿ êîíòåêñòó ç ï³äêëþ÷åííÿì äî MySQL 
// îñîáëèâ³ñòü - äëÿ êîíòåêñòó ñë³ä çàçíà÷èòè âåðñ³þ MySQL
/*
// âàð³àíò 1 - âèçíà÷èòè âåðñ³þ òà ââåñòè äàí³
ServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 23));
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlDb"),
        serverVersion));
*/

// âàð³àíò 2 - àâòîìàòè÷íå âèçíà÷åííÿ âåðñ³¿, àëå äëÿ öüîãî òðåáà 
// ïîïåðåäíüî ñòâîðèòè ï³äêëþ÷åííÿ
String? connectionString = builder.Configuration.GetConnectionString("MyDb");
MySqlConnection connection = new(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

//app.UseMiddleware<SessionAuthMiddleware>();
app.UseSessionAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();