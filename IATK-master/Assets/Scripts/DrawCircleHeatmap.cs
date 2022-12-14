using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace IATK
{
    [ExecuteInEditMode]
    public class DrawCircleHeatmap : MonoBehaviour
    {
        [HideInInspector]
        public GameObject circle;

        [HideInInspector]
        public LineRenderer circleRenderer;

        [HideInInspector]
        public HeatmapVisualisation heatmapVisualisation;

        [HideInInspector]
        public Vector3[] positions;

        [HideInInspector]
        float[,] dataArray; 
        
        [Range(0.0242f, 0.075f)]
        public float radius = 0.0242f;

        [Range(0.003f, 0.02f)]
        public float width = 0.008f;

        public Color circleColor = Color.red;

        [Range(0,29)]
        public int x = 0;

        [Range(0,29)]
        public int y = 0;

        public int circledValue;

        [HideInInspector]
        public int steps = 50;
        
        void Start() {   

            if (circle == null) {

                circle = new GameObject("Circle");
                circle.transform.SetParent(GetComponent<HeatmapVisualisation>().transform);
                circle.AddComponent<LineRenderer>();  

                circleRenderer = circle.GetComponent<LineRenderer>();
                circleRenderer.material = new Material(Shader.Find("IATK/Heatmap"));
                circleRenderer.startColor = circleColor;
                circleRenderer.endColor = circleColor;
                circleRenderer.useWorldSpace = false;
            
                positions = GetComponent<HeatmapVisualisation>().positions;  
                getDataArray(); 
            }          
                 
            Draw();
        }

        void Update() {

            Draw();

            if (dataArray == null) {
                getDataArray();
            }
            circledValue = (int) dataArray[x * 30 + y, 2];         
        } 
        
        void getDataArray() {

            CSVDataSource dataSource = (CSVDataSource) GetComponent<Visualisation>().dataSource;
            String data = dataSource.data.ToString();

            string[] lines = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int dimensions = lines[1].Split(new char[] { ',', '\t', ';'}).Count();
            dataArray = new float[lines.Length - 1, dimensions];

            if (lines.Length > 1) {
                for (int i = 1; i < lines.Length; i++) {
                    string[] values = lines[i].Split(new char[] { ',', '\t', ';'});

                    for (int k = 0; k < values.Count(); k++) {
                        string cleanedValue = values[k].Replace("\r", string.Empty);

                        if (k <= dimensions - 1) {
                            double result = 0.0f;
                            double.TryParse(cleanedValue, out result);
                            dataArray[i - 1, k] = (float)result;
                        }
                    }
                }
            }
            
            circledValue = (int) dataArray[x * 30 + y, 2];
        }


        void Draw() {

            circle.transform.position = GetComponent<HeatmapVisualisation>().transform.position 
            + GetComponent<HeatmapVisualisation>().transform.TransformDirection(new Vector3(positions[x * 30].x, positions[y].z, -0.02f));
            circleRenderer.startColor = circleColor;
            circleRenderer.endColor = circleColor;
            circleRenderer.startWidth = width;
            circleRenderer.endWidth = width;
            circleRenderer.positionCount = steps;

            float theta = 0f;

            for(int currentStep = 0; currentStep < steps; ++currentStep) {

                float x = Mathf.Sin(Mathf.Deg2Rad * theta) * radius;
                float y = Mathf.Cos(Mathf.Deg2Rad * theta) * radius;

                Vector3 currentPosition = new Vector3(x, y, 0f);
                circleRenderer.SetPosition(currentStep, currentPosition);
                
                theta += 380f / steps;
            }
        }
    }
}
