using UnifiedAuthDemo.Models;
using UnifiedAuthLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var config = new ConfigurationBuilder()
           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "OIDCSettings"))
           .AddJsonFile("ProviderInfo.json")
           .Build();
builder.Services.AddTransient<LineLoginService>();
builder.Services.AddTransient<GoogleLoginService>();
builder.Services.AddTransient<FaceBookLoginService>();
builder.Services.AddTransient<TwitterLoginService>();
builder.Services.AddTransient<DropboxLoginService>();
builder.Services.AddTransient<InstagramLoginService>();
builder.Services.Configure<LineLoginModel>(config.GetSection("LineLogin"));
builder.Services.Configure<GoogleLoginModel>(config.GetSection("GoogleLogin"));
builder.Services.Configure<FaceBookLoginModel>(config.GetSection("FaceBookLogin"));
builder.Services.Configure<TwitterLoginModel>(config.GetSection("TwitterLogin"));
builder.Services.Configure<DropboxLoginModel>(config.GetSection("DropboxLogin"));
builder.Services.Configure<InstagramLoginModel>(config.GetSection("InstagramLogin"));
builder.Services.AddHttpClient();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
