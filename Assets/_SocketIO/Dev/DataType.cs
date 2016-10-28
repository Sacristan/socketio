using System.Collections.Generic;

public enum DataType
{
    UNKNOWN = -1,
    INT = 0,
    FLOAT = 1,
    STRING = 2,
    BOOL = 3,
    OBJECT = 4
}

public static class DataTypeDeterminator
{
    private static object _obj;

    public static DataType DetermineDataType(object obj)
    {
        _obj = obj;

        DataType result = DataType.UNKNOWN;

        if (IsObject())
            result = DataType.OBJECT;
        else if (IsFloat())
            result = DataType.FLOAT;
        else if (IsInt())
            result = DataType.INT;
        else if (IsBool())
            result = DataType.BOOL;
        else
            result = DataType.STRING;

        return result;
    }

    private static bool IsObject()
    {
        return (Dictionary<string, object>)_obj != null;
    }

    private static bool IsInt()
    {
        int i;
        return int.TryParse(_obj.ToString(), out i);
    }

    private static bool IsFloat()
    {
        float i;
        return float.TryParse(_obj.ToString(), out i);
    }

    private static bool IsBool()
    {
        bool i;
        return bool.TryParse(_obj.ToString(), out i);
    }
}