using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainScene : MonoBehaviour
{
    public InputActionReference toggleReference = null;

    private Boolean started = false;

    private int currentVisualisation;

    private Boolean triggerValue;

    private Stopwatch sw;

    private List<double> elapsedSecondsArray;

    private String[] order;
    private int[] ind;

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
            NextVisualisation();
        }
    }

    private void Start() {
        order = new String[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i) {
            order[i] = transform.GetChild(i).gameObject.name;
        }
        System.Random rnd = new System.Random();
        ind = Enumerable.Range(0, transform.childCount).OrderBy(c => rnd.Next()).ToArray();
        currentVisualisation = 0;
    }

    public void StartTest()
    {
        started = true;
        elapsedSecondsArray = new List<double>();
        SelectVisualisation(order[ind[currentVisualisation]]);
        sw = new Stopwatch();
        sw.Start();
    }

    private void SelectVisualisation(String name)
    {
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(transform.GetChild(i).gameObject.name.Equals(name));
        }
    }

    public void NextVisualisation()
    {
        ++currentVisualisation;
        sw.Stop();
        elapsedSecondsArray.Add(sw.Elapsed.TotalSeconds);
        if (currentVisualisation == transform.childCount) {
            printTestResults();
            SceneManager.LoadScene("MainMenu");
        } else {
            SelectVisualisation(order[ind[currentVisualisation]]);
            sw = new Stopwatch();
            sw.Start();
        }
    }

    private void printTestResults()
    {
        String results = "";
        for (int i = 0; i < transform.childCount; ++i) {
            results += order[ind[i]] + ", " + String.Format("{0:0.0000}", elapsedSecondsArray[i]).Replace(",", ".") + "\n";
        }
        print(results);
    }

}
