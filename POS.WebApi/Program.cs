using log4net;
using log4net.Config;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Identity.Web;
using Azure.Security.KeyVault.Secrets;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Security.Policy;


var builder = WebApplication.CreateBuilder(args); 
//for vault key
var vaultUri = Environment.GetEnvironmentVariable("VaultURL");
var keyVaultClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());


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


//Configure Azure AD authentication
//Client ID from key vault
var id1 = await keyVaultClient.GetSecretAsync("ClientId");
var ClientId = id1.Value.Value;

//Tenant ID from key vault
var id2 = await keyVaultClient.GetSecretAsync("TenantId");
var TenantId = id2.Value.Value;
builder.Configuration["AzureAD:ClientId"] = ClientId;
builder.Configuration["AzureAD:TenantId"] = TenantId;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        options =>
        {
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidAudience = "api://6b1c8423-d675-4558-9302-05cdf2e9b14e";
        },
        microsoftidentityoptions =>
        {
            builder.Configuration.GetSection("AzureAD").Bind(microsoftidentityoptions);
        },
        "Bearer",
        true);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
           policy.RequireRole("Admin"));
    //options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("roles", "Admin"));
    options.AddPolicy("CashierPolicy", policy => policy.RequireRole( "Cashier"));
});

builder.Services.AddControllers();
//

//For Entity Framework
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
/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Roles";
    options.DefaultChallengeScheme = "Roles";
})
.AddJwtBearer("Roles", options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCashierRole", policy => policy.RequireRole("Cashier"));
    options.AddPolicy("RequireAdminOrCashierRole", policy =>
       policy.RequireRole("Admin", "Cashier"));
});
*/

// Configure CosmosClient
var secret = await keyVaultClient.GetSecretAsync("COSMOSCONNECTIONSTRING");
var connectionString = secret.Value.Value; // Retrieve the connection string value

builder.Services.AddSingleton<CosmosClient>(sp =>
{
    return new CosmosClient(connectionString);
});


//Without using key vault
/* //Configure CosmosClient
var cosmosDbSettings = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var connectionString = cosmosDbSettings["COSMOS_CONNECTION_STRING"];
    return new CosmosClient(connectionString);
});*/


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



// Add Swagger generation
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Resolve the CosmosDbSetup service and call the setup method
using (var scope = app.Services.CreateScope())
{
    var cosmosDbSetup = scope.ServiceProvider.GetRequiredService<CosmosDbSetup>();
    await cosmosDbSetup.CreateDatabaseAndContainerAsync();
}



//For Entity Framework
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();
    POSDbContext.SeedData(dbContext);
}*/



// To Use Basic Authentication uncomment this.
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
