
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using PointOfSaleWebAPIs.AutoMapper;
using POS.Data;
using POS.Repositories.ProductRepository;
using POS.Repositories.Repository;
using POS.Services.ProductServices;
using System.Reflection;
using POS.Middlewares;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Log4Net;
using POS.Repositories.UserRepository;
using POS.Services.UserServices;
using POS.Services;
using POS.Repositories.TransactionRepository;
using POS.Services.TransactionServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using POS.Middlewares.Middlewares;


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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
               /* ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,*/
                ValidateIssuerSigningKey = true,
             /*   ValidIssuer = "yourIssuer",
                ValidAudience = "yourAudience",*/
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32BytesLong")) // Must match the signing key
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CashierPolicy", policy => policy.RequireRole("Cashier"));
});
// Add Swagger generation
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();
    POSDbContext.SeedData(dbContext);
}

/*// Ensure that the BasicAuthMiddleware is added before BearerTokenMiddleware
app.UseMiddleware<BasicAuthMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>("Admin");
app.UseMiddleware<CustomExceptionHandlingMiddleware>();*/
//app.UseMiddleware<BearerTokenMiddleware>();

app.UseRouting();
/*
app.UseAuthentication();
app.UseAuthorization();*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
