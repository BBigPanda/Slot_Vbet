using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Button = UnityEngine.UI.Button;
using DG.Tweening;
using UnityEngine.UI;

public class InputPanelController : MonoBehaviour {
	[SerializeField] private GameObject _panel;
	[SerializeField] private Image _bkImage;
	[SerializeField] private Transform _inputPanel;
	[SerializeField] private TMP_InputField _userName;
	[SerializeField] private Button _doneButton;
	private bool _canUseReturnButton;
	private string _name = "";
	private LeadsData users = new LeadsData();

	private void OnEnable() {
		_userName.onValueChanged.AddListener(InputValueChanged);
		_doneButton.onClick.AddListener(Done);
		StaticEvents.SetSceneInputState(false);
		_canUseReturnButton = true;
		StartCoroutine(SelecField());
	}

	IEnumerator SelecField() {
		yield return new WaitForSeconds(0.2f);
		_userName.Select();
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

			PlayerPrefs.SetString("LeaderBoard", JsonUtility.ToJson(users));
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

			File.WriteAllText(
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/UserInfo.txt", fileText);
			Application.OpenURL("file:///" +
			                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
			                    "/UserInfo.txt");
			SceneManager.LoadScene(0);
		}
	}

	private void Done() {
		if (!string.IsNullOrEmpty(_name)) {
			if (_name.ToLower().CompareTo("removealll") == 0) {
				PlayerPrefs.SetString("LeaderBoard", "");
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
			_canUseReturnButton = false;
			_doneButton.gameObject.SetActive(false);
			StartCoroutine(wait());
		}
	}

	IEnumerator wait() {
		_inputPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => { _bkImage.DOFade(0, 0.5f); });
		yield return new WaitForSeconds(1);
		_panel.SetActive(false);
		StaticEvents.SetSceneInputState(true);
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.Return) && _canUseReturnButton) {
			_name = _userName.text;
			Done();
		}
	}
}