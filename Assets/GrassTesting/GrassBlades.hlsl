#ifndef GRASSBLADES_INCLUDED
#define GRASSBLADES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"//
#include "NMGGrassBladesGraphicsHelpers.hlsl"


struct DrawVertex {
	float3 positionWS; // Posicion global
	float height; // Altura de este vertice dentro del aspa de hierba
};

struct DrawTriangle {
	float3 lightingNormalWS; // La normal global, para usar en el algoritmo de luz
	DrawVertex vertices[3]; // Los tres puntos del triangulo
};

// Este buffer contiene el mesh generado
StructuredBuffer<DrawTriangle> _DrawTriangles;

struct VertexOutput {
	float uv : TEXCOORD0;
	float3 positionWS : TEXCOORD1;
	float3 normalWS : TEXCOORD2;

	float4 positionCS : SV_POSITION;
};

// Propiedades
float4 _BaseColor;
float4 _TipColor;

VertexOutput Vertex(uint vertexID: SV_VertexID) {

	VertexOutput output = (VertexOutput)0;

	// Obtener el verice del buffer
	// Como el buffer esta estructurado en triangulos, necesitamos dividir el vertexID por tres
	// para conseguir el triangulo, y despues modulo de tres para obtener el vertex del triangulo
	DrawTriangle tri = _DrawTriangles[vertexID / 3];
	DrawVertex input = tri.vertices[vertexID % 3];

	output.positionWS = input.positionWS;
	output.normalWS = tri.lightingNormalWS;
	output.uv = input.height;
	output.positionCS = TransformWorldToHClip(input.positionWS);

	return output;
}

half4 Fragment(VertexOutput input) : SV_Target{

	InputData lightingInput = (InputData)0;
	lightingInput.positionWS = input.positionWS;
	lightingInput.normalWS = input.normalWS;
	lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
	lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

	// Lerp entre el color de la base y de la punta
	float colorLerp = input.uv;
	float3 albedo = lerp(_BaseColor.rgb, _TipColor.rgb, input.uv);

	// Algoritmo simple de lit de URP
	return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1, 0); // Añadido un parametro mas al final //////////////////////////////////
}

#endif