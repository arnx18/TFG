using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System;

namespace IATK
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HeatmapVisualisation : AbstractVisualisation {

        public Gradient gradient = null;

        [HideInInspector]
        public GameObject gradientLegend;
        
        [HideInInspector]
        public GameObject legendHolder;

        [HideInInspector]
        public Gradient cacheGradient;

        [HideInInspector]
        public Vector3[] positions;

        [HideInInspector]
        public MeshFilter meshFilter;

        private int xSize;
        private int zSize;
        private float minTerrainHeight;
        private float maxTerrainHeight;

        public override void CreateVisualisation()
        {
            string savedName = name;

            foreach (View v in viewList)
            {
                DestroyImmediate(v.gameObject);
            }

            viewList.Clear();

            // Create new
            Dictionary<CreationConfiguration.Axis, string> axies = new Dictionary<CreationConfiguration.Axis, string>();
            if (visualisationReference.xDimension.Attribute != "" && visualisationReference.xDimension.Attribute != "Undefined")
            {
                axies.Add(CreationConfiguration.Axis.X, visualisationReference.xDimension.Attribute);
            }
            if (visualisationReference.yDimension.Attribute != "" && visualisationReference.yDimension.Attribute != "Undefined")
            {
                axies.Add(CreationConfiguration.Axis.Y, visualisationReference.yDimension.Attribute);
            }
            if (visualisationReference.zDimension.Attribute != "" && visualisationReference.zDimension.Attribute != "Undefined")
            {
                axies.Add(CreationConfiguration.Axis.Z, visualisationReference.zDimension.Attribute);
            }

            // Create the configuration object
            if (creationConfiguration == null)
                creationConfiguration = new CreationConfiguration(visualisationReference.geometry, axies);
            else
            {
                creationConfiguration.Axies = axies;
                creationConfiguration.Geometry = visualisationReference.geometry;
                creationConfiguration.LinkingDimension = visualisationReference.linkingDimension;
                creationConfiguration.Size = visualisationReference.size;
                creationConfiguration.MinSize = visualisationReference.minSize;
                creationConfiguration.MaxSize = visualisationReference.maxSize;
                creationConfiguration.colour = visualisationReference.colour;
            }

            CreateSimpleVisualisation(creationConfiguration);

            if (viewList.Count > 0 && visualisationReference.colourDimension != "Undefined")
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    viewList[i].SetColors(mapColoursContinuous(visualisationReference.dataSource[visualisationReference.colourDimension].Data));
                }
            }
            else if (viewList.Count > 0 && visualisationReference.colorPaletteDimension != "Undefined")
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    viewList[i].SetColors(mapColoursPalette(visualisationReference.dataSource[visualisationReference.colorPaletteDimension].Data, visualisationReference.coloursPalette));
                }
            }
            else if (viewList.Count > 0 && visualisationReference.colour != null)
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    Color[] colours = viewList[i].GetColors();
                    for (int j = 0; j < colours.Length; ++j)
                    {
                        colours[j] = visualisationReference.colour;
                    }
                    viewList[i].SetColors(colours);
                }
            }


            if (viewList.Count > 0)
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    viewList[i].SetSize(visualisationReference.size);
                    viewList[i].SetMinSize(visualisationReference.minSize);
                    viewList[i].SetMaxSize(visualisationReference.maxSize);

                    viewList[i].SetMinNormX(visualisationReference.xDimension.minScale);
                    viewList[i].SetMaxNormX(visualisationReference.xDimension.maxScale);
                    viewList[i].SetMinNormY(visualisationReference.yDimension.minScale);
                    viewList[i].SetMaxNormY(visualisationReference.yDimension.maxScale);
                    viewList[i].SetMinNormZ(visualisationReference.zDimension.minScale);
                    viewList[i].SetMaxNormZ(visualisationReference.zDimension.maxScale);

                    viewList[i].SetMinX(visualisationReference.xDimension.minFilter);
                    viewList[i].SetMaxX(visualisationReference.xDimension.maxFilter);
                    viewList[i].SetMinY(visualisationReference.yDimension.minFilter);
                    viewList[i].SetMaxY(visualisationReference.yDimension.maxFilter);
                    viewList[i].SetMinZ(visualisationReference.zDimension.minFilter);
                    viewList[i].SetMaxZ(visualisationReference.zDimension.maxFilter);
                }
            }

            if (viewList.Count > 0 && visualisationReference.sizeDimension != "Undefined")
            {
                for (int i = 0; i < viewList.Count; i++)
                {
                    viewList[i].SetSizeChannel(visualisationReference.dataSource[visualisationReference.sizeDimension].Data);
                }
            }

            name = savedName;
        }

        public override void UpdateVisualisation(PropertyType propertyType){

            if (viewList.Count == 0)
                CreateVisualisation();

            if (viewList.Count != 0)
                switch (propertyType)
                {
                    case AbstractVisualisation.PropertyType.X:
                        if (visualisationReference.xDimension.Attribute.Equals("Undefined"))
                        {
                            viewList[0].ZeroPosition(0);
                            viewList[0].TweenPosition();
                        }
                        else
                        {
                            viewList[0].UpdateXPositions(visualisationReference.dataSource[visualisationReference.xDimension.Attribute].Data);
                            viewList[0].TweenPosition();
                        }
                        if (creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.X))
                        {
                            creationConfiguration.Axies[CreationConfiguration.Axis.X] = visualisationReference.xDimension.Attribute;
                        }
                        else
                        {
                            creationConfiguration.Axies.Add(CreationConfiguration.Axis.X, visualisationReference.xDimension.Attribute);
                        }
                        break;
                    case AbstractVisualisation.PropertyType.Y:
                        if (visualisationReference.yDimension.Attribute.Equals("Undefined"))
                        {
                            viewList[0].ZeroPosition(1);
                            viewList[0].TweenPosition();
                        }
                        else
                        {
                            viewList[0].UpdateYPositions(visualisationReference.dataSource[visualisationReference.yDimension.Attribute].Data);
                            viewList[0].TweenPosition();
                        }
                        if (creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.Y))
                        {
                            creationConfiguration.Axies[CreationConfiguration.Axis.Y] = visualisationReference.yDimension.Attribute;
                        }
                        else
                        {
                            creationConfiguration.Axies.Add(CreationConfiguration.Axis.Y, visualisationReference.yDimension.Attribute);
                        }
                        break;
                    case AbstractVisualisation.PropertyType.Z:
                        if (visualisationReference.zDimension.Attribute.Equals("Undefined"))
                        {
                            viewList[0].ZeroPosition(2);
                            viewList[0].TweenPosition();
                        }
                        else
                        {
                            viewList[0].UpdateZPositions(visualisationReference.dataSource[visualisationReference.zDimension.Attribute].Data);
                            viewList[0].TweenPosition();
                        }
                        if (creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.Z))
                        {
                            creationConfiguration.Axies[CreationConfiguration.Axis.Z] = visualisationReference.zDimension.Attribute;
                        }
                        else
                        {
                            creationConfiguration.Axies.Add(CreationConfiguration.Axis.Z, visualisationReference.zDimension.Attribute);
                        }
                        break;
                    case AbstractVisualisation.PropertyType.Colour:
                        if (visualisationReference.colourDimension != "Undefined")
                        {
                            for (int i = 0; i < viewList.Count; i++)
                                viewList[i].SetColors(mapColoursContinuous(visualisationReference.dataSource[visualisationReference.colourDimension].Data));
                        }
                        else if (visualisationReference.colorPaletteDimension != "Undefined")
                        {
                            for (int i = 0; i < viewList.Count; i++)
                            {
                                viewList[i].SetColors(mapColoursPalette(visualisationReference.dataSource[visualisationReference.colorPaletteDimension].Data, visualisationReference.coloursPalette));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < viewList.Count; i++)
                            {
                                Color[] colours = viewList[0].GetColors();
                                for (int j = 0; j < colours.Length; ++j)
                                {
                                    colours[j] = visualisationReference.colour;
                                }
                                viewList[i].SetColors(colours);
                            }

                        }

                        creationConfiguration.ColourDimension = visualisationReference.colourDimension;
                        creationConfiguration.colourKeys = visualisationReference.dimensionColour;
                        creationConfiguration.colour = visualisationReference.colour;

                        break;
                    case AbstractVisualisation.PropertyType.Size:
                        {
                            for (int i = 0; i < viewList.Count; i++)
                            {
                                if (visualisationReference.sizeDimension != "Undefined")
                                {
                                    viewList[i].SetSizeChannel(visualisationReference.dataSource[visualisationReference.sizeDimension].Data);
                                }
                                else
                                {
                                    viewList[i].SetSizeChannel(new float[visualisationReference.dataSource.DataCount]);
                                }
                            }
                            creationConfiguration.SizeDimension = visualisationReference.sizeDimension;       
                            viewList[0].TweenSize();

                            break;

                        }
                    case PropertyType.SizeValues:
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            viewList[i].SetSize(visualisationReference.size);
                            viewList[i].SetMinSize(visualisationReference.minSize);        // Data is normalised
                            viewList[i].SetMaxSize(visualisationReference.maxSize);
                        }
                        creationConfiguration.Size = visualisationReference.size;
                        creationConfiguration.MinSize = visualisationReference.minSize;
                        creationConfiguration.MaxSize = visualisationReference.maxSize;

                        break;
                    case AbstractVisualisation.PropertyType.LinkingDimension:
                        creationConfiguration.LinkingDimension = visualisationReference.linkingDimension;
                        
                        CreateVisualisation(); // needs to recreate the visualsiation because the mesh properties have changed
                        rescaleViews();
                        break;

                    case AbstractVisualisation.PropertyType.GeometryType:
                        creationConfiguration.Geometry = visualisationReference.geometry;
                        CreateVisualisation(); // needs to recreate the visualsiation because the mesh properties have changed 
                        rescaleViews();
                        break;

                    case AbstractVisualisation.PropertyType.Scaling:
                        
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            viewList[i].SetMinNormX(visualisationReference.xDimension.minScale);
                            viewList[i].SetMaxNormX(visualisationReference.xDimension.maxScale);
                            viewList[i].SetMinNormY(visualisationReference.yDimension.minScale);
                            viewList[i].SetMaxNormY(visualisationReference.yDimension.maxScale);
                            viewList[i].SetMinNormZ(visualisationReference.zDimension.minScale);
                            viewList[i].SetMaxNormZ(visualisationReference.zDimension.maxScale);
                        }
                        
                        // TODO: Move visualsiation size from Scaling to its own PropertyType
                        creationConfiguration.VisualisationWidth = visualisationReference.width;
                        creationConfiguration.VisualisationHeight = visualisationReference.height;
                        creationConfiguration.VisualisationDepth = visualisationReference.depth;
                        break;

                    case AbstractVisualisation.PropertyType.DimensionFiltering:
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            viewList[i].SetMinX(visualisationReference.xDimension.minFilter);
                            viewList[i].SetMaxX(visualisationReference.xDimension.maxFilter);
                            viewList[i].SetMinY(visualisationReference.yDimension.minFilter);
                            viewList[i].SetMaxY(visualisationReference.yDimension.maxFilter);
                            viewList[i].SetMinZ(visualisationReference.zDimension.minFilter);
                            viewList[i].SetMaxZ(visualisationReference.zDimension.maxFilter);
                        }
                        break;
                    case AbstractVisualisation.PropertyType.AttributeFiltering:
                        if (visualisationReference.attributeFilters!=null)
                        {
                            foreach (var viewElement in viewList)
                            {
                                float[] isFiltered = new float[visualisationReference.dataSource.DataCount];
                                for (int i = 0; i < visualisationReference.dataSource.DimensionCount; i++)
                                {
                                    foreach (AttributeFilter attrFilter in visualisationReference.attributeFilters)
                                    {
                                        if (attrFilter.Attribute == visualisationReference.dataSource[i].Identifier)
                                        {
                                            float minFilteringValue = UtilMath.normaliseValue(attrFilter.minFilter, 0f, 1f, attrFilter.minScale, attrFilter.maxScale);
                                            float maxFilteringValue = UtilMath.normaliseValue(attrFilter.maxFilter, 0f, 1f, attrFilter.minScale, attrFilter.maxScale);

                                            for (int j = 0; j < isFiltered.Length; j++)
                                            {
                                                isFiltered[j] = (visualisationReference.dataSource[i].Data[j] < minFilteringValue || visualisationReference.dataSource[i].Data[j] > maxFilteringValue) ? 1.0f : isFiltered[j];
                                            }
                                        }
                                    }
                                }
                                // map the filtered attribute into the normal channel of the bigmesh
                                foreach (View v in viewList)
                                {
                                    v.SetFilterChannel(isFiltered);
                                }
                            }
                        }
                        break;
                    case PropertyType.VisualisationType:                       
                        break;
                    case PropertyType.BlendDestinationMode:
                        float bmds = (int)(System.Enum.Parse(typeof(UnityEngine.Rendering.BlendMode), visualisationReference.blendingModeDestination));
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            viewList[i].SetBlendindDestinationMode(bmds);
                        }

                            break;
                    case PropertyType.BlendSourceMode:
                        float bmd = (int)(System.Enum.Parse(typeof(UnityEngine.Rendering.BlendMode), visualisationReference.blendingModeSource));
                        for (int i = 0; i < viewList.Count; i++)
                        {
                            viewList[i].SetBlendingSourceMode(bmd);
                        }

                        break;
                    default:
                        break;
                }
            
            if (visualisationReference.geometry != GeometryType.Undefined)// || visualisationType == VisualisationTypes.PARALLEL_COORDINATES)
            SerializeViewConfiguration(creationConfiguration);

            //Update any label on the corresponding axes
            UpdateVisualisationAxes(propertyType);
        }

        public void UpdateVisualisationAxes(AbstractVisualisation.PropertyType propertyType)
        {
            switch (propertyType)
            {
                case AbstractVisualisation.PropertyType.X:
                    if (visualisationReference.xDimension.Attribute == "Undefined" && X_AXIS != null)// GameObject_Axes_Holders[0] != null)
                    {
                        DestroyImmediate(X_AXIS);
                    }
                    else if (X_AXIS != null)
                    {
                        Axis a = X_AXIS.GetComponent<Axis>();
                        a.Initialise(visualisationReference.dataSource, visualisationReference.xDimension, visualisationReference);
                        BindMinMaxAxisValues(a, visualisationReference.xDimension);
                    }
                    else if (visualisationReference.xDimension.Attribute != "Undefined")
                    {
                        Vector3 pos = Vector3.zero;
                        pos.y = -0.025f;
                        X_AXIS = CreateAxis(propertyType, visualisationReference.xDimension, pos, new Vector3(0f, 0f, 0f), 0);   
                        
                    }
                    break;
                case AbstractVisualisation.PropertyType.Y:
                    if (visualisationReference.yDimension.Attribute == "Undefined" && Y_AXIS != null)
                    {
                        DestroyImmediate(Y_AXIS);
                    }
                    else if (Y_AXIS != null)
                    {
                        Axis a = Y_AXIS.GetComponent<Axis>();
                        a.Initialise(visualisationReference.dataSource, visualisationReference.yDimension, visualisationReference);
                        BindMinMaxAxisValues(a, visualisationReference.yDimension);
                    }
                    else if (visualisationReference.yDimension.Attribute != "Undefined")
                    {
                        Vector3 pos = Vector3.zero;
                        pos.x = -0.025f;
                        Y_AXIS = CreateAxis(propertyType, visualisationReference.yDimension, pos, new Vector3(0f, 0f, 0f), 1);
                    }
                    break;
                case AbstractVisualisation.PropertyType.Z:
                    if (visualisationReference.zDimension.Attribute == "Undefined" && Z_AXIS != null)
                    {
                        DestroyImmediate(Z_AXIS);
                    }
                    else if (Z_AXIS != null)
                    {
                        Axis a = Z_AXIS.GetComponent<Axis>();
                        a.Initialise(visualisationReference.dataSource, visualisationReference.zDimension, visualisationReference);
                        BindMinMaxAxisValues(Z_AXIS.GetComponent<Axis>(), visualisationReference.zDimension);
                    }
                    else if (visualisationReference.zDimension.Attribute != "Undefined")
                    {
                        Vector3 pos = Vector3.zero;
                        pos.x = 1.095f;
                        Z_AXIS = CreateAxis(PropertyType.X, visualisationReference.zDimension, pos, new Vector3(0f, 0f, 0f), 2);
                        Z_AXIS.transform.eulerAngles= new Vector3(0f, 0f, 0f);
                        legendHolder.transform.SetParent(Z_AXIS.transform);
                        gradientLegend.transform.SetParent(legendHolder.transform);
                        
                    }
                    break;
                case AbstractVisualisation.PropertyType.DimensionFiltering:
                    if (visualisationReference.xDimension.Attribute != "Undefined")
                    {
                        BindMinMaxAxisValues(X_AXIS.GetComponent<Axis>(), visualisationReference.xDimension);
                    }
                    if (visualisationReference.yDimension.Attribute != "Undefined")
                    {
                        BindMinMaxAxisValues(Y_AXIS.GetComponent<Axis>(), visualisationReference.yDimension);
                    }
                    if (visualisationReference.zDimension.Attribute != "Undefined")
                    {
                        BindMinMaxAxisValues(Z_AXIS.GetComponent<Axis>(), visualisationReference.zDimension);
                    }
                    break;
                case AbstractVisualisation.PropertyType.Scaling:
                    if (visualisationReference.xDimension.Attribute != "Undefined")
                    {
                        Axis axis = X_AXIS.GetComponent<Axis>();
                        BindMinMaxAxisValues(axis, visualisationReference.xDimension);
                        axis.UpdateLength(visualisationReference.width);
                    }
                    if (visualisationReference.yDimension.Attribute != "Undefined")
                    {
                        Axis axis = Y_AXIS.GetComponent<Axis>();
                        BindMinMaxAxisValues(axis, visualisationReference.yDimension);
                        axis.UpdateLength(visualisationReference.height);
                    }
                    if (visualisationReference.zDimension.Attribute != "Undefined")
                    {
                        Axis axis = Z_AXIS.GetComponent<Axis>();
                        BindMinMaxAxisValues(axis, visualisationReference.zDimension);
                        axis.UpdateLength(visualisationReference.depth);
                    }
                    
                    rescaleViews();
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Gets the axies.
        /// </summary>
        /// <returns>The axies.</returns>
        /// <param name="axies">Axies.</param>
        protected string getAxis(Dictionary<CreationConfiguration.Axis, string> axies, CreationConfiguration.Axis axis)
        {

            string axes = null;
            string retVal = "";
            if (axies.TryGetValue(axis, out axes))
                retVal = axes;

            return retVal;
        }
        
        /// <summary>
        /// Rescales the views in this scatterplot to the width, height, and depth values in the visualisationReference
        /// </summary>
        protected void rescaleViews()
        {
            foreach (View view in viewList)
            {
                view.transform.localScale = new Vector3(
                    visualisationReference.width,
                    visualisationReference.height,
                    visualisationReference.depth
                );
            }
        }

        public override void SaveConfiguration(){}

        public override void LoadConfiguration(){}
        
        /// <summary>
        /// Maps the colours.
        /// </summary>
        /// <returns>The colours.</returns>
        /// <param name="data">Data.</param>
        public override Color[] mapColoursContinuous(float[] data)
        {
            Color[] colours = new Color[data.Length];

            for (int i = 0; i < data.Length; ++i)
            {
                colours[i] = visualisationReference.dimensionColour.Evaluate(data[i]);
            }

            return colours;
        }

        public Color[] mapColoursPalette(float[] data, Color[] palette)
        {
            Color[] colours = new Color[data.Length];

            float[] uniqueValues = data.Distinct().ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                int indexColor = Array.IndexOf(uniqueValues, data[i]);
                colours[i] = palette[indexColor];
            }

            return colours;
        }

        // ******************************
        // SPECIFIC VISUALISATION METHODS
        // ******************************
        
        private void CreateSimpleVisualisation(CreationConfiguration configuration)
        {

            if (visualisationReference.dataSource != null)
            {
                if (!visualisationReference.dataSource.IsLoaded) visualisationReference.dataSource.load();

                ViewBuilder builder = new ViewBuilder(geometryToMeshTopology(configuration.Geometry), "Simple Visualisation");

                if ((visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)] != null) ||
                    (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)] != null) ||
                    (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)] != null))
                {
                    builder.initialiseDataView(visualisationReference.dataSource.DataCount);

                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)].Data, ViewBuilder.VIEW_DIMENSION.X);
                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)].Data, ViewBuilder.VIEW_DIMENSION.Z);
                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)].Data, ViewBuilder.VIEW_DIMENSION.Y);
                    
                    //destroy the view to clean the big mesh
                    destroyView();
                
                }

                if (visualisationReference.xDimension.Attribute != "" && visualisationReference.xDimension.Attribute != "Undefined"
                    && visualisationReference.yDimension.Attribute != "" && visualisationReference.yDimension.Attribute != "Undefined"
                    && visualisationReference.zDimension.Attribute != "" && visualisationReference.zDimension.Attribute != "Undefined") {

                    int indX = Array.IndexOf(visualisationReference.dataSource.Select(m => m.Identifier).ToArray(), visualisationReference.xDimension.Attribute);
                    int indZ = Array.IndexOf(visualisationReference.dataSource.Select(m => m.Identifier).ToArray(), visualisationReference.yDimension.Attribute);

                    xSize = visualisationReference.dataSource[indX].MetaData.categoryCount;
                    zSize = visualisationReference.dataSource[indZ].MetaData.categoryCount;

                    positions = builder.Positions.ToArray();

                    CreateMesh();
                    CreateLegendMesh();
                    CreateMeshRenderer();
                }
            }
        }

        void CreateMesh() {
            
            if (gradient == null) CreateDefaultGradient();

            MESH = new Mesh();

            MESH.vertices = getHeightmapVertices();
            MESH.triangles = getHeightmapIndices();
            MESH.colors = getHeightmapColors();

            MESH.RecalculateNormals();

            cacheGradient = new Gradient();
            GradientColorKey[] gradientColorKeys = gradient.colorKeys;
            GradientAlphaKey[] gradientAlphaKeys = gradient.alphaKeys;
            cacheGradient.SetKeys(gradientColorKeys, gradientAlphaKeys);

            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = MESH;
 
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

        Vector3[] getHeightmapVertices() {

             Vector3[] vertices = (Vector3[]) positions.Clone();

            for (int i = 0; i < positions.Length; ++i) {
                float vertexHeight = positions[i].y;
                if (vertexHeight > maxTerrainHeight) maxTerrainHeight = vertexHeight;
                if (vertexHeight < minTerrainHeight) minTerrainHeight = vertexHeight;
                vertices[i].y = 0;
            }

            Vector3 center = new Vector3(0, 0, 0); 
            Quaternion newRotation = new Quaternion();
            newRotation.eulerAngles = new Vector3(-90,0,0); 
                
            for(int i = 0; i < vertices.Length; i++) { 
                vertices[i] = newRotation * (vertices[i] - center) + center;
            }

            return vertices;
        }

        int[] getHeightmapIndices() {

            int[] indices = new int[(xSize - 1) * (zSize - 1) * 6];

            int vert = 0;
            int tris = 0;

            if (xSize >= zSize) {
                for (int x = 0; x < xSize - 1; x++) {
                    for (int z = 0; z < zSize - 1; z++) {
                        indices[tris] = vert + 1;
                        indices[tris + 1] = vert + zSize;
                        indices[tris + 2] = vert;
                        indices[tris + 3] = vert + 1;
                        indices[tris + 4] = vert + zSize + 1;
                        indices[tris + 5] = vert + zSize;
                        vert++;
                        tris +=6;
                    }
                    vert++;
                }
            } else {
                for (int z = 0; z < zSize - 1; z++) {
                    for (int x = 0; x < xSize - 1; x++) {
                        indices[tris] = vert;
                        indices[tris + 1] = vert + xSize;
                        indices[tris + 2] = vert + 1;
                        indices[tris + 3] = vert + 1;
                        indices[tris + 4] = vert + xSize;
                        indices[tris + 5] = vert + xSize + 1;
                        vert++;
                        tris +=6;
                    }
                    vert++;
                }
            }

            return indices;
        }

        Color[] getHeightmapColors() {

            Color[] colors = new Color[positions.Length];

            for (int i = 0, z = 0; z < zSize; z++) {
                for (int x = 0; x < xSize; x++) {
                    float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, positions[i].y);
                    colors[i] = gradient.Evaluate(height);
                    i++;
                }
            }

            return colors;
        }
        
        void CreateLegendMesh() {

            float width = 1.0f;
            float height = 1.0f;

            Mesh legendMesh = new Mesh();

            int vertexCount = gradient.colorKeys.Length * 2;

            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Color[] colors = new Color[vertexCount];

            for (int i = 0; i < gradient.colorKeys.Length; ++i) {
                vertices[i * 2] = new Vector3(-width / 2f, gradient.colorKeys[i].time - (height / 2f), 0);
                vertices[i * 2 + 1] = new Vector3(width / 2f, gradient.colorKeys[i].time - (height / 2f), 0); 

                normals[i * 2] = -Vector3.forward;
                normals[i * 2 + 1] = Vector3.forward; 

                colors[i * 2] = gradient.colorKeys[i].color;
                colors[i * 2 + 1] = gradient.colorKeys[i].color;
            }

            legendMesh.vertices = vertices;

            int[] indices = new int[gradient.colorKeys.Length * 6];

            for (int i = 0; i < gradient.colorKeys.Length - 1; ++i) {
                indices[i * 6] = i * 2;
                indices[i * 6 + 1] = i * 2 + 2;
                indices[i * 6 + 2] = i * 2 + 1;
                indices[i * 6 + 3] = i * 2 + 2;
                indices[i * 6 + 4] = i * 2 + 3;
                indices[i * 6 + 5] = i * 2 + 1;
            }

            legendMesh.triangles = indices;
            legendMesh.normals = normals;
            legendMesh.colors = colors;

            if (legendHolder == null) {
                legendHolder = new GameObject("legendHolder");
                legendHolder.transform.localPosition = new Vector3(-0.045f, 0f, 0f);
            }

            if (gradientLegend == null) {
                gradientLegend = new GameObject("GradientLegend");
                gradientLegend.AddComponent<MeshFilter>();
                gradientLegend.AddComponent<MeshRenderer>();          
                gradientLegend.transform.localPosition = new Vector3(1.05f, 0.5f, 0f);
                gradientLegend.transform.localScale = new Vector3(0.03f, 1, 1);
            }

            legendHolder.transform.localScale = new Vector3(1, visualisationReference.depth, 1);
            
            
            gradientLegend.GetComponent<MeshFilter>().sharedMesh = legendMesh;
            
            Material material = new Material(Shader.Find("IATK/Heatmap"));
            gradientLegend.GetComponent<MeshRenderer>().sharedMaterial = material;

        }

        void CreateMeshRenderer() {

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material material = new Material(Shader.Find("IATK/Heatmap"));
            meshRenderer.sharedMaterial = material;
        }

        bool IsColorKeyApproxEqual(GradientColorKey colorKey1, GradientColorKey colorkey2) {

            if(Mathf.Abs(colorKey1.time - colorkey2.time) > 0.01f) return false;
            if(Mathf.Abs(colorKey1.color.r - colorkey2.color.r) > 0.01f) return false;
            if(Mathf.Abs(colorKey1.color.g - colorkey2.color.g) > 0.01f) return false;
            if(Mathf.Abs(colorKey1.color.b - colorkey2.color.b) > 0.01f) return false;
            return true;
        }

        // *************************************************************
        // ********************  UNITY METHODS  ************************
        // *************************************************************
        void Update() {
            if (visualisationReference.xDimension.Attribute != "" && visualisationReference.xDimension.Attribute != "Undefined"
                && visualisationReference.yDimension.Attribute != "" && visualisationReference.yDimension.Attribute != "Undefined"
                && visualisationReference.zDimension.Attribute != "" && visualisationReference.zDimension.Attribute != "Undefined") {

                if(gradient.colorKeys.Length != cacheGradient.colorKeys.Length) {
                    CreateMesh();
                    CreateLegendMesh();
                    return;
                }

                for(int i = 0; i < gradient.colorKeys.Length; i++) {
                    if (!IsColorKeyApproxEqual(gradient.colorKeys[i], cacheGradient.colorKeys[i])) {
                        CreateMesh();
                        CreateLegendMesh();
                        return;
                    }
                }
            }
        }
    }   
}