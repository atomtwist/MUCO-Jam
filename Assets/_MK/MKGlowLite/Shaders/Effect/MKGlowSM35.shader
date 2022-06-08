//////////////////////////////////////////////////////
// MK Glow Shader SM35								//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.           
 //
//////////////////////////////////////////////////////
Shader "Hidden/MK/Glow/MKGlowSM35"
{
	SubShader
	{
		Tags {"LightMode" = "Always" "RenderType"="Opaque" "PerformanceChecks"="False"}
		Cull Off ZWrite Off ZTest Always

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Copy - 0
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vertSimple
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "../Inc/Copy.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Presample - 1
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vertSimple
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile __ _MK_PPSV2
			#define _MK_BLOOM

			#include "../Inc/Presample.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Downsample - 2
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vertSimple
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile __ _MK_PPSV2
			#define _MK_BLOOM

			#include "../Inc/Downsample.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Upsample - 3
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile __ _MK_PPSV2
			#define _MK_BLOOM

			#include "../Inc/Upsample.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Composite - 4
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile __ _MK_PPSV2
			#pragma multi_compile __ _MK_LEGACY_BLIT

			#pragma multi_compile __ _MK_LENS_SURFACE

			#include "../Inc/Composite.hlsl"
			ENDHLSL
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
        // Debug - 5
        /////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile __ _MK_PPSV2
			#pragma multi_compile __ _MK_LEGACY_BLIT
			
			#pragma multi_compile __ _MK_DEBUG_RAW_BLOOM _MK_DEBUG_COMPOSITE
			#pragma multi_compile __ _MK_LENS_SURFACE

			#include "../Inc/Debug.hlsl"
			ENDHLSL
		}
	}
	FallBack "Hidden/MK/Glow/MKGlowSM25"
}
