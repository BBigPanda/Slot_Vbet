using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardItem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _numberState;
    [SerializeField] private TextMeshProUGUI _userName;
    [SerializeField] private TextMeshProUGUI _scoreValue;

    public void SetParams(string numberState, string userName,string scoreValue) {
        _numberState.text = numberState;
        _userName.text = userName;
        _scoreValue.text ="Score: " + scoreValue;
    }

}
