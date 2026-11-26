Shader "Cave/BuildIn/ChannelBlendShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _BlendAdditionalTex_L("BlendAdditionalTex_L", 2D) = "black" {}
        _BlendAdditionalTex_R("BlendAdditionalTex_R", 2D) = "black" {}
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

			fixed4 frag( v2f i ) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
        
        //pass1 r-gb
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

            float4 frag (v2f i) : SV_Target
            {
                float4 origin = tex2D(_MainTex, i.uv);
                float4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                float4 right = tex2D(_BlendAdditionalTex_R, i.uv);

                fixed alpha_left = left.a + origin.a * (1.0 - left.a);
                fixed alpha_right = right.a + origin.a * (1.0 - right.a);
                fixed4 col;
                col.r = (left.r * left.a + origin.r * origin.a * (1.0 - left.a)) / alpha_left;
                col.g = (right.g * right.a + origin.g * origin.a * (1.0 - right.a)) / alpha_right;
                col.b = (right.b * right.a + origin.b * origin.a * (1.0 - right.a)) / alpha_right;
                col.a = max(alpha_left, alpha_right);
                return col;
            }
            ENDCG
        }
        
        //pass2 rg-b
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                fixed4 right = tex2D(_BlendAdditionalTex_R, i.uv);

                fixed alpha_left = left.a + origin.a * (1.0 - left.a);
                fixed alpha_right = right.a + origin.a * (1.0 - right.a);
                fixed4 col;
                col.r = (left.r * left.a + origin.r * origin.a * (1.0 - left.a)) / alpha_left;
                col.g = (left.g * left.a + origin.g * origin.a * (1.0 - left.a)) / alpha_left;
                col.b = (right.b * right.a + origin.b * origin.a * (1.0 - right.a)) / alpha_right;
                col.a = max(alpha_left, alpha_right);
                return col;
            }
            ENDCG
        }

        //pass3 rb-g
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 origin = tex2D(_MainTex, i.uv);
                fixed4 left = tex2D(_BlendAdditionalTex_L, i.uv);
                fixed4 right = tex2D(_BlendAdditionalTex_R, i.uv);

                fixed alpha_left = left.a + origin.a * (1.0 - left.a);
                fixed alpha_right = right.a + origin.a * (1.0 - right.a);
                fixed4 col;
                col.r = (left.r * left.a + origin.r * origin.a * (1.0 - left.a)) / alpha_left;
                col.g = (right.g * right.a + origin.g * origin.a * (1.0 - right.a)) / alpha_right;
                col.b = (left.b * left.a + origin.b * origin.a * (1.0 - left.a)) / alpha_left;
                col.a = max(alpha_left, alpha_right);
                return col;
            }
            ENDCG
        }
    }
}
