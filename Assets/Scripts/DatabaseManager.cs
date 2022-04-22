using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private string userID;
    private DatabaseReference dbReference;
    public ARCloudAnchorHistoryCollection loadedHistory = new ARCloudAnchorHistoryCollection();

    private bool savingInProgress = false;
    private bool deletingInProgress = false;
    private bool loadingInProgress = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

    }


    public IEnumerator DBSaveHistory(ARCloudAnchorHistory history, Action callback)
    {

        string historyJson = JsonUtility.ToJson(history);

        var dbTask = dbReference.Child("users").Child(userID).Child(history.AnchorName).SetRawJsonValueAsync(historyJson);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log("Failed to save data");
            savingInProgress = false;

        }
        else
        {
            Debug.Log("Save finished");
            savingInProgress = false;
            callback.Invoke();
        }
    }
    public IEnumerator DBRemoveHistory(string saveName, Action callback)
    {

        var dbTask = dbReference.Child("users").Child(userID).Child(saveName).RemoveValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log("Failed to remove data");
            deletingInProgress = false;

        }
        else
        {
            Debug.Log("Successfully removed");
            deletingInProgress = false;
            callback.Invoke();

        }
    }

    private IEnumerator DBLoadHistory(Action callback)
    {
        var dbTask = dbReference.Child("users").Child(userID).GetValueAsync();

        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log($"Failed to register task with {dbTask.Exception}");
            loadingInProgress = false;
        }
        else if (dbTask.Result.Value == null)
        {
            Debug.Log("No data exist");
            loadingInProgress = false;
            loadedHistory = new ARCloudAnchorHistoryCollection();
            callback.Invoke();

        }
        else
        {
            ARCloudAnchorHistoryCollection newHistory = new ARCloudAnchorHistoryCollection();
            DataSnapshot snapshot = dbTask.Result;
            //iterate through save names
            foreach (DataSnapshot s in snapshot.Children)
            {
                string getHistoryJson = s.GetRawJsonValue();
                Debug.Log($"get: {getHistoryJson}");
                ARCloudAnchorHistory getHistory = JsonUtility.FromJson<ARCloudAnchorHistory>(getHistoryJson);

                newHistory.Collection.Add(getHistory);

            }

            loadedHistory = newHistory;
            loadingInProgress = false;
            callback.Invoke();

        }
    }

    public ARCloudAnchorHistoryCollection GetLoadedHistory()
    {
        Debug.Log("getting loaded history");
        return loadedHistory;
    }

    public void LoadHistory(Action callback)
    {
        if (loadingInProgress)
        {
            Debug.Log("loading in progress, wait till it finish");
            
        }
        else
        {
            Debug.Log("Loading Save from database");
            loadingInProgress = true;
            StartCoroutine(DBLoadHistory(callback));

        }

    }
    public void SaveHistory(ARCloudAnchorHistory data, Action callback)
    {

        if (savingInProgress)
        {
            Debug.Log("saving in progress, wait till it finish");
        }
        else
        {
            Debug.Log("saving to database");

            savingInProgress = true;
            StartCoroutine(DBSaveHistory(data, callback));

        }

    }

    public void DeleteHistory(string dataName, Action callback)
    {
        if (deletingInProgress)
        {
            Debug.Log("deleting in progress, wait till it finish");          
            
        }
        else
        {
            Debug.Log($"deleteing {dataName} from database");

            deletingInProgress = true;
            StartCoroutine(DBRemoveHistory(dataName, callback));
        }
    }

    
   
}
