using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

// Firebase things
using Firebase.Database;
using Random = UnityEngine.Random;

public class FirebaseLeaders : MonoBehaviour
{
    public TMP_Text leaderBoardString;
    public TMP_Text leaderboardEntryText;
    public GameObject leaderboardEntryObj;
    public GameObject leaderboardEntryContainer;

    public const int MaxAmountOfHighScores = 10;
    private DatabaseReference firebaseDB;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddScoreToLeaders("Test"+ Random.Range(1,100), Random.Range(1,100), firebaseDB.Child("scores"));
        }
    }

    private void Start()
    {
        // Get the root reference location of the database.
        firebaseDB = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void PlayerFinished(string name, float time)
    {
        AddScoreToLeaders(name, time, firebaseDB.Child("scores"));
    }
    async private void AddScoreToLeaders(string userName, float score, DatabaseReference leaderBoardRef)
    {
        //Passing DataSnapshot to generic task type, you get the results as a DataSnapshot:
        Task<DataSnapshot> leaderBoardTransactionTask = leaderBoardRef.RunTransaction(mutableData =>
        {
            List<object> leaders = mutableData.Value as List<object>;
            if (leaders == null)
            {
                leaders = new List<object>();
            }
            else if (mutableData.ChildrenCount >= MaxAmountOfHighScores)
            {
                float maxScore = float.MinValue;
                object maxVal = null;
                foreach (var child in leaders)
                {
                    if (!(child is Dictionary<string, object>)) continue;
                    object testScore = ((Dictionary<string, object>)child)["score"];
                    Debug.Log(testScore);
                    float childScore = Convert.ToSingle(testScore);
                    if (childScore > maxScore)
                    {
                        maxScore = childScore;
                        maxVal = child;
                    }
                }
                if (maxScore < score)
                {
                    Debug.Log(maxScore + " " + score);
                    // The new score is higher than the existing scores, abort.
                    Debug.Log("Higher than existing scores, don't add!");
                    return TransactionResult.Abort();
                }
                // Remove the highest score.
                leaders.Remove(maxVal);
                Debug.Log("Score removed");
            }
            Debug.Log(mutableData.ChildrenCount);
            // Add the new high score.
            Debug.Log("Added new high score!");
            Dictionary<string, object> newScoreMap = new Dictionary<string, object>();
            newScoreMap["score"] = score;
            newScoreMap["user"] = userName;
            leaders.Add(newScoreMap);
            mutableData.Value = leaders;
            return TransactionResult.Success(mutableData);
        });
        try
        {
            //Await for asynchronous task to complete (or to throw error e.g. if user doesn't exist...
            await leaderBoardTransactionTask;
        }
        catch (AggregateException ae)
        {
            Debug.Log(ae.Message);
            return;
        }
        //Get DataSnapshot from transaction Result:
        DataSnapshot transactionResult = leaderBoardTransactionTask.Result;
        //Get JSON from DataSnapshot:
        string resultingJSON = transactionResult.GetRawJsonValue();
        //Format JSON properly so that JsonUtility can instantiate objects:
        resultingJSON = "{\"leaderboards\":" + resultingJSON + "}";
        Debug.Log("JSON Result:\n" + resultingJSON);
        //Instantiate Leaderboard that then holds list of LeaderboardEntry instances:
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(resultingJSON);
        //Now that we have instances of objects it's trivial to show leaderboards in UI:
        leaderboard.leaderboards = leaderboard.leaderboards.OrderBy(entry => entry.score).ToList();

        // Empty the leadeboardContainer from entries
        /*
        foreach (Transform child in leaderboardEntryContainer.transform)
            Destroy(child.gameObject);
        */
        string scoreString = "";
        leaderboard.leaderboards.ToList().ForEach(lbEntry => {
            scoreString += lbEntry.user + " | " + HelperFunctions.TimeConvertToString(lbEntry.score) + "\n";
        });
        
        leaderBoardString.text = scoreString;
        
        // SetLeaderBoardString(scoreString);
        Debug.Log(scoreString);
    }
    private void SetLeaderBoardString(string scoreString)
    {
        leaderBoardString.text = scoreString;

        int index = 0;
        string tempString = "";

        // Instantiate every entry as an GameObject
        foreach (char ch in scoreString)
        {
            tempString += ch;
            if (ch == '\n')
            {
                index++;
                tempString = tempString.Trim('\n');
                tempString = tempString.Insert(0, index.ToString() + " ");
                GameObject newEntryObj = Instantiate(leaderboardEntryObj, leaderboardEntryContainer.transform);
                TMP_Text newEntry1 = newEntryObj.GetComponentInChildren<TMP_Text>();
                newEntry1.text = tempString;
                tempString = "";
            }
        }
    }
}
[Serializable]
public class Leaderboard
{
    public List<LeaderboardEntry> leaderboards = new List<LeaderboardEntry>();
}
[Serializable]
public class LeaderboardEntry
{
    public string user;
    public float score;
    public LeaderboardEntry(string _user, float _score)
    {
        user = _user;
        score = _score;
    }
}
