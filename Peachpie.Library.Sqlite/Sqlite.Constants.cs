using Pchp.Core;

namespace Peachpie.Library.Sqlite;

[PhpExtension(nameof(SQLite3))]
public partial class Constants
{
    [PhpConstant]
    public const int SQLITE3_ASSOC = 1;

    public const int SQLITE3_NUM = 0;
    
    public const int SQLITE3_BOTH = 2;
}