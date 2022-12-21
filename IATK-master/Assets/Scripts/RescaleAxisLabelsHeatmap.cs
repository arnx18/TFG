using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IATK
{
    [ExecuteInEditMode]
    public class RescaleAxisLabelsHeatmap : MonoBehaviour {

        public float fontSize = 0.45f;

        public bool showAllTicks = true;
        public bool enableAxisX = false;
        public bool enableAxisY = false;

        void Start() {   
            RescaleAxis();
        }

        void Update() {
            //RescaleAxis();
        } 

        void RescaleAxis() {

            transform.Find("Axis x/AxisLabels").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/AttributeLabel").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/MinNormaliser").gameObject.SetActive(enableAxisX);
            transform.Find("Axis x/MaxNormaliser").gameObject.SetActive(enableAxisX);
            transform.Find("Axis y/AxisLabels").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/AttributeLabel").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/MinNormaliser").gameObject.SetActive(enableAxisY);
            transform.Find("Axis y/MaxNormaliser").gameObject.SetActive(enableAxisY);
            transform.Find("Axis z/AttributeLabel").gameObject.SetActive(false);
            transform.Find("Axis z/Tip").gameObject.SetActive(false);
            transform.Find("Axis z/MinNormaliser").gameObject.SetActive(false);
            transform.Find("Axis z/MaxNormaliser").gameObject.SetActive(false);
            transform.Find("Axis z").localScale = new Vector3(1f, 2.225f, 1f);

            Transform axisZLabels = transform.Find("Axis z/AxisLabels");
            axisZLabels.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "0";
            for(int i = 0; i < axisZLabels.childCount; ++i) {
                Vector3 axisZLabelLocalScale = axisZLabels.transform.GetChild(i).GetChild(0).localScale;
                axisZLabels.transform.GetChild(i).GetChild(0).localScale = new Vector3(axisZLabelLocalScale.x, 0.45f, axisZLabelLocalScale.z);
                axisZLabels.transform.GetChild(i).GetChild(0).GetComponent<TMPro.TextMeshPro>().fontSize = fontSize;
                axisZLabels.transform.GetChild(i).gameObject.SetActive(showAllTicks);
            }
        }
    }
}
