using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Data;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddDataServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Writer");
        
        builder.Services.AddDbContext<AccessDbContext>(options =>
            options.UseMySql(
                connectionString: connectionString,
                serverVersion: new MySqlServerVersion(new Version(5, 31, 7)),
                mySqlOptionsAction: action =>
                {
                    action.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                }));
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        return builder;
    }
}
