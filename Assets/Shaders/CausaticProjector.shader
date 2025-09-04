Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _CausticTex  ("Texture", 2D) = "white" {}
        _Strength ("Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORDO;
                float2 uv :TEXCOORDO1;
            };

            sampler2D _CausticTex;
            float4 _CausticTex_ST;
            float _Strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _CausticTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uv = i.worldPos.xz / _CausticTex_ST.xy;
                fixed4 tex = tex2D(_CausticTex, i.uv) * _Strength;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return tex;
            }
            ENDCG
        }
    }
}
