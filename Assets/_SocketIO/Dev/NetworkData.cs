using System.Collections.Generic;
using UnityEngine;

public class NetworkData
{
    #region Properties
    private readonly string _rawJSON;
    private readonly string[] _keys;
    private readonly NetworkData[] _networkDatum;

    private readonly Dictionary<string, string> _objects = new Dictionary<string, string>(); // holds all key value data
    private readonly Dictionary<string, int> _ints = new Dictionary<string, int>();
    private readonly Dictionary<string, float> _floats = new Dictionary<string, float>();
    private readonly Dictionary<string, string> _strings = new Dictionary<string, string>();
    private readonly Dictionary<string, bool> _bools = new Dictionary<string, bool>();
    private readonly Dictionary<string, NetworkData> _arrays = new Dictionary<string, NetworkData>();

    public string raw { get { return _rawJSON; } }
    public string formattedRaw { get { return _rawJSON.Replace(System.Environment.NewLine, ""); } }
    public string header { get; private set; }

    public NetworkData this[int index]
    {
        get
        {
            return _networkDatum[index];
        }
    }

    public string[] keys { get { return _keys; } }
    public int keyCount { get { return keys.Length; } }
    #endregion

    public NetworkData(string rawJson)
    {
        _rawJSON = rawJson;

        _objects = JSONDictionaryParser.ParseJSON(formattedRaw);

        foreach (KeyValuePair<string, string> x in _objects)
        {
            string key = x.Key;
            string valueStr = x.Value;

            DataType dataType = DataTypeDeterminator.DetermineDataType(valueStr);

            switch (dataType)
            {
                case DataType.INT:
                    _ints.Add(key, int.Parse(valueStr));
                    break;
                case DataType.FLOAT:
                    _floats.Add(key, float.Parse(valueStr));
                    break;
                case DataType.STRING:
                    _strings.Add(key, valueStr);
                    break;
                case DataType.BOOL:
                    _bools.Add(key, bool.Parse(valueStr));
                    break;
                case DataType.OBJECT:
                    NetworkData networkData = new NetworkData(valueStr);
                    _arrays.Add(key, networkData);
                    break;
                default:
                    HandleParseException(valueStr);
                    break;
            }
        }

        _keys = new string[_objects.Keys.Count];
        _objects.Keys.CopyTo(_keys, 0);

        _networkDatum = new NetworkData[_arrays.Keys.Count];
        _arrays.Values.CopyTo(_networkDatum, 0);
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
        bool hasInt = _ints.ContainsKey(key);
        refVar = hasInt ? _ints[key] : -1;
        return hasInt;
    }

    public bool GetFloat(string key, out float refVar)
    {
        bool hasFloat = _floats.ContainsKey(key);
        refVar = hasFloat ? _floats[key] : -1;
        return hasFloat;
    }

    public bool GetBool(string key, out bool refVar)
    {
        bool hasBool = _bools.ContainsKey(key);
        refVar = hasBool ? _bools[key] : false;
        return hasBool;
    }

    public bool GetString(string key, out string refVar)
    {
        bool hasString = _strings.ContainsKey(key);
        refVar = hasString ? _strings[key] : null;
        return hasString;
    }

    public bool GetArray(string key, out NetworkData[] refVar)
    {
        throw new System.NotImplementedException();
        return false;
    }

    #endregion

    private void HandleParseException(string msg)
    {
        Debug.LogWarning("Something cheesy is going on: "+ msg);
    }
}