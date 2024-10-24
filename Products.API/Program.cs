using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Products.API;
using Products.API.Endpoints;
using Products.API.Repositories;
using Products.API.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// services

// setup EF connection string
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer("name=DefaultConnection"));

// setup Cors policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });

    // use to allow any origin
    options.AddPolicy("any", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// make appsettings service
var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettings);

// setup outpu cache
builder.Services.AddOutputCache();

// setup swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// dependency injections
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// setup automapper
builder.Services.AddAutoMapper(typeof(Program));

// setup fluentvalidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// setup jwt auth
// builder.Services.AddAuthentication().AddJwtBearer();
// builder.Services.AddAuthorization();

// configure jwt authentication
var appSett = appSettings.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSett!.Key ?? "");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();


// services end

var app = builder.Build();

// middlewares

// be mindful of correct middlewares use order
if (builder.Environment.IsDevelopment()) // enabled swagger while in development only
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseOutputCache();
app.UseAuthorization();

app.MapGroup("/api/products").MapProducts();
app.MapGroup("/api/authentication").MapAuthentication();
// middlewares end


app.Run();