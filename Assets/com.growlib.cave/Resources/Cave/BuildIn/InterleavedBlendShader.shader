Shader "Cave/BuildIn/InterleavedBlendShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _BlendAdditionalTex_L("BlendAdditionalTex_L", 2D) = "black" {}
        _BlendAdditionalTex_R("BlendAdditionalTex_R", 2D) = "black" {}

	   _Cols ("Cols", Int) = 1
	   _Rows ("Rows", Int) = 1
    }

    SubShader
    {
		//pass0 nothing
		Pass 
		{
			Cull Off ZWrite Off ZTest Always

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            //sampler2D _BlendAdditionalTex_L;
            //sampler2D _BlendAdditionalTex_R;

			int _Cols;
			int _Rows;

			fixed4 frag( v2f i ) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}

		//pass1 Interleaved Row
		Pass 
		{
			Cull Off ZWrite Off ZTest Always

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _BlendAdditionalTex_L;
            sampler2D _BlendAdditionalTex_R;
            
			int _Cols;
			int _Rows;

			fixed4 frag( v2f i ) : SV_Target
			{
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                fixed4 right = tex2D(_BlendAdditionalTex_R, i.uv);

				uint intRow = i.uv.y * _Rows;
				uint odd = intRow - intRow / 2 * 2;
				fixed4 mixed = odd == 0 ? left : right;

                fixed alpha = mixed.a + origin.a * (1.0 - mixed.a);
                fixed4 col;
                col.rgb = (mixed.rgb * mixed.a + origin.rgb * origin.a * (1.0 - mixed.a)) / alpha;
                col.a = alpha;
                return col;
			}
			ENDCG
		}

		//pass2 Interleaved Column
		Pass 
		{
			Cull Off ZWrite Off ZTest Always

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _BlendAdditionalTex_L;
            sampler2D _BlendAdditionalTex_R;
            
			int _Cols;
			int _Rows;

			fixed4 frag( v2f i ) : SV_Target
			{
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                fixed4 right = tex2D(_BlendAdditionalTex_R, i.uv);

				uint intColumn = i.uv.x * _Cols;
				uint odd = intColumn - intColumn / 2 * 2;
				fixed4 mixed = odd == 0 ? left : right;

                fixed alpha = mixed.a + origin.a * (1.0 - mixed.a);
                fixed4 col;
                col.rgb = (mixed.rgb * mixed.a + origin.rgb * origin.a * (1.0 - mixed.a)) / alpha;
                col.a = alpha;
                return col;
			}
			ENDCG
		}

		//pass3 Interleaved Checkerboard
		Pass 
		{
			Cull Off ZWrite Off ZTest Always

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _BlendAdditionalTex_L;
            sampler2D _BlendAdditionalTex_R;
            
			int _Cols;
			int _Rows;

			fixed4 frag( v2f i ) : SV_Target
			{
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                fixed4 right = tex2D(_BlendAdditionalTex_R, i.uv);

				uint intRow = i.uv.y * _Rows;
				uint intColumn = i.uv.x * _Cols;
				uint oddRow = intRow - intRow / 2 * 2;
				uint oddColumn = intColumn - intColumn / 2 * 2;
				uint switcher = oddRow + oddColumn - 2 * oddRow * oddColumn;
				fixed4 mixed = switcher == 0 ? left : right;

                fixed alpha = mixed.a + origin.a * (1.0 - mixed.a);
                fixed4 col;
                col.rgb = (mixed.rgb * mixed.a + origin.rgb * origin.a * (1.0 - mixed.a)) / alpha;
                col.a = alpha;
                return col;
			}
			ENDCG
		}
   
    }
}
