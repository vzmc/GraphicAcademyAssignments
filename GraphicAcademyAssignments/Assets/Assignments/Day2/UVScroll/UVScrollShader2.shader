Shader "GraphicAcademy/UVScrollShader2"
{
    // UVScroll + SinWave
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Direction ("Direction", Vector) = (0, 1, 0, 0)
        _ScrollSpeed ("ScrollSpeed", Range(0, 2)) = 1
        _NormalOffset ("NormalOffset", Range(-10, 10)) = 0
        _Amplitude ("Amplitude", Range(0, 5)) = 1
        _Frequency ("Frequency", Range(0, 100)) = 1
        _WaveSpeed ("WaveSpeed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Direction;
            float _ScrollSpeed;
            float _NormalOffset;
            float _Amplitude;
            float _Frequency;
            float _WaveSpeed;

            v2f vert (appdata v)
            {
                float2 dir = normalize(_Direction);
                float2 uv = v.uv + dir * _Time.y * _ScrollSpeed;

                float4 vertex = v.vertex;
                float2 waveUV = v.uv * dir;
                vertex.xyz = vertex.xyz + (_Amplitude * sin(_Frequency * (waveUV.x + waveUV.y) + _WaveSpeed * _Time.y) + _NormalOffset) * v.normal;

                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
