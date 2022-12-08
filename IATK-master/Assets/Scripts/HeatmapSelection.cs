using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapSelection : MonoBehaviour
{
    [SerializeField]
    private Button nextButton;
    
    private int currentHeatmap;

    private void Awake() {
        
        SelectHeatmap(0);
    }

    private void SelectHeatmap(int index) {

        nextButton.interactable = (index != transform.childCount - 1);
        
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }

    }

    public void NextHeatmap() {
        
        ++currentHeatmap;
        SelectHeatmap(currentHeatmap);
    }
}
