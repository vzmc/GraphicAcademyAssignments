Shader "GraphicAcademy/DitherShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
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
                float4 posOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Alpha;
            sampler2D _DitherMaskLOD2D;

            v2f vert (appdata v, out float4 posCS : SV_POSITION)
            {
                posCS = UnityObjectToClipPos(v.posOS);

                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
                // スクリーン座標から、0 ~ 1の範囲のUV座標に変換する
                float2 ditherUV = screenPos % 4 / 4;
                // ディザテキスチャの縦方向は4 x 16pxなので、4x4の範囲にサンプリングするため、y方向のUVを0~1の範囲から0~1/16に変換する
                ditherUV.y /= 16.0;
                // 設定した0~1範囲のAlpha値で、UVのy方向のOffset範囲を0~15/16に変換する
                ditherUV.y += _Alpha * (15.0 / 16.0);

                // _DitherMaskLOD2Dからアルファ値を取って、0ならclipして、1なら描画する
                float dither = tex2D(_DitherMaskLOD2D, ditherUV).a;
                clip(dither - 0.5);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
