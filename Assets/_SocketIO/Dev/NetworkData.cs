public class NetworkData
{
    #region Properties
    private readonly string _rawJSON;

    public string raw { get { return _rawJSON; } }

    public string header
    {
        get
        {
            throw new System.NotImplementedException();
            return "";
        }
    }

    public string[] keys
    {
        get
        {
            throw new System.NotImplementedException();
            return new string[0];
        }
    }

    public int keyCount
    {
        get
        {
            throw new System.NotImplementedException();
            return -1;
        }
    }
    #endregion


    public NetworkData(string rawJson)
    {
        _rawJSON = rawJson;
    }

    public override string ToString()
    {
        return _rawJSON;
    }

    #region Key Check

    public bool HasKey(string key)
    {
        return Exists(key);
    }

    public bool Exists(string key)
    {
        throw new System.NotImplementedException();
        return false;
    }

    #endregion

    #region Get By Key
    public bool GetInt(string key, out int refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    public bool GetFloat(string key, out float refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    public bool GetBool(string key, out bool refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    public bool GetString(string key, out string refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    public bool GetArray(string key, out NetworkData[] refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    #endregion

}