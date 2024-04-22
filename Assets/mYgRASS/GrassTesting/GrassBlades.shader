Shader "Unlit/GrassBlades"
{
    Properties
    {
        _BaseColor("Base color", Color) = (0, 0.5, 0, 1) // Color de la base
        _TipColor("Tip color", Color) = (0, 1, 0, 1) // Color de la punta
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True"}
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            Cull Off // No Cull ya que la hierba tiene que ser de doble lado

            HLSLPROGRAM

            // Indica que el shader requiere un compute shader
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Palabras clave de luces y sombras
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            // Registrar funciones
            #pragma vertex Vertex
            #pragma fragment Fragment

            // make fog work
            #pragma multi_compile_fog

            // Incluir el archivo de logica
            #include "GrassBlades.hlsl"

            ENDHLSL
        }
    }
}
