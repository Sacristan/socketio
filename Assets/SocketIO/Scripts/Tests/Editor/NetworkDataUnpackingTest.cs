using UnityEngine;
using NUnit.Framework;
using SocketIO;

public class NetworkDataUnpackingTest
{
    /*
        TODO:
        * Tmp removed Escaped quoute support ""networkDataFieldEmbeddedObject"": [5, ""{\""field\"":\""Value with \\\""escaped quotes\\\""\""}""],
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
            ""networkDataField"": [4, {""TestObject1"":[4, {""id"":[0,1],""amount"":[0,11]}],""TestObject2"":[4, {""id"":[0,2],""amount"":[0,22]}],""TestObject3"":[4, {""id"":[0,3],""amount"":[0,33], ""floatAmount"":[1,33.3]}}] ],
            ""emptyArrayField1"": [5, ],
            ""emptyArrayField2"": [5, []],
            ""intArrayField"": [5, [1, 2 ,3, 4, 5, 6]],
            ""floatArrayField"": [6, [0.9231, 1.2, 581.22]],
            ""stringArrayField"": [7, [""i"",""like"",""turtles""]],
            ""boolArrayField"": [8, [true, false, true]]
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

    const string intArrayField = "intArrayField";
    const string floatArrayField = "floatArrayField";
    const string stringArrayField = "stringArrayField";
    const string boolArrayField = "boolArrayField";

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

    #region GetObject
    [Test]
    public void GetObject_ReturnsTrueIfThereIsAKeyWithValue()
    {
        NetworkData[] outVar;
        Assert.IsTrue(childNetworkData.GetObject(networkDataField, out outVar));
    }

    public void GetObject_ReturnsEmptyArrayIfEmptyArraySymbols()
    {
        NetworkData[] outVar;
        childNetworkData.GetObject(networkDataFieldEmptyArray, out outVar);
        Assert.AreEqual(outVar, new NetworkData[0]);
    }

    public void GetObject_ReturnsEmptyArrayIfEmptyHashSymbols()
    {
        NetworkData[] outVar;
        childNetworkData.GetObject(networkDataFieldEmptyHash, out outVar);
        Assert.AreEqual(outVar, new NetworkData[0]);
    }

    public void GetObject_ReturnsCorrectDataForEmbeddedObject()
    {
        NetworkData[] outVar;
        childNetworkData.GetObject(networkDataFieldEmbeddedObject, out outVar);
        Assert.AreEqual(outVar.Length, 1);
    }

    public void GetObject_ReturnsCorrectDataForObject()
    {
        NetworkData[] outVar;
        childNetworkData.GetObject(networkDataField, out outVar);
        Assert.AreEqual(outVar.Length, 3);
    }

    [Test]
    public void GetObject_ReturnsFalseIfKeyDoesntExist()
    {
        NetworkData[] outVar;
        Assert.IsFalse(childNetworkData.GetObject(nonExistingField, out outVar));
    }
    [Test]
    public void GetObject_ReturnsFalseIfKeyExistsButIsntInt()
    {
        NetworkData[] outVar;
        Assert.IsFalse(childNetworkData.GetObject(floatField, out outVar));
    }
    #endregion

    #region GetIntArray
    [Test]
    public void GetIntArray_ReturnsTrueIfThereIsAKeyWithValue()
    {
        int[] outVar;
        Assert.IsTrue(childNetworkData.GetIntArray(intArrayField, out outVar));
        Assert.AreEqual(outVar, 1);
    }
    [Test]
    public void GetIntArray_ReturnsFalseIfKeyDoesntExist()
    {
        int[] outVar;
        Assert.IsFalse(childNetworkData.GetIntArray(nonExistingField, out outVar));
    }
    [Test]
    public void GetIntArray_ReturnsFalseIfKeyExistsButIsntType()
    {
        int[] outVar;
        Assert.IsFalse(childNetworkData.GetIntArray(intField, out outVar));
    }
    #endregion

    #region GetFloatArray
    [Test]
    public void GetFloatArray_ReturnsTrueIfThereIsAKeyWithValue()
    {
        float[] outVar;
        Assert.IsTrue(childNetworkData.GetFloatArray(floatArrayField, out outVar));
        Assert.AreEqual(outVar, 1);
    }
    [Test]
    public void GetFloatArray_ReturnsFalseIfKeyDoesntExist()
    {
        float[] outVar;
        Assert.IsFalse(childNetworkData.GetFloatArray(nonExistingField, out outVar));
    }
    [Test]
    public void GetFloatArray_ReturnsFalseIfKeyExistsButIsntType()
    {
        float[] outVar;
        Assert.IsFalse(childNetworkData.GetFloatArray(intField, out outVar));
    }
    #endregion

    #region GetStringArray
    [Test]
    public void GetStringArray_ReturnsTrueIfThereIsAKeyWithValue()
    {
        string[] outVar;
        Assert.IsTrue(childNetworkData.GetStringArray(stringArrayField, out outVar));
        Assert.AreEqual(outVar, 1);
    }
    [Test]
    public void GetStringArray_ReturnsFalseIfKeyDoesntExist()
    {
        string[] outVar;
        Assert.IsFalse(childNetworkData.GetStringArray(nonExistingField, out outVar));
    }
    [Test]
    public void GetStringArray_ReturnsFalseIfKeyExistsButIsntType()
    {
        string[] outVar;
        Assert.IsFalse(childNetworkData.GetStringArray(intField, out outVar));
    }
    #endregion

    #region GetBoolArray
    [Test]
    public void GetBoolArray_ReturnsTrueIfThereIsAKeyWithValue()
    {
        bool[] outVar;
        Assert.IsTrue(childNetworkData.GetBoolArray(stringArrayField, out outVar));
        Assert.AreEqual(outVar, 1);
    }
    [Test]
    public void GetBoolArray_ReturnsFalseIfKeyDoesntExist()
    {
        bool[] outVar;
        Assert.IsFalse(childNetworkData.GetBoolArray(nonExistingField, out outVar));
    }
    [Test]
    public void GetBoolArray_ReturnsFalseIfKeyExistsButIsntType()
    {
        bool[] outVar;
        Assert.IsFalse(childNetworkData.GetBoolArray(intField, out outVar));
    }
    #endregion

}
