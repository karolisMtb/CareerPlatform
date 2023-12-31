using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.BusinessLogic.Services.SecurityServices;
using CareerPlatform.BusinessLogic.Services.UserServices;
using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Interfaces;
using CareerPlatform.DataAccess.Repositories;
using CareerPlatform.DataAccess.UnitOfWork;
using CareerPlatform.DataAccess.Validators;
using FluentValidation;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordReminderService, PasswordReminderService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<UserProfile>, ProfileValidator>();
builder.Services.AddScoped<IValidator<Address>, AddressValidator>();
builder.Services.AddScoped<IValidator<LoginModel>, LoginValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterValidator>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")), ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Career platform API"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "A JWT token is required to access this API.",
        BearerFormat = "JWT",
        Name = "Bearer Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },

                Scheme = "oauth2",
                Name ="Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Configuration V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//ateiciai, kad tvarkingai atrodytu Program.cs:
//https://www.linkedin.com/search/results/content/?keywords=extensions%20methods&sid=ItY&update=urn%3Ali%3Afs_updateV2%3A(urn%3Ali%3Aactivity%3A7026106808721158144%2CBLENDED_SEARCH_FEED%2CEMPTY%2CDEFAULT%2Cfalse) linkas i Stefan Dokic post apie tai.