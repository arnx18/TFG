using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HeightmapMainScene : MonoBehaviour
{
    public InputActionReference toggleReference = null;

    private Boolean started = false;
    private int currentHeightmap;
    private Boolean triggerValue;
    private Stopwatch sw;
    private List<double> elapsedSecondsArray;

    private void Awake() {
        toggleReference.action.started += Toggle;
    }

    private void OnDestroy() {
        toggleReference.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context) {
        if (!started) {
            StartTest();
        } else {
            NextHeightmap();
        }
    }

    public void StartTest() {
        started = true;
        elapsedSecondsArray = new List<double>();
        SelectHeightmap(0);
        sw = new Stopwatch();
        sw.Start();
    }

    private void SelectHeightmap(int index) {
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public void NextHeightmap() {
        ++currentHeightmap;
        SelectHeightmap(currentHeightmap);
        sw.Stop();
        elapsedSecondsArray.Add(sw.Elapsed.TotalSeconds);
        if (currentHeightmap == transform.childCount) {
            printTestResults();
            SceneManager.LoadScene("MainMenu");
        } else {
            sw = new Stopwatch();
            sw.Start();
        }        
    }

    private void printTestResults() {
        String results = "";
        foreach(float time in elapsedSecondsArray) {
            results += time.ToString() + "\n";
        }
        print(results);
    }

}
