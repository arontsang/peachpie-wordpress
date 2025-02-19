using System.Data.Common;
using Microsoft.Data.Sqlite;
using Pchp.Core;

namespace Peachpie.Library.Sqlite;

[PhpType(PhpTypeAttribute.InheritName)]
[PhpExtension(nameof(SQLite3))]
public class SQLite3
{
    private readonly SqliteConnection _connection;
    private static readonly PhpArray _version = new PhpArray(2)
    {
        { "versionString", SQLitePCL.raw.sqlite3_libversion().utf8_to_string() },
        { "versionNumber", SQLitePCL.raw.sqlite3_libversion_number() },
    };
    
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

    public static PhpArray version() => _version;
    
    public void close() => _connection.Dispose();

    public void open(string fileName, int flags, string encryptionKey = "")
    {
        
    }
    
    public static string escapeString(string text) => text;
    
    public PhpValue exec(string query)
    {
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
            return PhpValue.True;
        }
        catch
        {
            return PhpValue.False;
        }
    }
    
    public PhpValue query(string query)
    {
        try
        {
            var command = _connection.CreateCommand();
            command.CommandText = query;
            var reader = command.ExecuteReader();
            
            var ret = new SQLite3Result(command, reader);
            return PhpValue.FromClass(ret);
        }
        catch (Exception)
        {
            return PhpValue.False;
        }
    }

    public PhpValue querySingle(string query, bool entireRow = false)
    {
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return PhpValue.False;

            if (!entireRow)
                return PhpValue.FromClr(reader.GetValue(0));

            var columns = reader.GetColumnSchema();
            var ret = new PhpArray(columns.Count);
            for (var i = 0; i < columns.Count; i++)
            {
                ret.Add(columns[i].ColumnName, reader.GetValue(i));
            }

            return ret;
        }
        catch
        {
            return PhpValue.False;
        }
    }
    
    private static SqliteOpenMode OpenMode(int flags)
    {
        // TODO: Correctly map the file modes
        return SqliteOpenMode.Memory;
    }
    
    
}