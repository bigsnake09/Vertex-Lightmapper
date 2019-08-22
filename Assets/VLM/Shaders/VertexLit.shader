Shader "VLM Example/VertexLit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Illum ("Illumination", 2D) = "black" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _Illum;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color * _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				col += tex2D(_Illum, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
