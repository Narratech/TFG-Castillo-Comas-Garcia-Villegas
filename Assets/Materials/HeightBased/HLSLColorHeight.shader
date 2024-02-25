Shader "Custom/HLSL_ColorHeight"
{
	Properties
	{
		testTexture("Albedo (RGB)", 2D) = "white" {}
		testScale("Scale", Float) = 1


		testTexture2("Albedo (RGB)", 2D) = "white" {}

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

				float3 worldNormalOS : NORMAL;
			};

			// VERTEX OUTPUT
			struct v2f {
				//UNITY_FOG_COORDS(1)

				// SV_POSITION significa pixel position
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;

				// Coordenada en el mundo
				float3 worldPos : TEXCOORD1; // Agregar el miembro para almacenar la posición del mundo

				
				float3 worldNormalWS : TEXCOORD2;

				// Codigo mio
				//float3 worldPos;
			};





			sampler2D testTexture;
			sampler2D testTexture2;
			float testScale;



			sampler2D _MainTex;
			float4 _MainTex_ST;

			float minHeight = 0;
			float maxHeight = 28;


			const static int maxLayerCount = 8;

			// Cuantos colores quiere el usuario utilizar
			int layerCount;
			// Colores deseados
			fixed4 baseColours[maxLayerCount];
			// Alturas que separan los diferentes colores
			float baseStartHeights[maxLayerCount];

			float baseBlends[maxLayerCount];
			float baseColourStrenght[maxLayerCount];
			float baseTexturesScales[maxLayerCount];


			UNITY_DECLARE_TEX2DARRAY(baseTextures);


			const static float epsilon = 1E-4;


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


				o.worldNormalWS = mul(unity_ObjectToWorld, v.worldNormalOS).xyz;;

				//UNITY_TRANSFER_FOG(o, o.vertex);

				return o;
			}

			// Metodo mio
			float inverseLerp(float a, float b, float value) {
				return saturate((value - a) / (b - a));
			}

			//float triplanarXD(float3 worldPos) {
			//	float3 scaledWorldPos = worldPos / testScale;

			//	//float3 blendAxes = abs(i.worldNormalWS);
			//	//blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
			//	float3 xProjection = tex2D(testTexture, scaledWorldPos.yz) * blendAxes.x;
			//	float3 yProjection = tex2D(testTexture, scaledWorldPos.xz) * blendAxes.y;
			//	float3 zProjection = tex2D(testTexture, scaledWorldPos.xy) * blendAxes.z;

			//	float3 projection = xProjection + yProjection + zProjection;
			//}

			float3 calculateProjections(sampler2D thisTexture, float3 scaledWorldPos, float3 blendAxes) {

				float3 xProjection = tex2D(thisTexture, scaledWorldPos.yz) * blendAxes.x;
				float3 yProjection = tex2D(thisTexture, scaledWorldPos.xz) * blendAxes.y;
				float3 zProjection = tex2D(thisTexture, scaledWorldPos.xy) * blendAxes.z;

				return xProjection + yProjection + zProjection;
			}


			float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
				// Triplanar mapping
				float3 scaledWorldPos = worldPos / scale;

				//float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
				//float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
				//float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;


				//float3 xProjection = tex2D(testTexture, scaledWorldPos.yz) * blendAxes.x;
				//float3 yProjection = tex2D(testTexture, scaledWorldPos.xz) * blendAxes.y;
				//float3 zProjection = tex2D(testTexture, scaledWorldPos.xy) * blendAxes.z;
				//float3 projection = xProjection + yProjection + zProjection;


				if (textureIndex %2 == 0) {
					return calculateProjections(testTexture, scaledWorldPos, blendAxes);
				}
				else {
					return calculateProjections(testTexture2, scaledWorldPos, blendAxes);
				}

				//return projection;
				//finalColor = fixed4(projection.x, projection.y, projection.z, 1);
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

				//float3 worldPos = i.worldPos;
				//float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

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




				//// GRADIENTE ENTRE 3 COLORES

				//fixed4 col = fixed4(0, 0, 1, 1);;

				//fixed4 bottomColor = baseColours[0];
				//fixed4 middleColor = baseColours[1];
				//fixed4 topColor = baseColours[2];

				////fixed4 bottomColor = fixed4(1, 0, 0, 1);
				////fixed4 middleColor = fixed4(0, 1, 0, 1);
				////fixed4 topColor = fixed4(0, 0, 1, 1);

				////float middlePosition = .4f;
				////float blendEffect = .2f;

				//if (heightPercent < middlePosition) {

				//	if (heightPercent < middlePosition - blendEffect)
				//		col = bottomColor;

				//	// Si se esta en la franja del blend 
				//	else 
				//		col = lerp(bottomColor, middleColor, ( heightPercent - (middlePosition - blendEffect) ) / blendEffect);
				//}
				//else {

				//	if (heightPercent < middlePosition + blendEffect)
				//		col = lerp(middleColor, topColor, (heightPercent - middlePosition) / blendEffect);

				//	// Si se esta en la franja del blend
				//	else
				//		col = topColor;
				//}

				//return col;







				//// COLORES SIN GRADIENTE
				//float3 worldPos = i.worldPos;
				//float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

				//// Inicializa el color final
				//fixed4 finalColor = fixed4(0, 0, 0, 1);

				//// Itera sobre los colores base
				//for (int i = 0; i < baseColourCount; i++)
				//{
				//	// Calcula la fuerza de dibujo basada en la altura del fragmento
				//	//float drawStrength = heightPercent - baseStartHeights[i];
				//	float drawStrength = saturate(sign(heightPercent - baseStartHeights[i]));

				//	// Mezcla el color actual con el color base ponderado por la fuerza de dibujo
				//	finalColor = finalColor * (1 - drawStrength) + baseColours[i] * drawStrength;
				//}





				//// COLORES CON GRADIENTE
				//float3 worldPos = i.worldPos;
				//float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

				//// Inicializa el color final
				//fixed4 finalColor = fixed4(0, 0, 0, 1);

				//// Itera sobre los colores base
				//for (int i = 0; i < baseColourCount; i++)
				//{
				//	// Calcula la fuerza de dibujo basada en la altura del fragmento
				//	//float drawStrength = heightPercent - baseStartHeights[i];
				//	float drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

				//	// Mezcla el color actual con el color base ponderado por la fuerza de dibujo
				//	finalColor = finalColor * (1 - drawStrength) + baseColours[i] * drawStrength;
				//}


				
				// TEXTURAS
				float3 worldPos = i.worldPos;
				float heightPercent = inverseLerp(3.5f, 27, worldPos.y);

				// Inicializa el color final
				fixed4 finalColor = fixed4(0, 0, 0, 1);
				float3 thisColour = float3(0, 0, 0);

				// Blending
				float3 blendAxes = abs(i.worldNormalWS);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;


				// Itera sobre los colores base
				for (int j = 0; j < layerCount; j++)
				{
					// Calcula la fuerza de dibujo basada en la altura del fragmento
					float drawStrength = inverseLerp(-baseBlends[j]/2 - epsilon, baseBlends[j]/2, heightPercent - baseStartHeights[j]);


					float3 baseColour = baseColours[j] * baseColourStrenght[j];
					float3 textureColour = triplanar(worldPos, baseTexturesScales[j], blendAxes, j) * (1 - baseColourStrenght[j]);

					//// Mezcla el color actual con el color base ponderado por la fuerza de dibujo
					thisColour = thisColour * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
					//finalColor = finalColor * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
				}

				fixed4 color = fixed4(thisColour.x, thisColour.y, thisColour.z, 1);



				//float3 finaColor = triplanar(worldPos, baseTexturesScales[j], blendAxes, j);
				//color = float4(finaColor.x, finaColor.y, finaColor.z, 1);









				//float3 xd = triplanar(worldPos, baseTexturesScales[j], blendAxes, j);
				//color = fixed4(xd.x, xd.y, xd.z, 1);


				/////////////////////////////////////////////////////////////////////////////////////////////////////

				//// Triplanar mapping
				//float3 scaledWorldPos = worldPos / testScale;

				////float3 blendAxes = abs(i.worldNormalWS);
				////blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
				//float3 xProjection = tex2D(testTexture, scaledWorldPos.yz) * blendAxes.x;
				//float3 yProjection = tex2D(testTexture, scaledWorldPos.xz) * blendAxes.y;
				//float3 zProjection = tex2D(testTexture, scaledWorldPos.xy) * blendAxes.z;

				//float3 projection = xProjection + yProjection + zProjection;
				//color = fixed4(projection.x, projection.y, projection.z, 1);
				
				///////////////////////////////////////////////////////////////////////////////////////////////////

				////color = float4(projection.x, projection.y, projection.z, 1);

				
				//color *= fixed4(0.1, 0, 0, 1);


				// Devuelve el color final
				return color;
			}
			ENDHLSL
		}
	}
}