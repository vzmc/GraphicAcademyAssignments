Shader "GraphicAcademy/VertexAnimShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(0, 2)) = 1
        _Frequency ("Frequency", Range(0, 2)) = 1
        _WaveSpeed ("Wavelength", Range(0, 2)) = 1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Amplitude;
            float _WaveSpeed;
            float _Frequency;

            v2f vert (appdata v)
            {
                v2f o;

                float4 vertex = v.vertex;
                vertex.y = _Amplitude * sin(_Frequency * (vertex.x + _WaveSpeed * _Time.y));

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
