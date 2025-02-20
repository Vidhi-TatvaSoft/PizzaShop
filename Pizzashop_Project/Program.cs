using BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using DAL.Models;
using BLL.Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var conn = builder.Configuration.GetConnectionString("PizzaDbConnection");
builder.Services.AddDbContext<DAL.Models.PizzashopDbContext>(q=>q.UseNpgsql(conn));

builder.Services.AddScoped<UserLoginService>();
builder.Services.AddScoped<JWTTokenService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(x=>{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],  // The issuer of the token (e.g., your app's URL)
            ValidAudience = builder.Configuration["JwtConfig:Audience"], // The audience for the token (e.g., your API)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]?? "")), // The key to validate the JWT's signature
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name 
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check for the token in cookies
                var token = context.Request.Cookies["AuthToken"]; // Change "AuthToken" to your cookie name if it's different
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                }
                return Task.CompletedTask;
            }
        };
    }
);

builder.Services.AddSession(
    options => {
        options.IdleTimeout = TimeSpan.FromSeconds(10);
    }
);
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();

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
    pattern: "{controller=UserLogin}/{action=VerifyPassword}/{id?}");

app.Run();
