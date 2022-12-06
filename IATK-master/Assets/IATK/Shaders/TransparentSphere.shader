Shader "IATK/TransparentSphere"
{
    Properties
    {
        _Transparency("_Transparency", Range(0.25, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull [_CullMode]
            Lighting Off
            Fog { Mode Off }
            ZTest [unity_GUIZTestMode]
            Blend One OneMinusSrcAlpha

            CGPROGRAM
				#pragma vertex VS_Main
				#pragma fragment FS_Main
                #include "UnityCG.cginc"

                struct VS_INPUT
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 position : SV_POSITION;
                    float4 color : COLOR;
                };
                
                float _Transparency;

                // **************************************************************
				// Shader Programs												*
				// **************************************************************
				
				// Vertex Shader ------------------------------------------------
                v2f VS_Main(VS_INPUT v)
                {
                    v2f o;

					o.color = v.color;
					o.position = UnityObjectToClipPos(v.vertex);

					return o;
                }

                // Fragment Shader -----------------------------------------------
                float4 FS_Main (v2f input) : COLOR
                {       
                    input.color.a = _Transparency;
                    return input.color;
                }
            ENDCG
        }
    }
}
