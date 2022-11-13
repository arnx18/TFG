using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

 [ExecuteInEditMode]
 [RequireComponent(typeof(MeshFilter))]
 [RequireComponent(typeof(MeshRenderer))]
public class HeightmapVisualisation : MonoBehaviour {

    public Gradient gradient = null;

    [HideInInspector]
    public Gradient cacheGradient;
    
    [HideInInspector]
    public Vector3[] vertices;

    [HideInInspector]
    public MeshFilter meshFilter;

    private int xSize = 20;
    private int zSize = 20;

    private float minTerrainHeight;
    private float maxTerrainHeight;

    void Reset() {

        meshFilter = GetComponent<MeshFilter>();

        meshFilter.sharedMesh = CreateMesh();

        CreateMeshRenderer();
    }

    void Update() {
        
        if(gradient.colorKeys.Length != cacheGradient.colorKeys.Length) {
            meshFilter.sharedMesh = CreateMesh();
            return;
        }

        for(int i = 0; i < gradient.colorKeys.Length; i++) {
            if (!IsColorKeyApproxEqual(gradient.colorKeys[i], cacheGradient.colorKeys[i])) {
                meshFilter.sharedMesh = CreateMesh();
                return;
            }
        }

    }

    Mesh CreateMesh() {

        if(gradient == null) CreateDefaultGradient();
        
        Mesh mesh = new Mesh();

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);

                if(y > maxTerrainHeight) maxTerrainHeight = y;
                if(y < minTerrainHeight) minTerrainHeight = y;

                i++;
            }
        }
        mesh.vertices = vertices;

        int[] triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris +=6;
            }
            vert++;
        }
        mesh.triangles = triangles;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
        mesh.colors = colors;

        mesh.RecalculateNormals();

        cacheGradient = new Gradient();
        GradientColorKey[] gradientColorKeys = gradient.colorKeys;
        GradientAlphaKey[] gradientAlphaKeys = gradient.alphaKeys;
        cacheGradient.SetKeys(gradientColorKeys, gradientAlphaKeys);

        return mesh;
    }

    void CreateDefaultGradient() {

        GradientColorKey[] gradientColorKeys;
        GradientAlphaKey[] gradientAlphaKeys;

        gradient = new Gradient();
        gradientColorKeys = new GradientColorKey[3];

        gradientColorKeys[0].color = new Color(0f, 0.62f, 0.62f);
        gradientColorKeys[0].time = 0f;

        gradientColorKeys[1].color = new Color(0.72f, 0.78f, 0f);
        gradientColorKeys[1].time = 0.5f;

        gradientColorKeys[2].color = new Color(0.86f, 0f, 0f);
        gradientColorKeys[2].time = 1.0f;

        gradientAlphaKeys = new GradientAlphaKey[2];
        gradientAlphaKeys[0].alpha = 1.0f;
        gradientAlphaKeys[0].time = 0f;
        gradientAlphaKeys[1].alpha = 1.0f;
        gradientAlphaKeys[1].time = 1f;

        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
    }

    void CreateMeshRenderer() {

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("IATK/Heightmap"));
        meshRenderer.allowOcclusionWhenDynamic = false;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.sharedMaterial = material;
    }

    bool IsColorKeyApproxEqual(GradientColorKey colorKey1, GradientColorKey colorkey2) {

        if(Mathf.Abs(colorKey1.time - colorkey2.time) > 0.01f) return false;
        if(Mathf.Abs(colorKey1.color.r - colorkey2.color.r) > 0.01f) return false;
        if(Mathf.Abs(colorKey1.color.g - colorkey2.color.g) > 0.01f) return false;
        if(Mathf.Abs(colorKey1.color.b - colorkey2.color.b) > 0.01f) return false;
        return true;
    }

    private void OnDrawGizmos() {

        if (vertices == null) return;

        for (int i = 0; i < vertices.Length; i++) {
           Gizmos.DrawSphere(vertices[i], .1f); 
        }
    }
}
