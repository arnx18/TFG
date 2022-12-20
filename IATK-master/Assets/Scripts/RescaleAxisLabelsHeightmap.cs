using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IATK
{
    [ExecuteInEditMode]
    public class RescaleAxisLabelsHeightmap : MonoBehaviour {

        public float fontSize = 0.45f;
        public Vector3 tickSize = new Vector3(0.0132665f, 0.0029005f, 0.0025f);
        public bool enableAxisX = false;
        public bool enableAxisY = false;

        void Start() {   
            RescaleAxis();
        }

        void Update() {
            RescaleAxis();
        } 

        void RescaleAxis() {           
            Transform axisZLabels = transform.Find("Axis z/AxisLabels");

            transform.Find("Axis x/AxisLabels").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/AttributeLabel").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/MinNormaliser").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/MaxNormaliser").gameObject.SetActive(enableAxisX);
            transform.Find("Axis y/AxisLabels").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/AttributeLabel").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/MinNormaliser").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/MaxNormaliser").gameObject.SetActive(enableAxisY);
            transform.Find("Axis z/AttributeLabel").gameObject.SetActive(false);

            Vector3 axisZModelLocalPosition = transform.Find("Axis z/Rod/Model").localPosition;
    
            for(int i = 0; i < axisZLabels.childCount; ++i) {
                axisZLabels.transform.GetChild(i).GetChild(0).GetComponent<TMPro.TextMeshPro>().fontSize = fontSize;
                axisZLabels.transform.GetChild(i).GetChild(1).transform.localScale = tickSize;
                Vector3 tickPosition = axisZLabels.transform.GetChild(i).GetChild(1).transform.localPosition;
                axisZLabels.transform.GetChild(i).GetChild(1).transform.localPosition = new Vector3(axisZModelLocalPosition.x + 0.006f, tickPosition.y, tickPosition.z);
            }
        }
    }
}
