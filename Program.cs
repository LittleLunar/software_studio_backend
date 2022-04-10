using software_studio_backend.Models;
using software_studio_backend.Services;
using software_studio_backend.Utils;
using software_studio_backend.Middlewares;
using software_studio_backend.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<TestDatabaseSettings>(
    builder.Configuration.GetSection("TestDatabase")
);

// There is no appropiate way to instantiate this class ^-^.
Configuration myStaticConfig = new Configuration(builder.Configuration);

builder.Services.AddSingleton<Configuration>(myStaticConfig);

// builder.Services.BuildServiceProvider().GetService<Configuration>();

builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ContentService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddTransient<Middleware>();

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

// app.UseCors(x => x
//                 .SetIsOriginAllowed(origin => true)
//                 .AllowAnyMethod()
//                 .AllowAnyHeader()
//                 .AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();

// Adding Custom Middlewares
app.Map("/api", nextPath =>
{
  nextPath.Map("/User", Middleware.RequireAuthHandler);

  nextPath.Map("/Comment", Middleware.RequireAuthHandler);

  nextPath.Map("/Content/Like", Middleware.RequireAuthHandler);

  nextPath.UseEndpoints(endpoints =>
  {
    endpoints.MapControllers();
  });

});

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();


