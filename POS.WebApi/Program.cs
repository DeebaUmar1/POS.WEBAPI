
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using PointOfSaleWebAPIs.AutoMapper;
using POS.Data;
using POS.Repositories.ProductRepository;
using POS.Repositories.Repository;
using POS.Services.ProductServices;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using POS.Middlewares;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Log4Net;
using POS.Repositories.UserRepository;
using POS.Services.UserServices;
using POS.Services;
using POS.Repositories.TransactionRepository;
using POS.Services.TransactionServices;
using Microsoft.Extensions.DependencyInjection;

using System.Text;
using POS.Middlewares.Middlewares;
using POS.Models.Entities;


var builder = WebApplication.CreateBuilder(args);

// Configure log4net
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

// Add services
// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfig)); // Ensure you specify the profile class


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<POSDbContext>(options =>
    options.UseInMemoryDatabase("POSDatabase"));

builder.Services.AddSingleton<TokenServices>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductService>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionService>();

// Adding Authentication  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });
/*Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings.GetValue<string>("Key") ?? throw new ArgumentNullException(nameof(jwtSettings));
var issuer = jwtSettings.GetValue<string>("Issuer") ?? throw new ArgumentNullException(nameof(jwtSettings));
var audience = jwtSettings.GetValue<string>("Audience") ?? throw new ArgumentNullException(nameof(jwtSettings));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});*/
// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCashierRole", policy => policy.RequireRole("Cashier"));
});
// Add Swagger generation
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();
    POSDbContext.SeedData(dbContext);
}

// Ensure that the BasicAuthMiddleware is added before BearerTokenMiddleware
//app.UseMiddleware<BasicAuthMiddleware>();
/*app.UseMiddleware<AuthorizationMiddleware>("Admin");*/

app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
