using System;
using System.Threading;
using System.Threading.Tasks;
using Pchp.Core;
using Pchp.Core.Reflection;
using Peachpie.Library.PDO;
using PeachPied.WordPress.Standard;
#nullable enable

namespace PeachPied.Demo.Plugins;

using Expression = System.Linq.Expressions.Expression;

public class LitestreamHelper : IWpPlugin
{
    private static Func<Context, PhpValue, PDO>? _fetchPdo;
    
    
    public ValueTask ConfigureAsync(WpApp app, CancellationToken token)
    {
        if (_fetchPdo == null && app.Context.GetDeclaredType("WP_SQLite_Translator") is {} translatorType)
        {
            _fetchPdo = FetchDbo(translatorType);
        }
        
        
        if (app.Context.Globals["wpdb"] is { } wpdb
            && wpdb.AsObject() is wpdb db
            && db.__get("dbh") is { IsObject: true } dbh
            && dbh.AsObject() is {} translator
            && translator.GetPhpTypeInfo() is { Name: "WP_SQLite_Translator", IsPhpType: true }
            && _fetchPdo != null)
        {
            var pdo = _fetchPdo(app.Context, dbh);

            pdo.exec("PRAGMA busy_timeout = 5000;");
            pdo.exec("PRAGMA synchronous = NORMAL;");
            pdo.exec("PRAGMA wal_autocheckpoint = 0;");
        }
        return ValueTask.CompletedTask;
    }

    private Func<Context, PhpValue, PDO> FetchDbo(PhpTypeInfo typeInfo)
    {
        var pdoProperty = typeInfo.GetDeclaredProperty("pdo");
        var context = Expression.Parameter(typeof(Context));
        var translator = Expression.Parameter(typeof(PhpValue));
        
        var asObject = typeof(PhpValue).GetMethod(nameof(PhpValue.AsObject))!;
        
        var getter = pdoProperty.Bind(
            context, 
            Expression.Convert(
                Expression.Call(translator, asObject), 
                typeInfo.Type));
        
        var lambda = Expression.Lambda<Func<Context, PhpValue, PDO>>(
            Expression.Convert(
                Expression.Call(getter, asObject), 
                typeof(PDO)),
            context,
            translator);
        return lambda.Compile();
    }
}