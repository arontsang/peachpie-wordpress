using System.Threading;
using System.Threading.Tasks;
using Pchp.Core.Reflection;
using Peachpie.Library.PDO;
using PeachPied.WordPress.Standard;

namespace PeachPied.Demo.Plugins;

public class LitestreamHelper : IWpPlugin
{
    public ValueTask ConfigureAsync(WpApp app, CancellationToken token)
    {
        if (app.Context.Globals["wpdb"] is { } wpdb
            && wpdb.AsObject() is wpdb db
            && db.__get("dbh") is { IsObject: true } dbh
            && dbh.AsObject() is {} translator
            && translator.GetPhpTypeInfo() is { Name: "WP_SQLite_Translator", IsPhpType: true }
            )
        {
            db.query("PRAGMA busy_timeout = 5000;");
            db.query("PRAGMA synchronous = NORMAL;");
            db.query("PRAGMA wal_autocheckpoint = 0;");
        }
        return ValueTask.CompletedTask;
    }
}