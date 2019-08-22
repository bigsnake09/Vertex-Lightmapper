Shader "VLM Example/Terrain (4 Blend)"
{
    Properties
    {
        _MainTex("Top", 2D) = "white" {}
        _MainTex2("Side Upper", 2D) = "white" {}
        _MainTex3("Side Lower", 2D) = "white" {}
        _MainTex4("Bottom", 2D) = "white" {}
		_FloorMin("Floor Min", Range(0, 1)) = 0
		_FloorMax("Floor Max", Range(0, 1)) = 1
		_WallDivideMin("Wall Divide Min", Float) = -0.5
		_WallDivideMax("Wall Divide Max", Float) = 0.5
		_ColorVariationFrequency("Color Variation Frequency", Float) = 0.5
		_ColorVariationAmount("Color Variation Amount", Float) = 0.5
		_ColorVariationTint("Color Variation Tint", Color) = (0, 0, 0, 0)
		_ColorVariationMin("Color Variation Min", Float) = 0
		_ColorVariationMax("Color Variation Max", Float) = 1
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
    }
		Category{
			SubShader
			{
				Pass
				{
					CGPROGRAM
					#include "UnityCG.cginc"
					#include "ShaderExtras.cginc"

					#pragma vertex vert
					#pragma fragment frag

					struct v2f
					{
						float4 position : SV_POSITION;
						half2 texcoord : TEXCOORD;
						half3 textureBlend : TEXCOORD2;
						fixed4 color : COLOR;
					};

					sampler2D _MainTex;
					sampler2D _MainTex2;
					sampler2D _MainTex3;
					sampler2D _MainTex4;
					float _FloorMin;
					float _FloorMax;
					float _WallDivideMin;
					float _WallDivideMax;
					float _ColorVariationFrequency;
					float _ColorVariationAmount;
					float4 _ColorVariationTint;
					float _ColorVariationMin;
					float _ColorVariationMax;
					float4 _MainTex_ST;
					float4 _Color;

					v2f vert(appdata_full v)
					{
						v2f o;

						float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);

						o.position = UnityObjectToClipPos(v.vertex);

						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);;
						o.color = v.color * _Color;

						float colorVariation = lerp(_ColorVariationMin, _ColorVariationMax, sin(length(worldPosition.xyz) * _ColorVariationFrequency) + 1.0f / 2.0f);
						o.color.rgb = lerp(o.color.rgb, o.color.rgb * _ColorVariationTint, colorVariation * _ColorVariationAmount);

						// normals
						half3 worldNormal = UnityObjectToWorldNormal(v.normal);
						float normalDot = dot(worldNormal, float3(0, 1, 0));
						float vertToHoriDot = 1 - abs(normalDot);

						o.textureBlend.x = InverseLerp(_FloorMin, _FloorMax, vertToHoriDot);
						o.textureBlend.y = 1.0f - round(normalDot + 1 / 2.0f);
						o.textureBlend.z = 1.0f - InverseLerp(_WallDivideMin, _WallDivideMax, worldPosition.y);

						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 top = tex2D(_MainTex, i.texcoord);
						fixed4 bottom = tex2D(_MainTex4, i.texcoord);
						fixed4 walls = tex2D(_MainTex2, i.texcoord);
						fixed4 walls2 = tex2D(_MainTex3, i.texcoord);

						return lerp(lerp(top, bottom, i.textureBlend.y), lerp(walls, walls2, i.textureBlend.z), i.textureBlend.x) * i.color;
					}

					ENDCG
				}
			}
		}
}
