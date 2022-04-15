using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[Serializable]
public struct ARCloudAnchorHistory
{
    public string AnchorName;
    public string AnchorID;
    public string SerializedTime;
    public int PrefabIndex;
    public string ObjectName;
    public string ObjectText;
    public ARAnchor arAnchor;

    //used when first queing anchor 
    public ARCloudAnchorHistory(int index, string objectName, ARAnchor anchor)
    {
        AnchorName = null;
        AnchorID = null;
        SerializedTime = null;
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = "";
        arAnchor = anchor;
    }

    public ARCloudAnchorHistory(string name, string id, DateTime time, int index, string objectName)
    {
        AnchorName = name;
        AnchorID = id;
        SerializedTime = time.ToString();
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = "";
        arAnchor = null;
    }

    public ARCloudAnchorHistory(string name, string id, DateTime time, int index, string objectName, string text)
    {
        AnchorName = name;
        AnchorID = id;
        SerializedTime = time.ToString();
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = text;
        arAnchor = null;

    }

    public DateTime CreatedTime
    {
        get
        {
            return Convert.ToDateTime(SerializedTime);
        }
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

}
[Serializable]
public class ARCloudAnchorHistoryCollection
{
    public List<ARCloudAnchorHistory> Collection = new List<ARCloudAnchorHistory>();
}


