using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Sharkable.AutoCrud.SqlSugar;

public static class AutoCrudExtension
{
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, Action<SqlSugarOptions>? setupOption = null)
    {
        var opt = new SqlSugarOptions();
        setupOption?.Invoke(opt);
        services.Configure<SqlSugarOptions>(o => o = opt);
        StaticConfig.EnableAot = true;
        var conf = new ConnectionConfig
        {
            IsAutoCloseConnection = opt.IsAutoCloseConnection,
            DbType = (global::SqlSugar.DbType)opt.DbType,
            ConnectionString = opt.ConnectionString,
            ConfigId = opt.ConfigId
        };
        SqlSugarScope sqlSugar = new(conf);
        services.AddSingleton<ISqlSugarClient>(sqlSugar);
        return services;
    }
}
