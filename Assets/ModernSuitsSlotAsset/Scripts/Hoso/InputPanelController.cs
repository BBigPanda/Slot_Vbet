using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Button = UnityEngine.UI.Button;

public class InputPanelController : MonoBehaviour {
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_InputField _userName;
    [SerializeField] private Button _doneButton;
    private string _name = "";
    private LeadsData users = new LeadsData();
    private void OnEnable() {
    _userName.onValueChanged.AddListener(InputValueChanged); 
    _doneButton.onClick.AddListener(Done);
    StaticEvents.SetSceneInputState(false);
    }

    private void InputValueChanged(string name) {
        _name = name;
    }

    private void RemoveZeros() {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LeaderBoard"))) {
            string jsonString = PlayerPrefs.GetString("LeaderBoard");
            users = JsonUtility.FromJson<LeadsData>(jsonString);
            for (int i = 0; i < users.data.Count; i++) {
                if (users.data[i].score <= 0) {
                    users.data.RemoveAt(i);
                    i--;
                }
            }
            PlayerPrefs.SetString("LeaderBoard",JsonUtility.ToJson(users));
        }

        SceneManager.LoadScene(0);
    
    }

    private void WirteAndOpenFile() {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LeaderBoard"))) {
            string jsonString = PlayerPrefs.GetString("LeaderBoard");
            users = JsonUtility.FromJson<LeadsData>(jsonString);
            string fileText = "";
            foreach (var user in users.data) {
                fileText += "Position: " + user.number + " \t";
                fileText += "Name: " + user.name + " \t";
                fileText += "Score: " + user.score + " \n";
            }
           
            File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"/UserInfo.txt",fileText);
            Application.OpenURL("file:///"+System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"/UserInfo.txt");
            SceneManager.LoadScene(0);
        }
    }
    private void Done() {
        if (!string.IsNullOrEmpty(_name)) {
            if (_name.ToLower().CompareTo("removealll") == 0) {
                PlayerPrefs.SetString("LeaderBoard","");
                SceneManager.LoadScene(0);
                return;
            }
            if (_name.ToLower().CompareTo("removezeroo") == 0) {
                RemoveZeros();
                return;
            }
            if (_name.ToLower().CompareTo("filee") == 0) {
                WirteAndOpenFile();
                return;
            }
            StaticEvents.AddUser(_name);
         _doneButton.gameObject.SetActive(false);
            StartCoroutine(wait());
        }
    }

    IEnumerator wait() {
        yield return new WaitForSeconds(2);   
        _panel.SetActive(false);
        StaticEvents.SetSceneInputState(true);
    }
    
}
