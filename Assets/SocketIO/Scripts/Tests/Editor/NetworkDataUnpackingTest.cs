using UnityEngine;
using NUnit.Framework;
using SocketIO;

public class NetworkDataUnpackingTest
{
    /*
        TODO:
        * add test embedded array test
        * Tmp removed Escaped quoute support ""networkDataFieldEmbeddedObject"": [5, ""{\""field\"":\""Value with \\\""escaped quotes\\\""\""}""],
        * TMP removed ""networkDataFieldEmptyArray"": [4, []],
    */

    const string jsonString = @"
    [
        ""someEvent"",
          {
            ""intField"": [0, 1],
            ""floatField"": [1, 2.1],
            ""stringField"": [2, ""Blah""],
            ""null"": [2, null],
            ""boolFieldTruePlain"": [3, ""true""],
            ""boolFieldFalsePlain"": [3, ""false""],
            ""boolFieldTrue"": [3, true],
            ""boolFieldFalse"": [3, false],
            ""networkDataFieldEmptyHash"": [4, {}],
            ""networkDataField"": [4, {""TestObject1"":[4, {""id"":[0,1],""amount"":[0,11]}],""TestObject2"":[4, {""id"":[0,2],""amount"":[0,22]}],""TestObject3"":[4, {""id"":[0,3],""amount"":[0,33], ""floatAmount"":[1,33.3]}}] ]
          }
    ]
    ";

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
        //get { return new NetworkData(jsonString);  }
        get { return _networkData; }
    }

    private NetworkData childNetworkData
    {
        get { return networkData; }
        //get { return networkData["someEvent"][0]; }
    }

    #region Properties
    [Test]
    public void Getter_Raw()
    {
        Assert.IsNotEmpty(networkData.raw);
    }

    [Test]
    public void Getter_KeyCount()
    {
        Assert.AreEqual(10, childNetworkData.keyCount);
    }

    [Test]
    public void Getter_EventName()
    {
        Assert.AreEqual("someEvent", networkData.eventName);
    }

    [Test]
    public void Getter_Keys()
    {
        string[] keys = new string[] { "intField", "floatField", "stringField", "null", "boolFieldTruePlain", "boolFieldFalsePlain", "boolFieldTrue", "boolFieldFalse", "networkDataFieldEmptyHash", "networkDataField" };
        Assert.AreEqual(keys, childNetworkData.keys);
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
    public void GetString_ReturnsTrueIfThereIsAKeyWithStringValue()
    {
        string outVar;
        Assert.IsTrue(childNetworkData.GetString(stringField, out outVar));
        Assert.AreEqual("Blah", outVar);
    }
    [Test]
    public void GetString_ReturnsFalseIfKeyDoesntExist()
    {
        string outVar;
        Assert.IsFalse(childNetworkData.GetString(nonExistingField, out outVar));
    }
    [Test]
    public void GetString_ReturnsFalseIfKeyExistsButIsntBool()
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
        Assert.IsTrue(outVar);
    }

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
