using System;

namespace GenX.Cli.Infrastructure.Oledb
{
    [Flags]
    public enum DbColumnFlags
    {
        IsBookmark      = 0x1,
        MayDefer        = 0x2,
        Write           = 0x4,
        WriteUnknown    = 0x8,
        IsFixedLength   = 0x10,
        IsNullable      = 0x20,
        MaybeNull       = 0x40,
        IsLong          = 0x80,
        IsRowId         = 0x100,
        IsRowVer        = 0x200,
        CacheDeferred   = 0x1000,
        ScaleIsNegative = 0x4000,
        Reserved        = 0x8000,
        IsRowUrl        = 0x10000,
        IsDefaultStream = 0x20000,
        IsCollection    = 0x40000,
        IsStream        = 0x80000,
        IsRowSet        = 0x100000,
        IsRow           = 0x200000,
        RowSpecificCol  = 0x400000
    };
}
