using System.Collections.ObjectModel;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Pchp.Core;

namespace Peachpie.Library.Sqlite;

[PhpType(PhpTypeAttribute.InheritName)]
[PhpExtension(nameof(SQLite3))]
public class SQLite3Result
{
    private readonly SqliteCommand _command;
    private readonly SqliteDataReader _reader;
    private readonly ReadOnlyCollection<DbColumn> _columns;

    public SQLite3Result(SqliteCommand command, SqliteDataReader reader)
    {
        _command = command;
        _reader = reader;
        _columns = reader.GetColumnSchema();
    }

    public PhpValue columnName(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= _columns.Count)
            return PhpValue.False;
        
        return _columns[columnIndex].ColumnName;
    }
    
    public PhpValue columnType(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= _columns.Count)
            return PhpValue.False;
        
        return GetColumnType(_columns[columnIndex]);
    }

    public PhpValue fetchArray(int mode)
    {
        if (!_reader.Read())
            return PhpValue.False;
        var ret = new PhpArray(_reader.FieldCount);
        for (var i = 0; i < _reader.FieldCount; i++)
        {
            var column = _columns[i];
            ret.Add(column.ColumnName, _reader.GetValue(i));
        }

        return ret;
    }

    private static int GetColumnType(DbColumn column)
    {
        return 1;
    }
}