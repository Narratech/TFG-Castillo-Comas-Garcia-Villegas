Shader "Custom/HLSL_ColorHeight"
{
	Properties
	{

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

				// Normales de cada vertice
				float3 worldNormalWS : TEXCOORD2;
			};

			// Texturas de cada capa
			sampler2D texture_0;
			sampler2D texture_1;
			sampler2D texture_2;
			sampler2D texture_3;
			sampler2D texture_4;
			sampler2D texture_5;
			sampler2D texture_6;
			sampler2D texture_7;

			// Altura minima y maxima entre las que se calculan las texturas
			float minHeight = 10;
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

			const static float epsilon = 1E-4;


			// -------------------------------------------------------------------------------- //
			//								 VERTEX SHADER										//
			// -------------------------------------------------------------------------------- //

			// Aqui se convierte un vertice de posicion en 3D a posicion en pantalla
			v2f vert(appdata v)
			{
				// Output
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calcular la posición del mundo y asignarla

				o.worldNormalWS = mul(unity_ObjectToWorld, v.worldNormalOS).xyz;

				return o;
			}

			// Metodo mio
			float inverseLerp(float a, float b, float value) {
				return saturate((value - a) / (b - a));
			}


			float3 calculateProjections(sampler2D thisTexture, float3 scaledWorldPos, float3 blendAxes) {

				float3 xProjection = tex2D(thisTexture, scaledWorldPos.yz) * blendAxes.x;
				float3 yProjection = tex2D(thisTexture, scaledWorldPos.xz) * blendAxes.y;
				float3 zProjection = tex2D(thisTexture, scaledWorldPos.xy) * blendAxes.z;

				return xProjection + yProjection + zProjection;
			}
			float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
				// Triplanar mapping
				float3 scaledWorldPos = worldPos / scale;

				sampler2D useThisTexture;
				switch (textureIndex) {
				case 0:
					return calculateProjections(texture_0, scaledWorldPos, blendAxes);
					break;
				case 1:
					return calculateProjections(texture_1, scaledWorldPos, blendAxes);
					break;
				case 2:
					return calculateProjections(texture_2, scaledWorldPos, blendAxes);
					break;
				case 3:
					return calculateProjections(texture_3, scaledWorldPos, blendAxes);
					break;
				case 4:
					return calculateProjections(texture_4, scaledWorldPos, blendAxes);
					break;
				case 5:
					return calculateProjections(texture_5, scaledWorldPos, blendAxes);
					break;
				case 6:
					return calculateProjections(texture_6, scaledWorldPos, blendAxes);
					break;
				case 7:
					return calculateProjections(texture_7, scaledWorldPos, blendAxes);
					break;
				default:
					return float3(0, 0, 0);
					break;
				}
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
				// TEXTURAS
				float3 worldPos = i.worldPos;
				float heightPercent = inverseLerp(10, 60, worldPos.y);

				// Inicializa el color final
				float3 thisColour = float3(0, 0, 0);

				// Blending
				float3 blendAxes = abs(i.worldNormalWS);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

				// Itera sobre los colores base
				for (int j = 0; j < layerCount; j++)
				{
					// Calcula la fuerza de dibujo basada en la altura del fragmento
					float drawStrength = inverseLerp(-baseBlends[j]/2 - epsilon, baseBlends[j]/2, heightPercent - baseStartHeights[j]);

					// Mezclar color base y la textura teniendo en cuenta la intensidad de los colores
					float3 baseColour = baseColours[j] * baseColourStrenght[j];
					float3 textureColour = triplanar(worldPos, baseTexturesScales[j], blendAxes, j) * (1 - baseColourStrenght[j]);

					// Mezcla el color actual con el color base ponderado hasta ahora
					thisColour = thisColour * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
				}

				fixed4 color = fixed4(thisColour.x, thisColour.y, thisColour.z, 1);

				// Devuelve el color final
				return color;
			}
			ENDHLSL
		}
	}
}