using System.Collections.Generic;
using UnityEngine;

public class NetworkData
{
    #region Properties
    private readonly string _rawJSON;

    //TODO: readonly?
    private Dictionary<string, object> _objects;
    private Dictionary<string, int> _ints;
    private Dictionary<string, float> _floats;
    private Dictionary<string, string> _strings;
    private Dictionary<string, NetworkData> _arrays;

    public string raw { get { return _rawJSON; } }
    public string formattedRaw { get { return _rawJSON.Replace(System.Environment.NewLine, ""); } }
    public string header { get; private set; }

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
            return keys.Length;
        }
    }
    #endregion

    public NetworkData(string rawJson)
    {
        _rawJSON = rawJson;
        Parse();
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
        return _objects.ContainsKey(key);
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

    private void Parse()
    {
        _objects = JSONDictionaryParser.ParseJSON(formattedRaw);
        Debug.Log(_objects.Keys.ToString());

        foreach(KeyValuePair<string,object> x in _objects)
        {
            string key = x.Key;
            object valueObj = x.Value;
            string valueStr = valueObj.ToString();

            Debug.Log(string.Format("Key: {0} Value: {1}", key, valueStr));

            DataType dataType = DataTypeDeterminator.DetermineDataType(x.Value);

            Debug.Log("Received Data type: "+dataType);

            switch (dataType)
            {
                case DataType.INT:
                    break;
                case DataType.FLOAT:
                    break;
                case DataType.STRING:
                    break;
                case DataType.BOOL:
                    break;
                case DataType.OBJECT:
                    //NetworkData networkData = new NetworkData(valueStr);
                    //_arrays.Add(key, networkData);
                    break;
                default:
                    HandleParseException();
                    break;
            }

        }
    }

    private void HandleParseException()
    {
        Debug.Log("Something went wrong when parsing data: "+ formattedRaw);
    }
}