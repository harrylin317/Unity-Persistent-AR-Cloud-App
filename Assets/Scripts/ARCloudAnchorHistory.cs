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
    public string TitleText;
    public string BodyText;
    public float TitleTextFontSize;
    public float BodyTextFontSize;
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
        TitleText = null;
        BodyText = null;
        TitleTextFontSize = 0;
        BodyTextFontSize = 0;
        arAnchor = null;
        ObjectScale = Vector3.zero;
    }

    //used when first queing anchor 
    public ARCloudAnchorHistory(string anchorName, int index, string objectName, ARAnchor anchor, 
        string titleText, string bodyText, float titleTextFontSize, float bodyTextFontSize, Vector3 scale)
    {
        AnchorName = anchorName;
        AnchorID = null;
        SerializedTime = null;
        PrefabIndex = index;
        ObjectName = objectName;
        TitleText = titleText;
        BodyText = bodyText;
        TitleTextFontSize = titleTextFontSize;
        BodyTextFontSize = bodyTextFontSize;
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
        TitleText = null;
        BodyText = null;
        TitleTextFontSize = 0;
        BodyTextFontSize = 0;
        arAnchor = null;
        ObjectScale = scale;

    }

    public ARCloudAnchorHistory(string anchorName, string id, DateTime time, int index, 
        string objectName, string titleText, string bodyText, float titleTextFontSize, float bodyTextFontSize, Vector3 scale)
    {
        AnchorName = anchorName;
        AnchorID = id;
        SerializedTime = time.ToString();
        PrefabIndex = index;
        ObjectName = objectName;

        TitleText = titleText;
        BodyText = bodyText;
        TitleTextFontSize = titleTextFontSize;
        BodyTextFontSize = bodyTextFontSize;

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


