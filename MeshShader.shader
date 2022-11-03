Shader "Unlit/MeshShader"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : Color;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : Color;
            };

            v2f vertexFunc (appdata IN)
            {
                v2f OUT;

                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.color = IN.color;

                return OUT;
            }

            fixed4 fragmentFunc (v2f IN) : SV_Target
            {
                return IN.color;
            }

            ENDCG
        }
    }
}
