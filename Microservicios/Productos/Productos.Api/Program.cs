using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Productos.Aplicacion;
using Productos.Infraestructura.Context;
using RegistroEstudiantesH.Aplicacion;
using Transversales.Repositorio.Implementaciones;
using Transversales.Repositorio.Interfaces;
using Transversales.Shared.Constantes;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<EstudiantesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(ConstantesAplicacion.ConnectionStringProductosDb)
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PublicApi", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// UnitOfWork
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<EstudiantesDbContext>());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// AutoMapper
builder.Services.AddAutoMapper(typeof(Productos.Aplicacion.Maps.RegistroEstudiantesHProfile).Assembly);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();

// Controllers
builder.Services.AddControllers();

// Swagger clásico (.NET 8)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // Definición del esquema JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    // Aplicar JWT globalmente
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "RegistroEstudiantesHApi",
            ValidAudience = "RegistroEstudiantesHApi",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("PublicApi");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
