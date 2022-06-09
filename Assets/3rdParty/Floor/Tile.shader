Shader "Torus/Tile"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Black ("Black", Color) = (0, 0, 0, 1)
        _White ("White", Color) = (1,1,1,1)
        _MatCap("MatCap", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2  tint   : TEXCOORD1;
                float3 wP     : TEXCOORD2;
            };

            sampler2D _MainTex;
            float3 HeadPos;
            float4 _White, _Black;
            sampler _MatCap;
            float _DistMulti;
            float4 _Tint;
            float _TintAmount;

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                
                o.wP = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv   = v.uv;
                o.tint = float2(-mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).y, v.color.x);
                
                return o;
            }
            
            fixed2 GetMatCapUV(float3 normal)
            {
                fixed2 matCapNormal = mul(UNITY_MATRIX_V, float4(normal, 0)).rg;
                            
                return fixed2((matCapNormal.r *  .485) + .5, 
                               matCapNormal.g *  .485 + .5);
            }
                          
            float MatCapX(float3 normal)
            {
                return tex2D(_MatCap, GetMatCapUV(normal)).x;
            }  

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 col = tex2D(_MainTex, i.uv);
                fixed tint = col.z * .85 + .15;
                float dist = length(float2(i.wP.x, i.wP.z) - float2(HeadPos.x, HeadPos.z)) * _DistMulti;
                float d = pow(1 - pow(saturate(dist * .1155), 6), 6);
                float k = 1.0 - saturate(i.tint.x * 30);
                float u  = tint * k;
                
                fixed2 n2d = fixed2(-(col.x * 2 - 1), (col.y * 2 - 1));
                       n2d *= tint * .75 + .25;
                 
                //return fixed4(n2d, 1, 1);      
                fixed tint2 = pow(tint, 2);
                fixed3 n = mul(unity_ObjectToWorld, normalize(fixed3(n2d.x, max(0, 1.0 - n2d.x - n2d.y), n2d.y)));
                       n = normalize(fixed3(n2d.x, max(0, 1.0 - n2d.x - n2d.y), n2d.y));
                //return fixed4(n, 1);  
                fixed matcap = MatCapX(n) * tint2;
                     
                //return fixed4(n, 1);     
                fixed t = ((d * .895 + .105) * ((1.0 - pow(saturate(dist * .021), 3)))) * 1.2;
                
                //float whateverthisis = 
                float dt = (1.0 - pow(1.0 - saturate(dot(-normalize(i.wP - _WorldSpaceCameraPos), n)), 2)) * tint2 * .85 + .15;
              
                     dt = lerp(1, dt, _TintAmount);
                     t  = lerp(1, t, _TintAmount);
                     
                return ((lerp(_Black, _White, u) * t * dt  + matcap * t * .2 * k)* (i.tint.y * .1 + .9)) *_Tint;
            }
            ENDCG
        }
    }
}
