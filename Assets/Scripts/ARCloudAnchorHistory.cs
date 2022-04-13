﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public struct CloudAnchorHistory
{
    public string Name;
    public string Id;
    public string SerializedTime;
    public string ObjectText;

    public CloudAnchorHistory(string name, string id, DateTime time)
    {
        Name = name;
        Id = id;
        SerializedTime = time.ToString();
        ObjectText = "";

    }

    public CloudAnchorHistory(string name, string id, DateTime time, string text)
    {
        Name = name;
        Id = id;
        SerializedTime = time.ToString();
        ObjectText = text;

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
public class CloudAnchorHistoryCollection
{
    public List<CloudAnchorHistory> Collection = new List<CloudAnchorHistory>();
}


