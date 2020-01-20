using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI name;
    [SerializeField]
    TextMeshProUGUI moves;
    [SerializeField]
    TextMeshProUGUI elapsed;
    [SerializeField]
    GameObject shells;
    [SerializeField]
    GameObject congrats;
    [SerializeField]
    Button game;
    [SerializeField]
    Button leaderboards;
    [SerializeField]
    GameObject leaderboardsPanel;

    [SerializeField]
    GameObject LEPrefab;

    

    float elapsedTime = 0;
    int numberTries = 0;
    int shellIndex = 0;

    bool gameEnded = false;

    private void Awake()
    {
        StartCoroutine(Download());
    }
    // Start is called before the first frame update
    void Start()
    {
        name.text = PlayerPrefs.GetString("PlayerName", "Player Name");
        moves.text = "Tries: " + numberTries;
        leaderboardsPanel.transform.SetSiblingIndex(0);
        
        
        
    }
    void Update()
    {
        if (!gameEnded) { 
        elapsedTime += Time.deltaTime;
        var timeSpan = TimeSpan.FromSeconds(elapsedTime);
        elapsed.text = string.Format(" Elapsed Time: {0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    public void success(Sprite s)
    {
        shells.transform.GetChild(shellIndex).GetComponent<Image>().sprite = s;
        shellIndex++;
        numberTries++;
        moves.text = "Tries: " + numberTries;
    }

    public void failure()
    {
        numberTries++;
        moves.text = "Tries: " + numberTries;
    }

    internal void endGame()
    {
        gameEnded = true;
        var points= (int)elapsedTime + numberTries;
        StartCoroutine(Upload(name.text, numberTries, (int)elapsedTime, points));
        StartCoroutine(finalScreen(points));
    }

    IEnumerator finalScreen(int points)
    {
        //yield return new WaitForSeconds(1f);
        congrats.SetActive(true);
        congrats.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text ="Points: "+ points.ToString();
        
        congrats.transform.SetAsLastSibling();
        yield return null;
    }


    IEnumerator Upload(String name , int tries, int elapsed, int points)
    {
        Debug.Log("Uploading");
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("points", points.ToString());
        form.AddField("elapsed_time", elapsed.ToString());
        form.AddField("number_of_tries", tries.ToString());
        
        ;

        UnityWebRequest www = UnityWebRequest.Post("http://3.125.144.225/leaderboards/post/", form);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
           
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }


    IEnumerator Download()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://3.125.144.225/leaderboards/get/");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
        }
        else
        {
            string results = www.downloadHandler.text.Replace(@"\", string.Empty);
            results=results.Trim('"');
            Debug.Log(results);
            LeaderboardEntry[] objects = JsonHelper.getJsonArray<LeaderboardEntry>(results);
            List<LeaderboardEntry> sortedList = objects.OrderBy(si => si.points).ToList();
            var content = GameObject.FindGameObjectWithTag("Leaderboards");
            //Scroller._arrayOfElements = new GameObject[objects.Length];
            int i = 1;
            foreach (LeaderboardEntry e in sortedList)
            {
                Debug.Log(e.time);
               setLeaderboardEntityUI(content.transform.GetChild(i).gameObject, e.name, e.points, e.time, e.number_of_moves);
                i++;
              
            }
        }
    }
    public void backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    

    private void setLeaderboardEntityUI(GameObject obj,string name, int points, int elapsedTime, int number)
    {
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = points.ToString();
        var timeSpan = TimeSpan.FromSeconds(elapsedTime); 
        obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = number.ToString();
    }

    public void GameView()
    {

        leaderboardsPanel.transform.SetSiblingIndex(0);
    }
    public void Leaderboard()
    {
        leaderboardsPanel.transform.SetSiblingIndex(3);
    }
}

public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Debug.Log(newJson);
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public int time;
    public int points;
    public int number_of_moves;

    public static LeaderboardEntry CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LeaderboardEntry>(jsonString);
    }
}
