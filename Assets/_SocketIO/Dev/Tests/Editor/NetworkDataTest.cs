using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NetworkDataTest
{
    const string jsonString = @"{
        ""someEvent"": {
            ""intField"": 1,
            ""floatField"": 2.1,
            ""stringField"": ""Blah"",
            ""boolFieldTruePlain"": ""true"",
            ""boolFieldFalsePlain"": ""false"",
            ""boolFieldTrue"": true,
            ""boolFieldFalse"": false,
            ""null"": null,
            ""networkDataFieldEmptyArray"": [ ],
            ""networkDataFieldEmptyHash"": { },
            ""networkDataFieldEmbeddedObject"": ""{\""field\"":\""Value with \\\""escaped quotes\\\""\""}"",
            ""networkDataField"": {
            ""TestObject1"": {
                ""id"": 1,
                ""amount"": 11,
            },
            ""TestObject2"": {
                ""id"": 2,
                ""amount"": 22,
            },
            ""TestObject3"": {
                ""id"": 3,
                ""amount"": 33,
            },
            },
        } ";

    const string header = "someEvent";

    const string nonExistingField = "nonExistingField";
    const string nullField = "null";

    const string intField = "intField";
    const string floatField = "floatField";
    const string stringField = "stringField";

    const string boolFieldTruePlain = "boolFieldTruePlain";
    const string boolFieldFalsePlain = "boolFieldFalsePlain";
    const string boolFieldTrue = "boolFieldTrue";
    const string boolFieldFalse = "boolFieldFalse";

    const string networkDataFieldEmptyArray = "networkDataFieldEmptyArray";
    const string networkDataFieldEmptyHash = "networkDataFieldEmptyHash";
    const string networkDataFieldEmbeddedObject = "networkDataFieldEmbeddedObject";
    const string networkDataField = "networkDataField";

    NetworkData _networkData = new NetworkData(jsonString);

    private NetworkData networkData
    {
        get { return _networkData; }
    }

    private NetworkData childNetworkData
    {
        get { return networkData[0]; }
    }

    #region Properties
    [Test]
    public void Getter_Raw()
    {
        Assert.AreEqual(jsonString, networkData.raw);
    }

    [Test]
    public void Getter_Header()
    {
        Assert.AreEqual(header, networkData.header);
    }

    [Test]
    public void Getter_KeyCount()
    {
        Assert.AreEqual(12, childNetworkData.keyCount);
    }

    [Test]
    public void Getter_Keys()
    {
        string[] keys = new string[] { "intField", "floatField", "stringField", "boolFieldTruePlain", "boolFieldFalsePlain", "boolFieldTrue", "boolFieldFalse", "null", "networkDataFieldEmptyArray", "networkDataFieldEmptyHash", "networkDataFieldEmbeddedObject", "networkDataField" };
        Assert.AreEqual(childNetworkData.keys, keys);
    }
    #endregion

    #region Exists

    [Test]
    public void ExistsReturnsTrueOnExistingKey()
    {
        Assert.IsTrue(childNetworkData.Exists(intField));
    }

    [Test]
    public void ExistsReturnsFalseOnNonExistingKey()
    {
        Assert.IsFalse(childNetworkData.Exists(nonExistingField));
    }

    #endregion

    #region GetInt
    [Test]
    public void GetInt_ReturnsTrueIfThereIsAKeyWithIntegerValue()
    {
        int outVar;
        Assert.IsTrue(childNetworkData.GetInt(intField, out outVar));
        Assert.AreEqual(outVar, 1);
    }
    [Test]
    public void GetInt_ReturnsFalseIfKeyDoesntExist()
    {
        int outVar;
        Assert.IsFalse(childNetworkData.GetInt(nonExistingField, out outVar));
    }
    [Test]
    public void GetInt_ReturnsFalseIfKeyExistsButIsntInt()
    {
        int outVar;
        Assert.IsFalse(childNetworkData.GetInt(floatField, out outVar));
    }

    [Test]
    public void GetInt_ReturnsFalseIfKeyExistsButIsNull()
    {
        int outVar;
        Assert.IsFalse(childNetworkData.GetInt(nullField, out outVar));
    }
    #endregion

    #region GetFloat
    [Test]
    public void GetFloat_ReturnsTrueIfThereIsAKeyWithFloatValue()
    {
        float outVar;
        Assert.IsTrue(childNetworkData.GetFloat(floatField, out outVar));
        Assert.IsTrue(Mathf.Approximately(outVar, 2.1f));
    }
    [Test]
    public void GetFloat_ReturnsFalseIfKeyDoesntExist()
    {
        float outVar;
        Assert.IsFalse(childNetworkData.GetFloat(nonExistingField, out outVar));
    }
    [Test]
    public void GetFloat_ReturnsFalseIfKeyExistsButIsntInt()
    {
        float outVar;
        Assert.IsFalse(childNetworkData.GetFloat(intField, out outVar));
    }
    #endregion

    #region GetString
    [Test]
    public void GetString_ReturnsTrueIfThereIsAKeyWithFloatValue()
    {
        string outVar;
        Assert.IsTrue(childNetworkData.GetString(stringField, out outVar));
        Assert.AreEqual(outVar, "Blah");
    }
    [Test]
    public void GetString_ReturnsFalseIfKeyDoesntExist()
    {
        string outVar;
        Assert.IsFalse(childNetworkData.GetString(nonExistingField, out outVar));
    }
    [Test]
    public void GetString_ReturnsFalseIfKeyExistsButIsntInt()
    {
        string outVar;
        Assert.IsFalse(childNetworkData.GetString(boolFieldTruePlain, out outVar));
    }
    #endregion

    #region GetBool
    [Test]
    public void GetBool_ReturnsTrueIfThereIsAKeyWithBoolValue()
    {
        bool outVar;
        Assert.IsTrue(childNetworkData.GetBool(boolFieldTruePlain, out outVar));
        Assert.AreEqual(outVar, true);
    }

    [Test]
    public void GetBool_CorrectlyParsesDataIfPlainTrue()
    {
        bool outVar;
        childNetworkData.GetBool(boolFieldTruePlain, out outVar);
        Assert.IsTrue(outVar);
    }

    [Test]
    public void GetBool_CorrectlyParsesDataIfPlainFalse()
    {
        bool outVar;
        childNetworkData.GetBool(boolFieldFalsePlain, out outVar);
        Assert.IsFalse(outVar);
    }

    [Test]
    public void GetBool_CorrectlyParsesDataIfBoolValTrue()
    {
        bool outVar;
        childNetworkData.GetBool(boolFieldTrue, out outVar);
        Assert.IsTrue(outVar);
    }

    public void GetBool_CorrectlyParsesDataIfBoolValFalse()
    {
        bool outVar;
        childNetworkData.GetBool(boolFieldFalse, out outVar);
        Assert.IsFalse(outVar);
    }

    [Test]
    public void GetBool_ReturnsFalseIfKeyDoesntExist()
    {
        bool outVar;
        Assert.IsFalse(childNetworkData.GetBool(nonExistingField, out outVar));
    }

    [Test]
    public void GetBool_ReturnsFalseIfKeyExistsButIsntInt()
    {
        bool outVar;
        Assert.IsFalse(childNetworkData.GetBool(intField, out outVar));
    }
    #endregion

    #region GetArray
    [Test]
    public void GetArray_ReturnsTrueIfThereIsAKeyWithIntegerValue()
    {
        NetworkData[] outVar;
        Assert.IsTrue(childNetworkData.GetArray(networkDataField, out outVar));
    }

    public void GetArray_ReturnsEmptyArrayIfEmptyArraySymbols()
    {
        NetworkData[] outVar;
        childNetworkData.GetArray(networkDataFieldEmptyArray, out outVar);
        Assert.AreEqual(outVar, new NetworkData[0]);
    }

    public void GetArray_ReturnsEmptyArrayIfEmptyHashSymbols()
    {
        NetworkData[] outVar;
        childNetworkData.GetArray(networkDataFieldEmptyHash, out outVar);
        Assert.AreEqual(outVar, new NetworkData[0]);
    }

    public void GetArray_ReturnsCorrectDataForEmbeddedObject()
    {
        NetworkData[] outVar;
        childNetworkData.GetArray(networkDataFieldEmbeddedObject, out outVar);
        Assert.AreEqual(outVar.Length, 1);
    }

    public void GetArray_ReturnsCorrectDataForObject()
    {
        NetworkData[] outVar;
        childNetworkData.GetArray(networkDataField, out outVar);
        Assert.AreEqual(outVar.Length, 3);
    }

    [Test]
    public void GetArray_ReturnsFalseIfKeyDoesntExist()
    {
        NetworkData[] outVar;
        Assert.IsFalse(childNetworkData.GetArray(nonExistingField, out outVar));
    }
    [Test]
    public void GetArray_ReturnsFalseIfKeyExistsButIsntInt()
    {
        NetworkData[] outVar;
        Assert.IsFalse(childNetworkData.GetArray(floatField, out outVar));
    }
    #endregion

}
