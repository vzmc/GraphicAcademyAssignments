Shader "GraphicAcademy/VertexAnimShader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalOffset ("NormalOffset", Range(-10, 10)) = 0
        _Amplitude ("Amplitude", Range(0, 5)) = 1
        _Frequency ("Frequency", Range(0, 100)) = 1
        _WaveSpeed ("WaveSpeed", Range(0, 10)) = 1
        _Direction ("Direction", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off

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

            float _NormalOffset;
            float _Amplitude;
            float _Frequency;
            float _WaveSpeed;
            float2 _Direction;

            v2f vert (appdata v)
            {
                float4 vertex = v.vertex;
                float2 dir = normalize(_Direction);
                float2 uv = v.uv * dir;
                vertex.xyz = vertex.xyz + (_Amplitude * sin(_Frequency * (uv.x + uv.y) + _WaveSpeed * _Time.y) + _NormalOffset) * v.normal;

                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
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
