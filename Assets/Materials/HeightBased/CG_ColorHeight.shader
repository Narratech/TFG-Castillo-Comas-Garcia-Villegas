Shader "Custom/CG_ColorHeight"
{
	Properties
	{
		//_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		LOD 100


		HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Vertex Input
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			// Vertex Output
			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)

				// Codigo mio
				//float3 worldPos;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float minHeight = 0;
			float maxHeight = 10;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o, o.vertex);

				// Codigo mio
				//o.worldPos = float3(1, 2, 3);

				return o;
			}

			//// Metodo mio
			//float inverseLerp(float a, float b, float value) {
			//	return saturate((value - a) / (b - a));
			//}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 redColor = fixed4(1, 0, 0, 1);


				// Codigo mio



/*				UNITY_APPLY_FOG(i.fogCoord, col)*/;
				return redColor;
			}
			ENDHLSL
		}
	}
}
