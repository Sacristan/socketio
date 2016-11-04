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
    const string objectField = "objectField";

    const string intArrayField = "intArrayField";
    const string floatArrayField = "floatArrayField";
    const string stringArrayField = "stringArrayField";
    const string boolArrayField = "boolArrayField";

    private string expectedJSON = @"{""intField"":[0,1],""floatField"":[1,1.1],""stringField"":[2,""foobar""],""boolField"":[3,true],""intArrayField"":[5,[3,2,1]],""floatArrayField"":[6,[0,1.2,3.5]],""stringArrayField"":[7,[""cats"",""are"",""cute""]],""boolArrayField"":[8,[false,true]],""objectField"":[4,{""json"":[0,1],},{""json1"":[1,1.2],""json2"":[2,""turtles""],},]}";

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
        Assert.IsFalse(networkData.GetObject(objectField, out d));

        NetworkData childNetworkData1 = new NetworkData();
        childNetworkData1.AddInt("json", 1);

        NetworkData childNetworkData2 = new NetworkData();
        childNetworkData2.AddFloat("json1", 1.2f);
        childNetworkData2.AddString("json2", "turtles");

        networkData.AddObject(objectField, childNetworkData1);
        networkData.AddObject(objectField, childNetworkData2);
        Assert.IsTrue(networkData.GetObject(objectField, out d));
    }


    [Test]
    public void AddIntArrayField()
    {
        int[] d;
        Assert.IsFalse(networkData.GetIntArray(intArrayField, out d));
        networkData.AddIntArray(intArrayField, new int[] { 3, 2, 1 } );
        Assert.IsTrue(networkData.GetIntArray(intArrayField, out d));
    }

    [Test]
    public void AddFloatArrayField()
    {
        float[] d;
        Assert.IsFalse(networkData.GetFloatArray(floatArrayField, out d));
        networkData.AddFloatArray(floatArrayField, new float[] { 0f, 1.2f, 3.5f });
        Assert.IsTrue(networkData.GetFloatArray(floatArrayField, out d));
    }

    [Test]
    public void AddStringArrayField()
    {
        string[] d;
        Assert.IsFalse(networkData.GetStringArray(stringArrayField, out d));
        networkData.AddStringArray(stringArrayField, new string[] { "cats", "are", "cute" });
        Assert.IsTrue(networkData.GetStringArray(stringArrayField, out d));
    }

    [Test]
    public void AddBoolArrayField()
    {
        bool[] d;
        Assert.IsFalse(networkData.GetBoolArray(boolArrayField, out d));
        networkData.AddBoolArray(boolArrayField, new bool[] { false, true });
        Assert.IsTrue(networkData.GetBoolArray(boolArrayField, out d));
    }

    [Test]
    public void ShouldHavePackedJSONCorrectly()
    {
        string json = networkData.ToJSONString();
        //Debug.Log(json);

        Assert.AreEqual(expectedJSON, json);
    }
}
