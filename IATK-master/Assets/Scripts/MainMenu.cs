using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void HeatmapButton() {
        SceneManager.LoadScene("HeatmapMainScene");
    }

    public void HeightmapButton() {
        SceneManager.LoadScene("HeightmapMainScene");
    }
}
