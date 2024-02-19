Shader "Custom/HLSL_ColorHeight"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}

	//_Color ("Color", Color) = (1,1,1,1)
	//_Glossiness ("Smoothness", Range(0,1)) = 0.5
	//_Metallic ("Metallic", Range(0,1)) = 0.0
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		LOD 100

		Pass
		{
			HLSLPROGRAM

			// Comunicarle a HLSL que "vert" y "frag" son los nombres de los metodos que definen
			// el "vertex shader" y el "fragment shader"
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// VERTEX INPUT
			// Parametros de entrada para el VertexShader
			struct appdata
			{
				// Posicion del vertice
				float4 vertex : POSITION;
				// Coordenada de la textura
				float2 uv : TEXCOORD0;
			};

			// VERTEX OUTPUT
			struct v2f {
				//UNITY_FOG_COORDS(1)

				// SV_POSITION significa pixel position
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;

				// Coordenada en el mundo
				float3 worldPos : TEXCOORD1; // Agregar el miembro para almacenar la posición del mundo


				// Codigo mio
				//float3 worldPos;
			};




			sampler2D _MainTex;
			float4 _MainTex_ST;

			float minHeight = 0;
			float maxHeight = 28;


			const static int maxColourCount = 8;

			// Cuantos colores quiere el usuario utilizar
			int baseColourCount;
			// Colores deseados
			fixed4 baseColours[maxColourCount];
			// Alturas que separan los diferentes colores
			float baseStartHeights[maxColourCount];

			float baseBlends[maxColourCount];


			float middlePosition = .4f;
			float blendEffect = .2f;


			// -------------------------------------------------------------------------------- //
			//								 VERTEX SHADER										//
			// -------------------------------------------------------------------------------- //

			// Aqui se convierte un vertice de posicion en 3D a posicion en pantalla
			// v : input
			v2f vert(appdata v)
			{
				// Output
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;


				//o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calcular la posición del mundo y asignarla
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calcular la posición del mundo y asignarla


				//UNITY_TRANSFER_FOG(o, o.vertex);

				return o;
			}

			// Metodo mio
			float inverseLerp(float a, float b, float value) {
				return saturate((value - a) / (b - a));
			}


			// -------------------------------------------------------------------------------- //
			//								FRAGMENT SHADER										//
			// -------------------------------------------------------------------------------- //

			// Se llama por cada pixel que se quiere dibujar en pantalla
			// Convierte todos los triangulos viisbles en pantalla y los transforma en fragmentos
			// Cada fragmento representa un pixel en pantalla
			// El color que devuelve la funcion es el color del pixel que se esta procesando en cada momento
			fixed4 frag(v2f i) : SV_Target
			{
				// PRUEBAS MIAS

				//fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 color = fixed4(1, 1, 0, 1);

				//float3 worldPos = i.worldPos;

				//float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

				//if (heightPercent < .15f)
				//	color = fixed4(1, 0, 0, 1);
				//else if (heightPercent < .5f)
				//	color = heightPercent;
				//else if (worldPos.x < 0)
				//	color = fixed4(0, 0, 1, 1);





				// CAMBIO ENTRE COLORES PLANOS FUNCIONANDO

				float3 worldPos = i.worldPos;
				float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

				//fixed4 color = fixed4(1, 1, 1, 1);

				//for (int i = 0; i < baseColourCount; i++)
				//{
				//	float strenght = 5;


				//	float thisHeight = baseStartHeights[i];
				//	if (heightPercent < thisHeight) {
				//		color = baseColours[i] * baseColours[i];
				//		break;
				//	}
				//}

				//return color;




				// GRADIENTE ENTRE 3 COLORES

				fixed4 col = fixed4(0, 0, 1, 1);;

				fixed4 bottomColor = baseColours[0];
				fixed4 middleColor = baseColours[1];
				fixed4 topColor = baseColours[2];

				//fixed4 bottomColor = fixed4(1, 0, 0, 1);
				//fixed4 middleColor = fixed4(0, 1, 0, 1);
				//fixed4 topColor = fixed4(0, 0, 1, 1);

				//float middlePosition = .4f;
				//float blendEffect = .2f;

				if (heightPercent < middlePosition) {

					if (heightPercent < middlePosition - blendEffect)
						col = bottomColor;

					// Si se esta en la franja del blend 
					else 
						col = lerp(bottomColor, middleColor, ( heightPercent - (middlePosition - blendEffect) ) / blendEffect);
				}
				else {

					if (heightPercent < middlePosition + blendEffect)
						col = lerp(middleColor, topColor, (heightPercent - middlePosition) / blendEffect);

					// Si se esta en la franja del blend
					else
						col = topColor;
				}

				return col;
			}
			ENDHLSL
		}
	}
}