Shader "MyShaders/TransparentUI"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }


		Pass
		{
			ZTest Always
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _Cutoff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 rgba = tex2D(_MainTex, i.uv);
				fixed3 col = rgba.rgb;
				clip(rgba.a - _Cutoff);


				fixed4 finalColor = fixed4(col*_Color.rgb,_Color.a);
				return finalColor;
			}
			ENDCG
		}
	}
}
