using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IATK
{
    [ExecuteInEditMode]
    public class RescaleAxisLabels : MonoBehaviour {

        public float fontSize = 0.2f;

        public bool showHalfTicks = false;

        void Start() {   
            RescaleAxis();
        }

        void Update() {
            RescaleAxis();
        } 

        void RescaleAxis() {           
            Transform axisX = transform.Find("Axis x");
            Vector3 axisXLocalScale = axisX.transform.localScale;
            Transform axisZLabels = transform.Find("Axis z/AxisLabels");  
            
            Vector3 scale = GetComponent<HeightmapVisualisation>().transform.localScale;
            
            if (scale.x != 0 && scale.y != 0) {
                axisX.transform.localScale = new Vector3(scale.x / scale.y, axisXLocalScale.y, axisXLocalScale.z);          

                for(int i = 0; i < axisZLabels.childCount; ++i) {
                    Vector3 axisZLabelLocalScale = axisZLabels.transform.GetChild(i).GetChild(0).localScale;
                    axisZLabels.transform.GetChild(i).GetChild(0).localScale = new Vector3(axisZLabelLocalScale.x, scale.x / scale.y, axisZLabelLocalScale.z);
                    axisZLabels.transform.GetChild(i).GetChild(0).GetComponent<TMPro.TextMeshPro>().fontSize = fontSize;
                    Vector3 axisZAttributeLabelLocalScale = transform.Find("Axis z/AttributeLabel").localScale;
                    transform.Find("Axis z/AttributeLabel").localScale = new Vector3(scale.x / scale.y, axisZAttributeLabelLocalScale.y, axisZAttributeLabelLocalScale.z);
                }
            }

            for(int i = 1; i < axisZLabels.childCount; i += 2) {
                axisZLabels.transform.GetChild(i).gameObject.SetActive(showHalfTicks);
            } 

        }
    }
}
