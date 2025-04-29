Shader "Custom/surfaceCurvature"
{
    Properties
    {
        _CurveAmount ("Curve Strength", Float) = 0.0005
        _MainTex      ("Albedo (RGB)", 2D)    = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float      _CurveAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // compute quadratic displacement
                float x = v.vertex.x;
                float z = v.vertex.z;
                float bend = _CurveAmount * (x*x + z*z);
                v.vertex.y += bend;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}

