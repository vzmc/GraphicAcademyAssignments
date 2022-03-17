Shader "GraphicAcademy/FragmentLightShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(2, 128)) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 posCS : SV_POSITION;
                float4 posWS : TEXCOORD1;
                float3 normalWS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _Gloss;

            v2f vert(appdata v) {
                v2f o;
                o.posCS = UnityObjectToClipPos(v.vertex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 環境光色
                fixed3 ambient = unity_AmbientSky.xyz;

                // 世界空間のライト方向
                half3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.posWS));

                // 拡散反射光色
                fixed3 diffuse = _LightColor0.rgb * saturate(dot(i.normalWS, worldLightDir));

                // 鏡面反射光
                half3 reflectDir = normalize(reflect(-worldLightDir, i.normalWS));
                half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWS.xyz);
                fixed3 specular = _LightColor0.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);

                fixed3 lightColor = ambient + diffuse + specular;

                fixed4 col = tex2D(_MainTex, i.uv) * _Color * fixed4(lightColor, 1);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}
