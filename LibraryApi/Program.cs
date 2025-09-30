using HotChocolate.AspNetCore.Voyager;
using LibraryApi.Data;
using LibraryApi.Graphql.Book.Mutations;
using LibraryApi.Graphql.Book.Queries;
using LibraryApi.Graphql.Loan.Mutation;
using LibraryApi.Graphql.Loan.Queries;
using LibraryApi.Graphql.User.Mutations;
using LibraryApi.Graphql.User.Queries;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Registrar DbContext con SQL Server
builder.Services.AddPooledDbContextFactory<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar GraphQL (sin autenticación)
builder.Services
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))  
        .AddTypeExtension<BookQuery>()   
        .AddTypeExtension<LoanQuery>()   
        .AddTypeExtension<UserQuery>()  
    .AddMutationType(d => d.Name("Mutation")) 
        .AddTypeExtension<BookMutation>()    
        .AddTypeExtension<LoanMutation>()     
        .AddTypeExtension<UserMutation>()
    .AddFiltering()
    .AddSorting();

// Configurar JWT para los endpoints REST (no para GraphQL)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "clave_super_secreta_1234567890";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Servicios personalizados
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmins", policy =>
        policy.RequireClaim("Admin", "true"));
});

// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Configuración de seguridad para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT generado."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Orden correcto de middlewares
app.UseCors("AllowAll");


app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoints GraphQL (sin autenticación)
app.MapGraphQL("/graphql");

// Voyager (explorador visual tipo Swagger para GraphQL)
app.UseVoyager("/graphql-voyager", "/graphql");

app.Run();
