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
        if (setupOption == null)
            //will not proceed
            return services;

        var option = new SqlSugarOptions();
        //invoke and setup option
        services.Configure<SqlSugarOptions>(opt => 
        {
            setupOption?.Invoke(opt);
        });
        setupOption?.Invoke(option);
        //set aot mode
        StaticConfig.EnableAot = Shark.SharkOption.AotMode;

        //get config if already exists
        var provider = services.BuildServiceProvider();
        var beforeCfg = provider.GetService<IOptions<ConnectionConfig>>()?.Value;

        //setup config by the given condition
        ConnectionConfig conf;

        if (beforeCfg != null && beforeCfg.ConnectionString != null)
        {
            conf = beforeCfg;
        }    
        else
        {
            conf = new ConnectionConfig
            {
                IsAutoCloseConnection = option.IsAutoCloseConnection,
                DbType = (global::SqlSugar.DbType)option.DbType,
                ConnectionString = option.ConnectionString,
                ConfigId = option.ConfigId,
                InitKeyType = (global::SqlSugar.InitKeyType)option.InitKeyType,
                DbLinkName = option.DbLinkName,
                LanguageType = (global::SqlSugar.LanguageType)option.LanguageType,
                IndexSuffix = option.IndexSuffix,
            };
        }
        SqlSugarScope sqlSugar = new(conf);
        services.AddKeyedSingleton<ISqlSugarClient>(AutoCrudSqlSugar.ServiceName, sqlSugar);
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
