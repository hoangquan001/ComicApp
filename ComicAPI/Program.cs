using System.Text;
using ComicApp.Data;
using ComicApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ComicAPI.Middleware;
using ComicAPI.Services;
using System.Security.Claims;
using ComicApp.Models;
using Microsoft.Extensions.FileProviders;
using ComicAPI.Updater;

// Enable CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ComicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.RequireHttpsMetadata = false;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        // ValidAudience = builder.Configuration["JWT:ValidAudience"],
        // ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
}

);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddSingleton<ITokenMgr, TokenMgr>();
builder.Services.AddScoped<IComicReposibility, ComicReposibility>();

// builder.Services.AddSingleton<IComicCacheReposibility, ComicCacheReposibility>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Specify the authorization token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAutoMapper(typeof(Program));


WebApplication? app = builder.Build();

// Configure the HTTP request pipeline. 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Use CORS
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TokenHandlerMiddlerware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    RequestPath = new PathString("/static")
});
//imject IUserService
app.Use((context, next) =>
{
    return next();
});
app.UseAuthentication();
app.Use((context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated ?? false)
    {
        var userService = context.RequestServices.GetService<IUserService>();
        if (userService != null)
        {
            var strId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            userService.UserID = int.Parse(strId!);
        }

    }
    return next();
});


app.UseAuthorization();
app.MapControllers();


// Task task1 =  Updater.Update();

app.Run();

