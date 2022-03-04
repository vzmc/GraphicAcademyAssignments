Shader "GraphicAcademy/SpriteAnimShader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WidthSeparate ("Width Separate", int) = 8
        _HeightSeparate ("Height Separete", int) = 8
        _TimePerFrame ("Time Per Frame", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            // アルファ値で透過するために、RenderTypeとQueueをTransparentに変更
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        // アルファブレンド方式を設定
        Blend SrcAlpha OneMinusSrcAlpha

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
            int _WidthSeparate;
            int _HeightSeparate;
            float _TimePerFrame;

            v2f vert (appdata v)
            {
                // 0-1のUVを分割する
                float2 unitUV = 1.0 / float2(_WidthSeparate, _HeightSeparate);

                // 今のコマのIndexを算出
                int index = floor(_Time.y / _TimePerFrame % (_WidthSeparate * _HeightSeparate));

                // 行番号、列番号算出
                int col = index % _WidthSeparate;
                int row = index / _WidthSeparate;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // コマのUV座標算出(rowを逆にすれば、上->下順になる)
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) * unitUV + float2(col, -row) * unitUV;
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
