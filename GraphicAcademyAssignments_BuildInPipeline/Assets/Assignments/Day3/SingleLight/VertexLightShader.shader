Shader "GraphicAcademy/VertexLightShader"
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

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed3 lightColor : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // 環境光
                fixed3 ambient = unity_AmbientSky.xyz;

                // 世界空間の法線方向とライト方向
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

                // 拡散反射光
                fixed3 diffuse = _LightColor0.rgb * saturate(dot(worldNormal, worldLightDir));

                // 鏡面反射光
                half3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
                half3 viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                fixed3 specular = _LightColor0.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);
                
                // 環境光 + 拡散反射光 + 鏡面反射光
                o.lightColor = ambient + diffuse + specular;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * fixed4(i.lightColor, 1);
                return col;
            }
            ENDCG
        }
    }
}
