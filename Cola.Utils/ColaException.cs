using Cola.Utils.Enums;
using Cola.Utils.Extensions;

namespace Cola.Utils;

public class ColaException : Exception
{
    public ColaException(EnumException enumException) : base(enumException.ToString())
    {
    }

    public ColaException(EnumException enumException, string msg) : base(string.Format(enumException.ToString(), msg))
    {
    }

    public ColaException(string errorMessage) : base(errorMessage)
    {
    }
}