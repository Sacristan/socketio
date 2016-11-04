namespace SocketIO
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class NetworkDataType
    {
        public const int INT = 0;
        public const int FLOAT = 1;
        public const int STRING = 2;
        public const int BOOL = 3;
        public const int OBJECT = 4;
        public const int INT_ARRAY = 5;
        public const int FLOAT_ARRAY = 6;
        public const int STRING_ARRAY = 7;
        public const int BOOL_ARRAY = 8;
    }

    public class NetworkData
    {
        static readonly char[] WHITESPACE_CHARS = new[] { ' ', '\r', '\n', '\t' };

        #region Cache

        private List<string> _keys = new List<string>(); // holds all keys
        private List<string> _objectKeys = new List<string>(); // holds all keys

        private Dictionary<string, int> _ints = new Dictionary<string, int>(); // holds all ints
        private Dictionary<string, float> _floats = new Dictionary<string, float>(); // holds all floats
        private Dictionary<string, string> _strings = new Dictionary<string, string>(); // holds all strings
        private Dictionary<string, bool> _bools = new Dictionary<string, bool>(); //holds all bools

        private Dictionary<string, List<NetworkData>> _objects = new Dictionary<string, List<NetworkData>>();  //Holds all Network data objects

        private Dictionary<string, int[]> _intArrays = new Dictionary<string, int[]>(); // holds all int arrays
        private Dictionary<string, float[]> _floatArrays = new Dictionary<string, float[]>(); // holds all float arrays
        private Dictionary<string, string[]> _stringArrays = new Dictionary<string, string[]>(); // holds all float arrays
        private Dictionary<string, bool[]> _boolArrays = new Dictionary<string, bool[]>(); // holds all float arrays
        #endregion

        #region Public Properties

        public string eventName { get; private set; }
        public string raw { get; private set; }
        public string[] keys { get { return _keys.ToArray(); } }
        public int keyCount { get { return _keys.Count; } }
        public int Count { get { return keyCount; } }

        public int networkDataCount { get { return _objects.Count; } }

        //TODO: Add some safety here
        public NetworkData[] this[int idx]
        {
            get
            {
                string key = _objectKeys[idx];
                return GetObject(key);
            }
        }

        public NetworkData[] this[string key]
        {
            get
            {
                return GetObject(key);
            }
        }

        #endregion

        #region Constructors

        public NetworkData() { }
        public NetworkData(string rawJSON)
        {
            CreateFrom(rawJSON);
        }

        #endregion

        #region JSON parsing

        private void CreateFrom(string rawJSON)
        {
            Logger.Log("NetworkData received RAW: " + rawJSON);
            raw = System.Text.RegularExpressions.Regex.Replace(rawJSON, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            raw = raw.Trim(WHITESPACE_CHARS);

            Logger.Log("Raw: " + raw);

            Dictionary<string, object> parsedJSONDict = JSONDictParser.ParseJSON(raw);

            CreateFrom(parsedJSONDict);
        }

        private void CreateFrom(Dictionary<string, object> dict)
        {
            Logger.Log("NetworkData received DICT with " + dict.Count + " keys!");

            foreach (KeyValuePair<string, object> keyValuePair in dict)
            {
                Logger.Log(keyValuePair);

                string key = keyValuePair.Key;
                object value = keyValuePair.Value;

                Logger.Log(string.Format("Key: {0}", key));
                Logger.Log(string.Format("Value: {0} {1}", value, value.GetType()));

                if (IsString(value))
                {
                    Logger.LogError("This Shouldnt happen. This is probably because of no added data type int. Data: " + value);
                }

                else if (IsList(value))
                {
                    Logger.Log("Create Received Array");

                    List<object> arr = value as List<object>;

                    for (int i = 0; i < arr.Count; i++)
                    {
                        object element = arr[i];

                        try
                        {
                            Parse(element);
                        }
                        catch (Exception e)
                        {
                            Logger.LogException("NetworkData Dict Parse:", e);
                        }
                    }

                }
                else if (IsDictionary(value))
                {
                    Logger.LogWarning("Create Received Dictionary. Data wont be processed as this format is not accepted. Data: " + value);
                }

            }

        }

        private void Parse(object element)
        {
            if (IsString(element))
            {
                string key = ObjToString(element);
                Logger.Log("Received key: " + key);
                this.eventName = key;
            }
            else if (IsList(element))
            {
                List<object> arr = ObjToList(element);
                Logger.LogWarning("WARNING: array at data root: " + arr.ToArray());
            }
            else if (IsDictionary(element))
            {
                Dictionary<string, object> dict = ObjToDict(element);

                Logger.Log("Received Dictionary with " + dict.Count + " elements!");

                foreach (KeyValuePair<string, object> keyValuePair in dict)
                {
                    GatherDataFromKeyValuePair(keyValuePair);
                }

            }
            else
            {
                Logger.LogError("Received unsupported element type: " + element.GetType());
            }
        }

        private void GatherDataFromKeyValuePair(KeyValuePair<string, object> keyValuePair)
        {
            Logger.Log(">>> Parsing : " + keyValuePair);

            string key = keyValuePair.Key;
            object value = keyValuePair.Value;

            if (value == null)
            {
                Logger.LogWarning(string.Format("Received null. key: {0} data: {1}", key, value));
                return;
            }

            List<object> valueArr = ObjToList(value);

            if (valueArr != null)
            {
                if (valueArr.Count < 1)
                {
                    Logger.Log(string.Format("Key: {0} Data: {1}", key, valueArr.ToArray()));
                }

                else if (valueArr.Count == 1)
                {
                    string dataType = ObjToString(valueArr[0]);
                    Logger.LogWarning("Just datatype received. No data present!");
                    AddToDictionaries(key, null, dataType);
                }

                else if (valueArr.Count == 2)
                {
                    string dataType = ObjToString(valueArr[0]);
                    object data = valueArr[1];

                    Logger.Log("Data: " + data + " type: " + data.GetType());

                    AddToDictionaries(key, data, dataType);
                }
                else
                {
                    //TODO: ISSUE JSONDictParser is taking key, opening array and only reading lowest level array. No nested arr support? 
                    Logger.LogWarning(string.Format("Received data must be have type and value. Key: {0} data: {1} dataType: {2} Count: {3} valueArr: {4}", key, value, value.GetType(), valueArr.Count, valueArr.ToArray()));

                    foreach (object item in valueArr)
                    {
                        Logger.LogWarning(item);
                    }

                }
            }
            else
            {
                Logger.LogWarning(string.Format("WARNING: Incorrect key {0} field value format - should be Array [dataTypeInt, data]. Instead received (plain?) Data: {1}", key, value));
            }

        }

        private void AddToDictionaries(string key, object data, string dataType)
        {
            Logger.Log(string.Format("AddToDictionaries Received key: {0} data: {1} dataType: {2}", key, data, dataType));

            int dataTypeI = int.Parse(dataType); //FIXME: Possible error while parsing string
            string dataStr = ObjToString(data);

            switch (dataTypeI)
            {
                case NetworkDataType.INT:
                    AddToInts(key, int.Parse(dataStr));
                    break;
                case NetworkDataType.FLOAT:
                    AddToFloats(key, float.Parse(dataStr));
                    break;
                case NetworkDataType.STRING:
                    AddToStrings(key, dataStr);
                    break;
                case NetworkDataType.BOOL:
                    AddToBools(key, bool.Parse(dataStr));
                    break;
                case NetworkDataType.OBJECT:
                    AddToObjects(key, ObjToDict(data));
                    break;
                case NetworkDataType.INT_ARRAY:
                    AddToIntArrays(key, new int[0]); //TODO
                    break;
                case NetworkDataType.FLOAT_ARRAY:
                    AddToFloatArrays(key, new float[0]); //TODO
                    break;
                case NetworkDataType.STRING_ARRAY:
                    AddToStringArrays(key, new string[0]); //TODO
                    break;
                case NetworkDataType.BOOL_ARRAY:
                    AddToBoolArrays(key, new bool[0]); //TODO
                    break;
                default:
                    Logger.LogWarning("Received unsupported datatype " + dataType + " Data: " + dataStr);
                    break;
            }

        }

        #endregion

        #region Object type checkers

        private bool IsString(object obj)
        {
            return obj.GetType() == typeof(string);
        }

        private bool IsDictionary(object obj)
        {
            return obj.GetType() == typeof(Dictionary<string, object>);
        }

        private bool IsList(object obj)
        {
            return obj.GetType() == typeof(List<object>);
        }

        private string ObjToString(object obj)
        {
            return obj as string;
        }

        private List<object> ObjToList(object obj)
        {
            return obj as List<object>;
        }

        private Dictionary<string, object> ObjToDict(object obj)
        {
            return obj as Dictionary<string, object>;
        }
        #endregion

        #region Public Getter interface

        public bool HasKey(string key)
        {
            return Exists(key);
        }

        public bool Exists(string name)
        {
            return _keys.Contains(name);
        }

        public bool GetInt(string key, out int refVar)
        {
            bool hasInt = _ints.ContainsKey(key);
            refVar = hasInt ? _ints[key] : -1;
            return hasInt;
        }

        public int GetInt(string key)
        {
            int result;
            GetInt(key, out result);
            return result;
        }

        public bool GetFloat(string key, out float refVar)
        {
            bool hasFloat = _floats.ContainsKey(key);
            refVar = hasFloat ? _floats[key] : -1;
            return hasFloat;
        }

        public float GetFloat(string key)
        {
            float result;
            GetFloat(key, out result);
            return result;
        }

        public bool GetBool(string key, out bool refVar)
        {
            bool hasBool = _bools.ContainsKey(key);
            refVar = hasBool ? _bools[key] : false;
            return hasBool;
        }

        public bool GetBool(string key)
        {
            bool result;
            GetBool(key, out result);
            return result;
        }

        public bool GetString(string key, out string refVar)
        {
            bool hasString = _strings.ContainsKey(key);
            refVar = hasString ? _strings[key] : null;
            return hasString;
        }

        public string GetString(string key)
        {
            string result;
            GetString(key, out result);
            return result;
        }

        public bool GetObject(string key, out NetworkData[] refVar)
        {
            bool hasArray = _objects.ContainsKey(key);
            refVar = hasArray ? _objects[key].ToArray() : new NetworkData[0];
            return hasArray;
        }

        public NetworkData[] GetObject(string key)
        {
            NetworkData[] result;
            GetObject(key, out result);
            return result;
        }

        public bool GetIntArray(string key, out int[] refVar)
        {
            bool exists = _intArrays.ContainsKey(key);
            refVar = exists ? _intArrays[key] : null;
            return exists;
        }

        public int[] GetIntArray(string key)
        {
            int[] result;
            GetIntArray(key, out result);
            return result;
        }

        public bool GetFloatArray(string key, out float[] refVar)
        {
            bool exists = _floatArrays.ContainsKey(key);
            refVar = exists ? _floatArrays[key] : null;
            return exists;
        }

        public float[] GetFloatArray(string key)
        {
            float[] result;
            GetFloatArray(key, out result);
            return result;
        }

        public bool GetStringArray(string key, out string[] refVar)
        {
            bool exists = _stringArrays.ContainsKey(key);
            refVar = exists ? _stringArrays[key] : null;
            return exists;
        }

        public string[] GetStringArray(string key)
        {
            string[] result;
            GetStringArray(key, out result);
            return result;
        }

        public bool GetBoolArray(string key, out bool[] refVar)
        {
            bool exists = _boolArrays.ContainsKey(key);
            refVar = exists ? _boolArrays[key] : null;
            return exists;
        }

        public bool[] GetBoolArray(string key)
        {
            bool[] result;
            GetBoolArray(key, out result);
            return result;
        }

        #endregion

        #region Public Setter interface
        public bool AddInt(string key, int data)
        {
            return AddToInts(key, data);
        }

        public bool AddFloat(string key, float data)
        {
            return AddToFloats(key, data);
        }

        public bool AddString(string key, string data)
        {
            return AddToStrings(key, data);
        }

        public bool AddBool(string key, bool data)
        {
            return AddToBools(key, data);
        }

        public bool AddObject(string key, NetworkData data)
        {
            return AddToObjects(key, data);
        }

        public bool AddIntArray(string key, int[] data)
        {
            return AddToIntArrays(key, data);
        }

        public bool AddFloatArray(string key, float[] data)
        {
            return AddToFloatArrays(key, data);
        }

        public bool AddStringArray(string key, string[] data)
        {
            return AddToStringArrays(key, data);
        }

        public bool AddBoolArray(string key, bool[] data)
        {
            return AddToBoolArrays(key, data);
        }
        #endregion

        #region Adding to collections
        private bool AddToInts(string key, int data)
        {
            bool canAdd = !_ints.ContainsKey(key);

            if (canAdd) _ints.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToFloats(string key, float data)
        {
            bool canAdd = !_floats.ContainsKey(key);

            if (canAdd) _floats.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToStrings(string key, string data)
        {
            bool canAdd = !_strings.ContainsKey(key);

            if (canAdd) _strings.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToBools(string key, bool data)
        {
            bool canAdd = !_bools.ContainsKey(key);

            if (canAdd) _bools.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToObjects(string key, NetworkData networkData)
        {
            if (!_objects.ContainsKey(key)) _objects[key] = new List<NetworkData>();

            bool canAdd = !_objects[key].Contains(networkData);
            if (canAdd) _objects[key].Add(networkData);

            if (!_objectKeys.Contains(key)) _objectKeys.Add(key);
            RegisterKey(key);

            return canAdd;
        }

        private void AddToObjects(string key, Dictionary<string, object> dict)
        {
            NetworkData networkData = new NetworkData();
            networkData.CreateFrom(dict);

            AddToObjects(key, networkData);
        }

        private bool AddToIntArrays(string key, int[] data)
        {
            bool canAdd = !_intArrays.ContainsKey(key);

            if (canAdd) _intArrays.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToFloatArrays(string key, float[] data)
        {
            bool canAdd = !_floatArrays.ContainsKey(key);

            if (canAdd) _floatArrays.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private bool AddToStringArrays(string key, string[] data)
        {
            bool canAdd = !_stringArrays.ContainsKey(key);

            if (canAdd) _stringArrays.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }


        private bool AddToBoolArrays(string key, bool[] data)
        {
            bool canAdd = !_boolArrays.ContainsKey(key);

            if (canAdd) _boolArrays.Add(key, data);
            RegisterKey(key);

            return canAdd;
        }

        private void RegisterKey(string key)
        {
            if (!_keys.Contains(key)) _keys.Add(key);
        }
        #endregion

        #region Packing to JSON
        public JSONObject ToJSONObject()
        {
            return new JSONObject(ToJSONString());
        }

        public string ToJSONString()
        {
            return HashDataToJSON();
        }

        //TODO: Little refactoring could help 
        private string HashDataToJSON()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();

            jsonStringBuilder.Append('{');

            foreach (KeyValuePair<string, int> _int in _ints)
            {
                string str = string.Format(@"""{0}"":[0,{1}],", _int.Key, _int.Value);
                jsonStringBuilder.Append(str);
            }

            foreach (KeyValuePair<string, float> _float in _floats)
            {
                string str = string.Format(@"""{0}"":[1,{1}],", _float.Key, _float.Value);
                jsonStringBuilder.Append(str);
            }

            foreach (KeyValuePair<string, string> _string in _strings)
            {
                string str = string.Format(@"""{0}"":[2,""{1}""],", _string.Key, _string.Value);
                jsonStringBuilder.Append(str);
            }

            foreach (KeyValuePair<string, bool> _bool in _bools)
            {
                string str = string.Format(@"""{0}"":[3,{1}],", _bool.Key, _bool.Value.ToString().ToLower());
                jsonStringBuilder.Append(str);
            }

            foreach (KeyValuePair<string, int[]> _intArray in _intArrays)
            {
                StringBuilder strBuilder = new StringBuilder();
                int[] value = _intArray.Value;

                strBuilder.Append(string.Format(@"""{0}"":[5,[", _intArray.Key));

                for (int i = 0; i < value.Length; i++)
                {
                    strBuilder.Append("" + value[i]);
                    if (i < value.Length - 1) strBuilder.Append(',');
                }

                strBuilder.Append("]],");
                jsonStringBuilder.Append(strBuilder);
            }

            foreach (KeyValuePair<string, float[]> _floatArray in _floatArrays)
            {
                StringBuilder strBuilder = new StringBuilder();
                float[] value = _floatArray.Value;

                strBuilder.Append(string.Format(@"""{0}"":[6,[", _floatArray.Key));

                for (int i = 0; i < value.Length; i++)
                {
                    strBuilder.Append("" + value[i]);
                    if (i < value.Length - 1) strBuilder.Append(',');
                }

                strBuilder.Append("]],");
                jsonStringBuilder.Append(strBuilder);
            }
            foreach (KeyValuePair<string, string[]> _stringArray in _stringArrays)
            {
                StringBuilder strBuilder = new StringBuilder();
                string[] value = _stringArray.Value;

                strBuilder.Append(string.Format(@"""{0}"":[7,[", _stringArray.Key));

                for (int i = 0; i < value.Length; i++)
                {
                    strBuilder.Append(string.Format(@"""{0}""", value[i]));
                    if (i < value.Length - 1) strBuilder.Append(',');
                }

                strBuilder.Append("]],");
                jsonStringBuilder.Append(strBuilder);
            }
            foreach (KeyValuePair<string, bool[]> _boolArray in _boolArrays)
            {
                StringBuilder strBuilder = new StringBuilder();
                bool[] value = _boolArray.Value;

                strBuilder.Append(string.Format(@"""{0}"":[8,[", _boolArray.Key));

                for (int i = 0; i < value.Length; i++)
                {
                    strBuilder.Append("" + value[i].ToString().ToLower());
                    if (i < value.Length - 1) strBuilder.Append(',');
                }

                strBuilder.Append("]],");
                jsonStringBuilder.Append(strBuilder);
            }

            StringBuilder objectStringBuilder = new StringBuilder();
            foreach (string key in _objects.Keys)
            {

                objectStringBuilder.Append(string.Format(@"""{0}"":", key));

                objectStringBuilder.Append("[4,");

                NetworkData[] arr = _objects[key].ToArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    NetworkData networkData = arr[i];
                    objectStringBuilder.Append(networkData.ToJSONString());
                    objectStringBuilder.Append(',');
                }

                objectStringBuilder.Append(']');
            }

            jsonStringBuilder.Append(objectStringBuilder.ToString());


            jsonStringBuilder.Append('}');

            return jsonStringBuilder.ToString();
        }


        #endregion

    }
}