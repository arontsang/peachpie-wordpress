using Microsoft.Data.Sqlite;
using Pchp.Core;

namespace Peachpie.Library.Sqlite;

[PhpType(PhpTypeAttribute.InheritName)]
[PhpExtension(nameof(SQLite3))]
public class SQLite3
{
    private readonly SqliteConnection _connection;
    
    
    public SQLite3(
        string fileName,
        int flags,
        string encryptionKey = "")
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = fileName;
        connectionStringBuilder.Mode = OpenMode(flags);
        _connection = new SqliteConnection(connectionStringBuilder.ToString());
    }

    public void close() => _connection.Dispose();
    
    private static SqliteOpenMode OpenMode(int flags)
    {
        // TODO: Correctly map the file modes
        return SqliteOpenMode.Memory;
    }
}