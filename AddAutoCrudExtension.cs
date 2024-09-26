using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Sharkable.AutoCrud.SqlSugar;

public static class AutoCrudExtension
{
    /// <summary>
    /// add sqlsugar service for auto crud
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupOption"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, Action<SqlSugarOptions>? setupOption = null)
    {
        var opt = new SqlSugarOptions();
        setupOption?.Invoke(opt);
        services.Configure<SqlSugarOptions>(o => o = opt);
        StaticConfig.EnableAot = true;
        
        //get config if already exists
        var provider = services.BuildServiceProvider();
        var beforeCfg = provider.GetService<IOptions<ConnectionConfig>>()?.Value;

        //setup config by the given condition
        ConnectionConfig conf;

        if (beforeCfg != null)
        {
            conf = beforeCfg;
        }    
        else
        {
            conf = new ConnectionConfig
            {
                IsAutoCloseConnection = opt.IsAutoCloseConnection,
                DbType = (global::SqlSugar.DbType)opt.DbType,
                ConnectionString = opt.ConnectionString,
                ConfigId = opt.ConfigId,
                InitKeyType = (global::SqlSugar.InitKeyType)opt.InitKeyType,
                DbLinkName = opt.DbLinkName,
                LanguageType = (global::SqlSugar.LanguageType)opt.LanguageType,
                IndexSuffix = opt.IndexSuffix,
            };
        }
        SqlSugarScope sqlSugar = new(conf);
        services.AddKeyedSingleton<ISqlSugarClient>("autocrudsqlsugar", sqlSugar);
        return services;
    }

    /// <summary>
    /// add auto crud sqlsugar service options before add other a sharkable service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupConfig"></param>
    /// <returns></returns>
    public static IServiceCollection BeforeShark(this IServiceCollection services, Action<ConnectionConfig>? setupConfig)
    {
        services.Configure<ConnectionConfig>(opt =>
        {
            setupConfig?.Invoke(opt);
        });
        return services;
    }
}
