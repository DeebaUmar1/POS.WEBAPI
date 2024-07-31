using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using PointOfSaleWebAPIs.AutoMapper;
using POS.Data;
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
using Microsoft.Azure.Cosmos;
using POS.Repositories.ProductRepository;
using Microsoft.Extensions.Hosting;
using POS.Repositories.PurchaseProductRepository;
using POS.Services.PurchaseProductServices;


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
/*
builder.Services.AddDbContext<POSDbContext>(options =>
    options.UseInMemoryDatabase("POSDatabase"));


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductService>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionService>();
*/
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



// Configure CosmosClient
var cosmosDbSettings = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var connectionString = cosmosDbSettings["COSMOS_CONNECTION_STRING"];
    return new CosmosClient(connectionString);
});

// Register CosmosDbSetup with CosmosClient
builder.Services.AddSingleton<CosmosDbSetup>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    return new CosmosDbSetup(cosmosClient);
});


// Register ProductCosmosRepository with correct parameters
builder.Services.AddScoped<IProductRepository>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<CosmosClient>();
 
    return new ProductCosmosRepository(client);
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductService>();



// Register UserCosmosRepository with correct parameters
builder.Services.AddScoped<IUserRepository>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<CosmosClient>();
   
    return new UserCosmosRepository(client);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserService>();



// Register TransactionCosmosRepository with correct parameters
builder.Services.AddScoped<ITransactionRepository>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<CosmosClient>();

    return new TransactionCosmosRepository(client);
});

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionService>();




// Register PurchaseProductRepository with correct parameters
builder.Services.AddScoped<IPurchaseProductRepository>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<CosmosClient>();

    return new PurchaseProductCosmosRepository(client);
});

builder.Services.AddScoped<IPurchaseProductServices, PurchaseProductServices>();
builder.Services.AddScoped<PurchaseProductServices>();



// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCashierRole", policy => policy.RequireRole("Cashier"));
});
// Add Swagger generation
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Resolve the CosmosDbSetup service and call the setup method
using (var scope = app.Services.CreateScope())
{
    var cosmosDbSetup = scope.ServiceProvider.GetRequiredService<CosmosDbSetup>();
    await cosmosDbSetup.CreateDatabaseAndContainerAsync();
}


/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();
    POSDbContext.SeedData(dbContext);
}*/

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
