using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class HeightmapMainScene : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button finishButton;
    
    private int currentHeatmap;

    private Stopwatch sw;
    private List<double> elapsedSecondsArray;

    private void Awake() {
        startButton.interactable = true;
        nextButton.interactable = false;
        finishButton.interactable = false;
    }

    private void Update() {
         
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        //print(inputDevices.Count);
 
        foreach (var device in inputDevices) {

            bool triggerValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue) {
                print("Trigger button is pressed");
            }
        }
    }

    public void StartTest() {
        startButton.interactable = false;
        elapsedSecondsArray = new List<double>();
        SelectHeightmap(0);
        sw = new Stopwatch();
        sw.Start();
    }

    private void SelectHeightmap(int index) {
        nextButton.interactable = (index != transform.childCount - 1);
        finishButton.interactable = (index == transform.childCount - 1);
        
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public void NextHeightmap() {
        ++currentHeatmap;
        SelectHeightmap(currentHeatmap);
        sw.Stop();
        elapsedSecondsArray.Add(sw.Elapsed.TotalSeconds);
        sw = new Stopwatch();
        sw.Start();
    }

    public void FinishTest() {
        sw.Stop();
        elapsedSecondsArray.Add(sw.Elapsed.TotalSeconds);
        printTestResults();
    }

    private void printTestResults() {
        String results = "";
        foreach(float time in elapsedSecondsArray) {
            results += time.ToString() + "\n";
        }
        print(results);
    }

}
