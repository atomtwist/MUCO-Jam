// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Kabaret/Skybox"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex("Main Tex", CUBE) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 2
        [HideInInspector] __dirty( "", Int ) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "Queue" = "Background+0" "IgnoreProjector" = "True" "IsEmissive" = "true"
        }
        Cull [_CullMode]
        ZWrite Off
        ZTest LEqual
        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers xboxseries playstation switch nomrt
        #pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd
        struct Input
        {
            float3 viewDir;
        };

        uniform int _CullMode;
        uniform samplerCUBE _MainTex;
        uniform float4 _Color;

        inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(0, 0, 0, s.Alpha);
        }

        void surf(Input i, inout SurfaceOutput o)
        {
            o.Emission = (texCUBE(_MainTex, -i.viewDir) * _Color).rgb;
            o.Alpha = 1;
        }
        ENDCG
    }
    CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
125;143;1217;746;1570.722;509.2276;1.552306;True;False
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-1053.5,-96.89998;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;7;-833.5001,-92.89998;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;3;-565.6,66.5;Float;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0.04402518,0.05660379,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-653.5001,-122.4;Inherit;True;Property;_MainTex;Main Tex;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;08708a709a27a3443b01ebe7035de537;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-269.6001,47.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;8;12.26901,-91.67327;Float;False;Property;_CullMode;Cull Mode;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;1;False;0;1;INT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Kabaret/Skybox;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Opaque;;Background;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;True;8;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;5;0
WireConnection;2;1;7;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;0;2;4;0
ASEEND*/
//CHKSM=77A9AD901F6C39AFE5CBECFC4B7D48AC59522129