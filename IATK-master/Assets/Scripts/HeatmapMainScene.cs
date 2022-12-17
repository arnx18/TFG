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

public class HeatmapMainScene : MonoBehaviour
{
    public InputActionReference toggleReference = null;

    private Boolean started = false;
    private int currentHeatmap;
    private Boolean triggerValue;
    private Stopwatch sw;
    private List<double> elapsedSecondsArray;

    private void Awake()
    {
        toggleReference.action.started += Toggle;
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        if (!started) {
            StartTest();
        } else {
            NextHeatmap();
        }
    }

    public void StartTest()
    {
        started = true;
        elapsedSecondsArray = new List<double>();
        SelectHeatmap(0);
        sw = new Stopwatch();
        sw.Start();
    }

    private void SelectHeatmap(int index)
    {
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public void NextHeatmap()
    {
        ++currentHeatmap;
        SelectHeatmap(currentHeatmap);
        sw.Stop();
        elapsedSecondsArray.Add(sw.Elapsed.TotalSeconds);
        if (currentHeatmap == transform.childCount) {
            printTestResults();
            SceneManager.LoadScene("MainMenu");
        } else {
            sw = new Stopwatch();
            sw.Start();
        }
    }

    private void printTestResults()
    {
        String results = "";
        foreach (float time in elapsedSecondsArray) {
            results += time.ToString() + "\n";
        }
        print(results);
    }

}
