public enum DataType
{
    UNKNOWN = -1,
    INT = 0,
    FLOAT = 1,
    STRING = 2,
    BOOL = 3,
    OBJECT = 4,
    NULL=5
}

public static class DataTypeDeterminator
{
    private static string _str;

    public static DataType DetermineDataType(string str)
    {
        _str = str;

        DataType result = DataType.UNKNOWN;

        if (IsObject() || IsArray())
            result = DataType.OBJECT;
        else if (IsNull())
            result = DataType.NULL;
        else if (IsInt())
            result = DataType.INT;
        else if (IsFloat())
            result = DataType.FLOAT;
        else if (IsBool())
            result = DataType.BOOL;
        else
            result = DataType.STRING;

        return result;
    }

    private static bool IsObject()
    {
        return _str.Contains("{") && _str.Contains("}");
    }

    private static bool IsArray()
    {
        return _str.Contains("[") && _str.Contains("]");
    }

    private static bool IsInt()
    {
        int i;
        return int.TryParse(_str, out i);
    }

    private static bool IsFloat()
    {
        float i;
        return float.TryParse(_str, out i);
    }

    private static bool IsNull()
    {
        return _str == "null" || _str.Replace(" ", "").Length == 0;
    }

    private static bool IsBool()
    {
        bool i;
        return bool.TryParse(_str, out i);
    }
}