using software_studio_backend.Services;
using software_studio_backend.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using software_studio_backend.Middleware;
using software_studio_backend.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<TestDatabaseSettings>(
    builder.Configuration.GetSection("TestDatabase")
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["Jwt:Issuer"],
      ValidAudience = builder.Configuration["Jwt:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
      ClockSkew = TimeSpan.Zero
    };
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(name: builder.Configuration["Authorization:PolicyName"],
    policy =>
    {
      policy.RequireRole("admin");
    });
});

builder.Services.AddCors(option =>
{
  option.AddPolicy(name: builder.Configuration["Cors:PolicyName"],
  policy =>
  {
    policy.WithOrigins("http://localhost:5500", "http://localhost:3000")
      .SetIsOriginAllowed(origin => true)
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
  });
});
// builder.Services.AddCors();

// There is no appropiate way to instantiate this class ^-^.
Configuration myStaticConfig = new Configuration(builder.Configuration);
// TokenUtils tokenUtil = new TokenUtils();
builder.Services.AddSingleton<Configuration>(myStaticConfig);
// builder.Services.AddSingleton<TokenUtils>(tokenUtil);
builder.Services.AddSingleton<MongoDBService>();

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

app.UseCors(builder.Configuration["Cors:PolicyName"]);

app.UseRevokeToken();

app.UseAuthentication();
app.UseAuthorization();

// Adding Custom Middlewares

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();


