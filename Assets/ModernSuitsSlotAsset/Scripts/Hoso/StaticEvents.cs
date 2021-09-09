using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEvents : MonoBehaviour {

    public static Action<int> Score;

    public static Action<string> AddUser;

    public static Action<bool> SceneInputState;
    // Start is called before the first frame update
    public static void AddScore(int score) {
        Score?.Invoke(score);
    }

    public static void AddNewUser(string name) {
        AddUser?.Invoke(name);
    }

    public static void SetSceneInputState(bool value) {
        SceneInputState?.Invoke(value);
    }
}
