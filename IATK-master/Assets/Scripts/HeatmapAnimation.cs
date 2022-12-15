using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapAnimation : MonoBehaviour {
    
    private Vector3 initialPosition;

    private void Awake() {
        initialPosition = transform.position;
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(-0.5f, 0.7f, 0.5f), 0.1f);
    }

    private void OnDisable() {
        transform.position = initialPosition;
    } 
}
