using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using Toolbox_Library.Context;
using Toolbox_Library.Repositories;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities.Encryption;
using Toolbox_Library.Utilities.Encryption.Interface;

namespace Toolbox_API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        string saltFileLocation = builder.Configuration["Salt"] ?? throw new Exception("Salt file location is not configured");
        string encryptionKeyFileLocation = builder.Configuration["EncryptionKey"] ?? throw new Exception("Encryption key file location is not configured");

        if (!File.Exists(saltFileLocation)) throw new FileNotFoundException("Salt key file does not exist!");
        else if (!File.Exists(encryptionKeyFileLocation)) throw new FileNotFoundException("Encryption key file does not exist");

        byte[] encryptedSaltFileBytes = File.ReadAllBytes(saltFileLocation);
        byte[] encryptedEncryptionFileBytes = File.ReadAllBytes(encryptionKeyFileLocation);

        byte[] decryptedSaltFileBytes = ProtectedData.Unprotect(
            encryptedSaltFileBytes,
            optionalEntropy: null,
            scope: DataProtectionScope.LocalMachine
        ); // Gives a warning as this feature is only available on windows!

        byte[] decryptedEncryptionFileBytes = ProtectedData.Unprotect(
            encryptedEncryptionFileBytes,
            optionalEntropy: null,
            scope: DataProtectionScope.LocalMachine
        ); // Gives a warning as this feature is only available on windows!

        if (decryptedSaltFileBytes == null || decryptedEncryptionFileBytes == null) throw new Exception("Something went wrong during decryption of file bytes");

        string saltKey = Encoding.UTF8.GetString(decryptedSaltFileBytes);
        string encryptionKey = Encoding.UTF8.GetString(decryptedEncryptionFileBytes);

        builder.Services.AddSingleton<IEncryptionHandler>(sp => new EncryptionHandler(saltKey, encryptionKey));

        string redisConnection = builder.Configuration.GetConnectionString("RedisConnection") ?? throw new Exception("No redis connection string!");
        string postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("No Postgres connection string!");

        builder.Services.AddDbContext<ToolboxDbContext>(options =>
            options.UseNpgsql(postgresConnection));

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnection);
        });

        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IEncryptionHandler, EncryptionHandler>();

        builder.Services.AddScoped<IUserService, UserService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
