Shader "GraphicAcademy/ToonShader"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _Outline ("Outline", Range(0, 1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _SpecularScale ("Specular Scale", Range(0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
                ZFail Keep
            }

            Cull Back

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Ramp;
            fixed4 _Specular;
            fixed _SpecularScale;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : NORMAL;
                float3 posWS : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.posCS = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX (v.uv, _MainTex);
                o.normalWS  = UnityObjectToWorldNormal(v.normal);
                o.posWS = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 normalWS = normalize(i.normalWS);
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.posWS));
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.posWS));
                float3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                // 自身の色
                fixed4 texColor = tex2D (_MainTex, i.uv);
                fixed3 albedo = texColor.rgb * _Color.rgb;

                // 環境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                // 拡散反射光
                float diff =  dot(normalWS, worldLightDir);
                diff = (diff * 0.5 + 0.5);
                fixed3 diffuse = _LightColor0.rgb * tex2D(_Ramp, float2(diff, diff)).rgb;

                // 鏡面反射光
                float spec = dot(normalWS, worldHalfDir);
                float w = fwidth(spec) * 2.0;
                fixed3 specular = _Specular.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);

                // 最終合成色
                fixed3 finalColor = (ambient + diffuse) * albedo + specular;
                return fixed4(finalColor, 1.0);
            }

            ENDCG
        }

        Pass
        {
            Name "OUTLINE"
            
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
                ZFail Keep
            }

            Cull Front

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Outline;
            fixed4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // ViewSpaceで頂点を拡張する
                float4 posVS = float4(UnityObjectToViewPos(v.vertex), 1.0);
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                normal.z = -0.5;
                posVS = posVS + float4(normalize(normal), 0) * _Outline;
                o.posCS = mul(UNITY_MATRIX_P, posVS);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(_OutlineColor.rgb, 1);
            }

            ENDCG
        }

    }
    FallBack "Standard"
}
