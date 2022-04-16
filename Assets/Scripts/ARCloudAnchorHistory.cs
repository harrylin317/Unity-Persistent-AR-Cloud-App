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
    public Vector3 ObjectScale;

    //used to create empty struct
    public ARCloudAnchorHistory(int empty)
    {
        AnchorName = null;
        AnchorID = null;
        SerializedTime = null;
        PrefabIndex = 0;
        ObjectName = null;
        ObjectText = null;
        arAnchor = null;
        ObjectScale = Vector3.zero;
    }

    //used when first queing anchor 
    public ARCloudAnchorHistory(string anchorName, int index, string objectName, ARAnchor anchor, Vector3 scale)
    {
        AnchorName = anchorName;
        AnchorID = null;
        SerializedTime = null;
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = null;
        arAnchor = anchor;
        ObjectScale = scale;
    }

    public ARCloudAnchorHistory(string anchorName, string id, DateTime time, int index, string objectName, Vector3 scale)
    {
        AnchorName = anchorName;
        AnchorID = id;
        SerializedTime = time.ToString();
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = null;
        arAnchor = null;
        ObjectScale = scale;

    }

    public ARCloudAnchorHistory(string anchorName, string id, DateTime time, int index, string objectName, string text, Vector3 scale)
    {
        AnchorName = anchorName;
        AnchorID = id;
        SerializedTime = time.ToString();
        PrefabIndex = index;
        ObjectName = objectName;
        ObjectText = text;
        arAnchor = null;
        ObjectScale = scale;


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


