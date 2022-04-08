Shader "GraphicAcademy/MultiLightShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(2, 128)) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        // 全Pass共通部分
        CGINCLUDE

        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
        #include "Lighting.cginc"
    
        struct appdata 
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct v2f 
        {
            float4 posCS : SV_POSITION;
            float4 posWS : TEXCOORD1;
            float3 normalWS : NORMAL;
            float2 uv : TEXCOORD0;
            fixed3 ambient : COLOR;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _Color;
        fixed _Gloss;

        v2f vert(appdata v) 
        {
            v2f o;
            o.posCS = UnityObjectToClipPos(v.vertex);
            o.posWS = mul(unity_ObjectToWorld, v.vertex);
            o.normalWS = UnityObjectToWorldNormal(v.normal);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.ambient = 0;

            #if UNITY_SHOULD_SAMPLE_SH
                // まず環境光色を取得する
                // o.ambient = UNITY_LIGHTMODEL_AMBIENT.xyz; 古い
                o.ambient = unity_AmbientSky.xyz;
                #ifdef VERTEXLIGHT_ON
                    // 非重要のライト4つまで加える
                    o.ambient += Shade4PointLights (
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].xyz, unity_LightColor[1].xyz, unity_LightColor[2].xyz, unity_LightColor[3].xyz,
                        unity_4LightAtten0,
                        o.posWS, o.normalWS);
                #endif
                // それ以上のライトは球面調和ライトとして加える
                o.ambient += max(0, ShadeSH9(float4(o.normalWS, 1)));
            #endif

            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            // 自身の色
            fixed3 albedo = tex2D(_MainTex, i.uv) * _Color;

            // 世界空間のライト方向
            //half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
            half3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.posWS));
            // 拡散反射光色
            fixed3 diffuse = _LightColor0.rgb * saturate(dot(i.normalWS, worldLightDir));

            // 鏡面反射光
            half3 reflectDir = normalize(reflect(-worldLightDir, i.normalWS));
            //half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWS.xyz);
            half3 viewDir = normalize(UnityWorldSpaceViewDir(i.posWS.xyz));
            fixed3 specular = _LightColor0.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);

            fixed atten = 1.0;
            #ifdef UNITY_PASS_FORWARDADD
                // ForwardAddのPassのみ光の減衰を計算する
                #ifdef POINT
                    float3 lightCoord = mul(unity_WorldToLight, i.posWS).xyz;
                    atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                #elif SPOT
                    float4 lightCoord = mul(unity_WorldToLight, i.posWS);
                    atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                #endif
            #endif

            fixed3 lightColor = i.ambient + (diffuse + specular) * atten;

            fixed3 finalColor = albedo * lightColor;
            return fixed4(finalColor, 1);
        }

        ENDCG

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            ENDCG
        }

        Pass
        {
            Tags { "LightMode"="ForwardAdd" }

            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            ENDCG
        }
    }

    FallBack "Diffuse"
}
