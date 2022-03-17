Shader "GraphicAcademy/MultiTexturesShader"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _SpecularMask ("Specular Mask", 2D) = "white" {}
        _SpecularScale ("Specular Scale", Range(0.0, 1.0)) = 1.0
        _Gloss ("Gloss", Range(2, 128)) = 8
        _AOMask ("AO Mask", 2D) = "white" {}
        _AOScale ("AO Scale", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SpecularMask;
            float _SpecularScale;
            float _Gloss;
            sampler2D _AOMask;
            float _AOScale;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 posWS : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.posCS = UnityObjectToClipPos(v.vertex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normalWS = normalize(i.normalWS);
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.posWS));
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.posWS));
                float3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed aoMask = 1 - (1 - tex2D(_AOMask, i.uv).r) * _AOScale;

                fixed3 diffuse = _LightColor0.rgb * saturate(dot(i.normalWS, worldLightDir));

                fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
                fixed3 specular = _LightColor0.rgb * pow(saturate(dot(normalWS, worldHalfDir)), _Gloss);

                fixed3 finalColor = (ambient * aoMask + diffuse + specular * specularMask) * albedo;
                return fixed4(finalColor, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Standard"
}
