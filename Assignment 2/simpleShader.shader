// UVic CSC 305, 2019 Spring
// Assignment 01
// Name: Alwien Dippenaar 
// UVic ID: V00849850

Shader "Unlit/simpleShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}

		_Scale("UVScale", Float) = 20.0

		_GrassTex("Grass", 2D) = "white" {}
		_SandTex("Sand", 2D) = "white" {}
		_WaterTex("Water", 2D) = "white" {}
		_SnowTex("Snow", 2D) = "white" {}

		_Color("Color", Color) = (1, 1, 1, 1)

		_Shininess ("Shininess", Float) = 10
		_SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque"
		"LightMode" = "ForwardBase" }
		LOD 200

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma require 2darray

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;

			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float height : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float4 worldpos : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _GrassTex;
			float4 _GrassTex_ST;

			sampler2D _SandTex;
			float4 _SandTex_ST;

			sampler2D _WaterTex;
			float4 _WaterTex_ST;

			sampler2D _SnowTex;
			float4 _SnowTex_ST;

			float _Scale;

			uniform float4 _Color;
			uniform float4 _SpecularColor;
			uniform float _Shininess;

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.height = v.vertex.y;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if (o.height <= .3) { o.uv = TRANSFORM_TEX(v.uv, _WaterTex); }
				else if (o.height < .5) { o.uv = TRANSFORM_TEX(v.uv, _SandTex); }
				else if (o.height < 2.4) { o.uv = TRANSFORM_TEX(v.uv, _GrassTex); }
				else { o.uv = TRANSFORM_TEX(v.uv, _SnowTex); }

				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldpos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}


			float3 frag(v2f i) : SV_Target // focus on this one for the assignment
			{
				float3 normalDirection = normalize(i.normal);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldpos.xyz);

				float3 vert2LightSource = _WorldSpaceLightPos0.xyz - i.worldpos.xyz;
				float oneOverDistance = 1.0 / length(vert2LightSource);
				float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
				float3 lightDirection = _WorldSpaceLightPos0.xyz - i.worldpos.xyz * _WorldSpaceLightPos0.w;

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if (dot(i.normal, lightDirection) < 0.0) //Light on the wrong side - no specular
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					//Specular component
					specularReflection = attenuation * _LightColor0.rgb * _SpecularColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}
				
				float3 col = tex2D(_MainTex, i.worldpos.xz / _Scale);
				if (i.height <= .3) { col = (ambientLighting + diffuseReflection) * tex2D(_WaterTex, i.worldpos.xz / _Scale) + specularReflection; }
				else if (i.height < .5) { col = (ambientLighting + diffuseReflection) * tex2D(_SandTex, i.worldpos.xz / _Scale*20); }
				else if (i.height < 2.4) { col = (ambientLighting + diffuseReflection) * tex2D(_GrassTex, i.worldpos.xz / _Scale); }
				else { col = (ambientLighting + diffuseReflection) * tex2D(_SnowTex, i.worldpos.xz / _Scale); }
				return col;
			}

			ENDCG
		}
	}
}
