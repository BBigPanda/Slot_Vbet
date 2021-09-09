using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mkey;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoardController : MonoBehaviour {
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private LeaderBoardItem _leaderBoardItem;
    UserDataLead TempUser = new UserDataLead();
    private int _tempScore = 0;
    private LeadsData users = new LeadsData();
    public void AddUser(string name,string score) {
        UserDataLead d = new UserDataLead();
        d.name = name;
        d.score = int.Parse(score);
        if (users.data.Find(_ => {
            if (_.name.CompareTo(name) == 0) {
                SceneManager.LoadScene(0);
                return true;
            }
            return false;
        }) != null) {
            return;
        }
        TempUser = d;
        users.data.Add(d);
        SaveLeaderBoardData();
        Debug.Log(JsonUtility.ToJson(users));
        SortUsersByScore();
    }
    public void InitUsers() {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LeaderBoard"))) {
            string jsonString = PlayerPrefs.GetString("LeaderBoard");
            users = JsonUtility.FromJson<LeadsData>(jsonString);
        }
    }

    private void SortUsersByScore() {
        users.data.Sort(SortByScore);

        for (int i = 0; i < users.data.Count; i++) {
            users.data[i].number = (i+1);
        }
    }
    static int SortByScore(UserDataLead p1, UserDataLead p2)
    {
        return p2.score.CompareTo(p1.score);
    }

    private void SaveLeaderBoardData() {
        PlayerPrefs.SetString("LeaderBoard",JsonUtility.ToJson(users));
    }

    private void UpdateView() {
        SortUsersByScore();
  Clear(ref _container);
  foreach (var user in users.data) {
      LeaderBoardItem tmp = Instantiate(_itemPrefab, _container).GetComponent<LeaderBoardItem>();
     tmp.SetParams(user.number.ToString(),user.name,user.score.ToString());
  }
    }
    public void Clear(ref Transform transform)
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
   
    public void AddScoreToTempUser(int value) {
        _tempScore += value;
        TempUser.score = _tempScore;
      
        users.data.Find(_ => {
            if (_.name.CompareTo(TempUser.name) == 0) {
                 _.score = TempUser.score;
                 TempUser.number = _.number;
                Debug.Log("FInd " +_.name);
            }

            return true;
        });
        SortUsersByScore();
        users.data.Find(_ => {
            if (_.name.CompareTo(TempUser.name) == 0) {
                TempUser.number = _.number;
            }

            return true;
        });
        _leaderBoardItem.gameObject.SetActive(true);
        _leaderBoardItem.SetParams(TempUser.number.ToString(),TempUser.name,TempUser.score.ToString());
        SaveLeaderBoardData();
        UpdateView();
    }

    private void AddNewUser(string name) {
        AddUser(name,"0");
        _leaderBoardItem.gameObject.SetActive(true);
        _leaderBoardItem.SetParams(TempUser.number.ToString(),TempUser.name,TempUser.score.ToString());
    }
    private void OnEnable() {
        StaticEvents.Score += AddScoreToTempUser;
        StaticEvents.AddUser += AddNewUser;
        StaticEvents.SceneInputState += InGame;
        InitUsers();
        UpdateView();
    }

    private void InGame(bool value) {
        if (!value) {
          
        }   
    }

    private void OnDisable() {
        StaticEvents.Score -= AddScoreToTempUser;
        StaticEvents.AddUser -= AddNewUser;

    }
}
