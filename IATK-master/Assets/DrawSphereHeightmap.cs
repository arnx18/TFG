using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace IATK
{
    [ExecuteInEditMode]
    public class DrawSphereHeightmap : MonoBehaviour
    {
        [HideInInspector]
        public GameObject sphere;

        [HideInInspector]
        public MeshRenderer sphereRenderer;

        [HideInInspector]
        public HeatmapVisualisation heatmapVisualisation;

        [HideInInspector]
        public Vector3[] positions;

        [HideInInspector]
        float[,] dataArray; 
        
        [Range(0.025f, 0.075f)]
        public float radius = 0.055f;

        [Range(0.25f, 1f)]
        public float transparency = 0.5f;

        public Color sphereColor = Color.red;

        [Range(0,29)]
        public int x = 0;

        [Range(0,29)]
        public int z = 0;

        public int sphereValue;

        [HideInInspector]
        public int steps = 50;
        
        void Start() {   

            if (sphere == null) {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(GetComponent<HeightmapVisualisation>().transform);

                sphereRenderer = sphere.GetComponent<MeshRenderer>();
                sphereRenderer.material = new Material(Shader.Find("IATK/TransparentSphere"));

                setSphereColor();

                sphere.transform.localScale = new Vector3(radius, radius, radius);

                sphereRenderer.sharedMaterial.SetFloat("_Transparency", transparency);
            
                positions = GetComponent<HeightmapVisualisation>().positions;

                getDataArray();
            }          
        }

        void Update() {
            
            sphere.transform.position = new Vector3(positions[x * 30].x, positions[x * 30 + z].y, positions[z].z); 
            sphere.transform.localScale = new Vector3(radius, radius, radius);

            setSphereColor();

            sphereRenderer.sharedMaterial.SetFloat("_Transparency", transparency);
            sphereValue = (int) dataArray[x * 30 + z, 2];
        } 
        
        void getDataArray() {

            CSVDataSource dataSource = (CSVDataSource) GetComponent<Visualisation>().dataSource;
            String data = dataSource.data.ToString();

            string[] lines = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            dataArray = new float[lines.Length - 1, dataSource.DimensionCount];

            if (lines.Length > 1) {
                for (int i = 1; i < lines.Length; i++) {
                    string[] values = lines[i].Split(new char[] { ',', '\t', ';'});

                    for (int k = 0; k < values.Count(); k++) {
                        string cleanedValue = values[k].Replace("\r", string.Empty);

                        if (k <= dataSource.DimensionCount - 1) {
                            double result = 0.0f;
                            double.TryParse(cleanedValue, out result);
                            dataArray[i - 1, k] = (float)result;
                        }
                    }
                }
            }
            
            sphereValue = (int) dataArray[x * 30 + z, 2];
        }

        void setSphereColor() {
            int sphereVertices = sphere.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
            Color[] newSphereColor = new Color[sphereVertices];
            for (int i = 0; i < sphereVertices; ++i) {
                newSphereColor[i] = sphereColor;
            }
            sphere.GetComponent<MeshFilter>().sharedMesh.colors = newSphereColor;
        }

    }
}
