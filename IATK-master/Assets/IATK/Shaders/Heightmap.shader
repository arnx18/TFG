Shader "IATK/Heightmap"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
			Tags {
				"LightMode" = "ForwardBase"
			}

            CGPROGRAM
				#pragma vertex VS_Main
				#pragma fragment FS_Main
                #include "UnityCG.cginc"
                #include "UnityLightingCommon.cginc" // for _LightColor0

                struct VS_INPUT
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float3 normal: NORMAL;
                };

                struct v2f
                {
                    float4 position : SV_POSITION;
                    float4 color : COLOR;
                };
                
                // **************************************************************
				// Shader Programs												*
				// **************************************************************
				
				// Vertex Shader ------------------------------------------------
                v2f VS_Main(VS_INPUT v)
                {
                    v2f o;

					float3 normalDirection = normalize(mul(float4(v.normal, 0.0),unity_WorldToObject).xyz);
					float3 lightDirection;
					float atten = 1.0;

					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float3 diffuseReflection = atten *_LightColor0.xyz*v.color.rgb*max(0.0,dot(normalDirection, lightDirection));

					o.color = float4(diffuseReflection, 1.0);
					o.position = UnityObjectToClipPos(v.vertex);

					return o;
                }

                // Fragment Shader -----------------------------------------------
                float4 FS_Main (v2f input) : COLOR
                {
                    return input.color;
                }
            ENDCG
        }
    }
}
