using UnityEngine;
using NUnit.Framework;
using SocketIO;

public class NetworkDataPackingTest
{
    NetworkData _networkData = new NetworkData();

    private NetworkData networkData
    {
        get { return _networkData; }
    }

    const string intField = "intField";
    const string floatField = "floatField";
    const string stringField = "stringField";
    const string boolField = "boolField";
    const string arrayField = "arrayField";

    //private string expectedJSON = @"[""event"",{""intField"":[0,1],""floatField"":[1,1.1],""stringField"":[2,""foobar"",""boolField"":[3,true],""arrayField"":[4,[""embeddedData"",{""json"":[0,1]}]]]}]";
    private string expectedJSON = @"{""intField"":[0,1],""floatField"":[1,1.1],""stringField"":[2,""foobar""],""boolField"":[3,true],""arrayField"":[4,{""json"":[0,1],},{""json1"":[1,1.2],""json2"":[2,""turtles""],},]}";

    [Test]
    public void AddIntField()
    {
        int d;
        Assert.IsFalse(networkData.GetInt(intField, out d));
        networkData.AddInt(intField, 1);
        Assert.IsTrue(networkData.GetInt(intField, out d));
    }

    [Test]
    public void AddFloatField()
    {
        float d;
        Assert.IsFalse(networkData.GetFloat(floatField, out d));
        networkData.AddFloat(floatField, 1.1f);
        Assert.IsTrue(networkData.GetFloat(floatField, out d));
    }

    [Test]
    public void AddStringField()
    {
        string d;
        Assert.IsFalse(networkData.GetString(stringField, out d));
        networkData.AddString(stringField, "foobar");
        Assert.IsTrue(networkData.GetString(stringField, out d));
    }

    [Test]
    public void AddBoolField()
    {
        bool d;
        Assert.IsFalse(networkData.GetBool(boolField, out d));
        networkData.AddBool(boolField, true);
        Assert.IsTrue(networkData.GetBool(boolField, out d));
    }

    [Test]
    public void AddArrayField()
    {
        NetworkData[] d;
        Assert.IsFalse(networkData.GetObject(arrayField, out d));

        NetworkData childNetworkData1 = new NetworkData();
        childNetworkData1.AddInt("json", 1);

        NetworkData childNetworkData2 = new NetworkData();
        childNetworkData2.AddFloat("json1", 1.2f);
        childNetworkData2.AddString("json2", "turtles");

        networkData.AddObject(arrayField, childNetworkData1);
        networkData.AddObject(arrayField, childNetworkData2);
        Assert.IsTrue(networkData.GetObject(arrayField, out d));
    }


    [Test]
    public void ShouldHavePackedJSONCorrectly()
    {
        string json = networkData.ToJSONString();
        //Debug.Log(json);

        Assert.AreEqual(expectedJSON, json);
    }
}
