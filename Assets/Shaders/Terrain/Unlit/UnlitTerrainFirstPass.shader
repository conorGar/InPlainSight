// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Terrain/UnlitTerrainFirstPass" //The Shaders Name
{
	//The inputs shown in the material panel
	Properties
	{
		_Control("Control", 2D) = "white" {}
		_Splat0("Splat0", 2D) = "white" {}
		_Splat1("Splat1", 2D) = "white" {}
		_Splat2("Splat2", 2D) = "white" {}
		_Splat3("Splat3", 2D) = "white" {}
	}

		SubShader
	{
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}//A bunch of settings telling Unity a bit about the shader.

		LOD 200
		AlphaToMask Off

		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			ZTest LEqual
			ZWrite On
			Blend Off//No transparency
			Cull Back//Culling specifies which sides of the models faces to hide.


			CGPROGRAM
			// compile directives
			#pragma vertex Vertex
			#pragma fragment Pixel
			#pragma target 2.5
			#pragma multi_compile_fog
			#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
			#pragma multi_compile_fwdbase novertexlight
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"

			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal

			//Make our inputs accessible by declaring them here.
			sampler2D _Control;
			float4 _Control_ST;
			float4 _Control_HDR;

			sampler2D _Splat0;
			float4 _Splat0_ST;
			float4 _Splat0_HDR;

			sampler2D _Splat1;
			float4 _Splat1_ST;
			float4 _Splat1_HDR;

			sampler2D _Splat2;
			float4 _Splat2_ST;
			float4 _Splat2_HDR;

			sampler2D _Splat3;
			float4 _Splat3_ST;
			float4 _Splat3_HDR;

#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
#if SHADER_TARGET < 30
	#define SSUNITY_BRDF_PBS 3
#elif defined(UNITY_PBS_USE_BRDF3)
	#define SSUNITY_BRDF_PBS 3
#elif defined(UNITY_PBS_USE_BRDF2)
	#define SSUNITY_BRDF_PBS 2
#elif defined(UNITY_PBS_USE_BRDF1)
	#define SSUNITY_BRDF_PBS 1
#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
	// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
	#define SSUNITY_BRDF_PBS 1
#else
	#error something broke in auto-choosing BRDF (Shader Sandwich)
#endif
#endif

	//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones
	inline float3 UnityObjectToWorldNormalNew(in float3 norm)
	{
		#ifdef UNITY_ASSUME_UNIFORM_SCALING
		return UnityObjectToWorldDir(norm);
		#else
		// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
		return normalize(mul(norm, (float3x3)unity_WorldToObject));
		#endif
	}

float4 GammaToLinear(float4 col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
	//Best programming evar XD
#else
	col.rgb = pow(col,2.2);
#endif
return col;
}

float4 GammaToLinearForce(float4 col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
	//Best programming evar XD
#else
	col.rgb = pow(col,2.2);
#endif
return col;
}

float4 LinearToGamma(float4 col) {
	return col;
}

float4 LinearToGammaForWeirdSituations(float4 col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
		col.rgb = pow(col,2.2);
	#endif
	return col;
}

float3 GammaToLinear(float3 col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
	//Best programming evar XD
#else
	col = pow(col,2.2);
#endif
return col;
}

float3 GammaToLinearForce(float3 col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
	//Best programming evar XD
#else
	col = pow(col,2.2);
#endif
return col;
}

float3 LinearToGamma(float3 col) {
	return col;
}

float GammaToLinear(float col) {
	#if defined(UNITY_COLORSPACE_GAMMA)
	//Best programming evar XD
#else
	col = pow(col,2.2);
#endif
return col;
}








struct VertexShaderInput {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel {
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float2 uv_Control : TEXCOORD2;
	float2 uv_Splat0 : TEXCOORD3;
	float2 uv_Splat1 : TEXCOORD4;
	float2 uv_Splat2 : TEXCOORD5;
	float2 uv_Splat3 : TEXCOORD6;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh : TEXCOORD7;
#endif
	#define pos position
		SHADOW_COORDS(8)
		UNITY_FOG_COORDS(9)
#undef pos
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap : TEXCOORD10;
	#endif
};

struct VertexData {
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldViewDir;
	float2 uv_Control;
	float2 uv_Splat0;
	float2 uv_Splat1;
	float2 uv_Splat2;
	float2 uv_Splat3;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float Mask0;
	float Mask1;
	float Mask2;
	float Mask3;
	float Atten;
};
//OutputPremultiplied: True
//UseAlphaGenerate: True
half4 Diffuse(VertexData vd, UnityGI gi, UnityGIInput giInput) {
	half4 Surface = half4(0,0,0,0);
	//Generate layers for the Albedo channel.
		//Generate Layer: Splat0
			//Sample parts of the layer:
				half4 Splat0Albedo_Sample1 = tex2D(_Splat0,vd.uv_Splat0);

Surface = lerp(Surface,half4(Splat0Albedo_Sample1.rgb, 1),vd.Mask0);//8


		//Generate Layer: Splat1
			//Sample parts of the layer:
				half4 Splat1Albedo_Sample1 = tex2D(_Splat1,vd.uv_Splat1);

Surface = lerp(Surface,half4(Splat1Albedo_Sample1.rgb, 1),vd.Mask1);//2


		//Generate Layer: Splat2
			//Sample parts of the layer:
				half4 Splat2Albedo_Sample1 = tex2D(_Splat2,vd.uv_Splat2);

Surface = lerp(Surface,half4(Splat2Albedo_Sample1.rgb, 1),vd.Mask2);//2


		//Generate Layer: Splat3
			//Sample parts of the layer:
				half4 Splat3Albedo_Sample1 = tex2D(_Splat3,vd.uv_Splat3);

Surface = lerp(Surface,half4(Splat3Albedo_Sample1.rgb, 1),vd.Mask3);//2


float3 Lighting;
#if SSUNITY_BRDF_PBS==1
half roughness = 1 - 0;
half3 halfDir = normalize(gi.light.dir + vd.worldViewDir);

half nh = saturate(dot(vd.worldNormal, halfDir));
half nl = saturate(dot(vd.worldNormal, gi.light.dir));
half nv = abs(dot(vd.worldNormal, vd.worldViewDir));
half lh = saturate(dot(gi.light.dir, halfDir));

half nlPow5 = Pow5(1 - nl);
half nvPow5 = Pow5(1 - nv);
#else
Lighting = saturate(dot(vd.worldNormal, gi.light.dir));
#endif
UnityGI o_gi;
ResetUnityGI(o_gi);
o_gi.light = giInput.light;
o_gi.light.color *= giInput.atten;
gi = o_gi;
#if SSUNITY_BRDF_PBS==1
half Fd90 = 0.5 + 2 * lh * lh * roughness;
half disneyDiffuse = (1 + (Fd90 - 1) * nlPow5) * (1 + (Fd90 - 1) * nvPow5);

half diffuseTerm = disneyDiffuse * nl;
Lighting = diffuseTerm;
#endif
//Generate layers for the Diffuse channel.
	//Generate Layer: NdotL
		//Sample parts of the layer:
			half4 NdotLDiffuse_Sample1 = float4(gi.light.ndotl.rrr,0);

Lighting = NdotLDiffuse_Sample1.rgb;//0


		//Generate Layer: Attenuation
			//Sample parts of the layer:
				half4 AttenuationDiffuse_Sample1 = float4(vd.Atten.rrr,0);

Lighting = AttenuationDiffuse_Sample1.rgb;//0


return half4(Lighting,1) * Surface;
}
float Mask_Mask0(VertexData vd) {
	//Set default mask color
		float Mask0 = 0;
		//Generate layers for the C_R channel.
			//Generate Layer: ControlR
				//Sample parts of the layer:
					half4 ControlRMask0_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask0 = ControlRMask0_Sample1.r;//2


	return Mask0;

}
float Mask_Mask1(VertexData vd) {
	//Set default mask color
		float Mask1 = 0;
		//Generate layers for the C_G channel.
			//Generate Layer: ControlG
				//Sample parts of the layer:
					half4 ControlGMask1_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask1 = ControlGMask1_Sample1.g;//2


	return Mask1;

}
float Mask_Mask2(VertexData vd) {
	//Set default mask color
		float Mask2 = 0;
		//Generate layers for the C_B channel.
			//Generate Layer: ControlB
				//Sample parts of the layer:
					half4 ControlBMask2_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask2 = ControlBMask2_Sample1.b;//2


	return Mask2;

}
float Mask_Mask3(VertexData vd) {
	//Set default mask color
		float Mask3 = 0;
		//Generate layers for the C_A channel.
			//Generate Layer: ControlA
				//Sample parts of the layer:
					half4 ControlAMask3_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask3 = ControlAMask3_Sample1.a;//2


	return Mask3;

}
VertexToPixel Vertex(VertexShaderInput v) {
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.uv_Control = TRANSFORM_TEX(v.texcoord, _Control);
	vd.uv_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	vd.uv_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	vd.uv_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	vd.uv_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	#if UNITY_SHOULD_SAMPLE_SH
	vd.sh = 0;
	// SH/ambient and vertex lights
	#ifdef LIGHTMAP_OFF
		vd.sh = 0;
		// Approximated illumination from non-important point lights
		#ifdef VERTEXLIGHT_ON
			vd.sh += Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, vd.worldPos, vd.worldNormal);
		#endif
		vd.sh = ShadeSHPerVertex(vd.worldNormal, vd.sh);
	#endif // LIGHTMAP_OFF
	;
	#endif
		#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		vd.lmap = 0;
	#ifndef DYNAMICLIGHTMAP_OFF
		vd.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif
	#ifndef LIGHTMAP_OFF
		vd.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif
	;
		#endif




		vtp.worldPos = vd.worldPos;
		vtp.position = vd.position;
		vtp.worldNormal = vd.worldNormal;
		vtp.uv_Control = vd.uv_Control;
		vtp.uv_Splat0 = vd.uv_Splat0;
		vtp.uv_Splat1 = vd.uv_Splat1;
		vtp.uv_Splat2 = vd.uv_Splat2;
		vtp.uv_Splat3 = vd.uv_Splat3;
		#if UNITY_SHOULD_SAMPLE_SH
		vtp.sh = vd.sh;
	#endif
		#define pos position
		TRANSFER_SHADOW(vtp);
		UNITY_TRANSFER_FOG(vtp,vtp.pos);
	#undef pos
		#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		vtp.lmap = vd.lmap;
		#endif
		return vtp;
	}
				half4 Pixel(VertexToPixel vtp) : SV_Target {
					half4 outputColor = half4(0,0,0,0);
					half3 outputNormal = half3(0,0,1);
					half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth
					half3 tempDepth = half3(0,0,0);
					VertexData vd;
					UNITY_INITIALIZE_OUTPUT(VertexData,vd);
					vd.worldPos = vtp.worldPos;
					vd.worldNormal = normalize(vtp.worldNormal);
					vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
					vd.uv_Control = vtp.uv_Control;
					vd.uv_Splat0 = vtp.uv_Splat0;
					vd.uv_Splat1 = vtp.uv_Splat1;
					vd.uv_Splat2 = vtp.uv_Splat2;
					vd.uv_Splat3 = vtp.uv_Splat3;
		#if UNITY_SHOULD_SAMPLE_SH
					vd.sh = vtp.sh;
	#endif
		#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
					vd.lmap = vtp.lmap;
		#endif
					outputNormal = vd.worldNormal;
					UnityGI gi;
					UNITY_INITIALIZE_OUTPUT(UnityGI,gi);
					// Setup lighting environment
									#ifndef USING_DIRECTIONAL_LIGHT
										fixed3 lightDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));
									#else
										fixed3 lightDir = _WorldSpaceLightPos0.xyz;
									#endif
										//Uses SHADOW_COORDS
										UNITY_LIGHT_ATTENUATION(atten, vtp, vd.worldPos)
										vd.Atten = atten;
										gi.indirect.diffuse = 0;
										gi.indirect.specular = 0;
										#if !defined(LIGHTMAP_ON)
											gi.light.color = _LightColor0.rgb;
											gi.light.dir = lightDir;
											gi.light.ndotl = LambertTerm(vd.worldNormal, gi.light.dir);
										#endif
											// Call GI (lightmaps/SH/reflections) lighting function
											UnityGIInput giInput;
											UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
											giInput.light = gi.light;
											giInput.worldPos = vd.worldPos;
											giInput.worldViewDir = vd.worldViewDir;
											giInput.atten = atten;
											#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
												giInput.lightmapUV = vd.lmap;
											#else
												giInput.lightmapUV = 0.0;
											#endif
											#if UNITY_SHOULD_SAMPLE_SH
												giInput.ambient = vd.sh;
											#else
												giInput.ambient.rgb = 0.0;
											#endif
											giInput.probeHDR[0] = unity_SpecCube0_HDR;
											giInput.probeHDR[1] = unity_SpecCube1_HDR;
											#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
												giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
											#endif
											#if UNITY_SPECCUBE_BOX_PROJECTION
												giInput.boxMax[0] = unity_SpecCube0_BoxMax;
												giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
												giInput.boxMax[1] = unity_SpecCube1_BoxMax;
												giInput.boxMin[1] = unity_SpecCube1_BoxMin;
												giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
											#endif
							vd.Mask0 = Mask_Mask0(vd);
							vd.Mask1 = Mask_Mask1(vd);
							vd.Mask2 = Mask_Mask2(vd);
							vd.Mask3 = Mask_Mask3(vd);
											half4 outputDiffuse = Diffuse(vd, gi, giInput);
											outputColor = outputDiffuse;//10
															UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
											return outputColor;

										}
									ENDCG
								}
							AlphaToMask Off
								Pass {
									Name "ShadowCaster"
									Tags { "LightMode" = "ShadowCaster" }
								ZTest LEqual
								ZWrite On
								Blend Off//No transparency
								Cull Back//Culling specifies which sides of the models faces to hide.


									CGPROGRAM
											// compile directives
												#pragma vertex Vertex
												#pragma fragment Pixel
												#pragma target 2.5
												#pragma multi_compile_fog
												#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
												#pragma multi_compile_shadowcaster
												#include "HLSLSupport.cginc"
												#include "UnityShaderVariables.cginc"
												#define SHADERSANDWICH_SHADOWCASTER
												#include "UnityCG.cginc"
												#include "Lighting.cginc"
												#include "UnityPBSLighting.cginc"
												#include "AutoLight.cginc"

												#define INTERNAL_DATA
												#define WorldReflectionVector(data,normal) data.worldRefl
												#define WorldNormalVector(data,normal) normal
											//Make our inputs accessible by declaring them here.
												sampler2D _Control;
								float4 _Control_ST;
								float4 _Control_HDR;
												sampler2D _Splat0;
								float4 _Splat0_ST;
								float4 _Splat0_HDR;
												sampler2D _Splat1;
								float4 _Splat1_ST;
								float4 _Splat1_HDR;
												sampler2D _Splat2;
								float4 _Splat2_ST;
								float4 _Splat2_HDR;
												sampler2D _Splat3;
								float4 _Splat3_ST;
								float4 _Splat3_HDR;

								//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones
								inline float3 UnityObjectToWorldNormalNew(in float3 norm) {
									#ifdef UNITY_ASSUME_UNIFORM_SCALING
										return UnityObjectToWorldDir(norm);
									#else
									// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
									return normalize(mul(norm, (float3x3)unity_WorldToObject));
								#endif
							}
							float4 GammaToLinear(float4 col) {
								#if defined(UNITY_COLORSPACE_GAMMA)
								//Best programming evar XD
							#else
								col.rgb = pow(col,2.2);
							#endif
							return col;
						}

						float4 GammaToLinearForce(float4 col) {
							#if defined(UNITY_COLORSPACE_GAMMA)
							//Best programming evar XD
						#else
							col.rgb = pow(col,2.2);
						#endif
						return col;
					}

					float4 LinearToGamma(float4 col) {
						return col;
					}

					float4 LinearToGammaForWeirdSituations(float4 col) {
						#if defined(UNITY_COLORSPACE_GAMMA)
							col.rgb = pow(col,2.2);
						#endif
						return col;
					}

					float3 GammaToLinear(float3 col) {
						#if defined(UNITY_COLORSPACE_GAMMA)
						//Best programming evar XD
					#else
						col = pow(col,2.2);
					#endif
					return col;
				}

				float3 GammaToLinearForce(float3 col) {
					#if defined(UNITY_COLORSPACE_GAMMA)
					//Best programming evar XD
				#else
					col = pow(col,2.2);
				#endif
				return col;
			}

			float3 LinearToGamma(float3 col) {
				return col;
			}

			float GammaToLinear(float col) {
				#if defined(UNITY_COLORSPACE_GAMMA)
				//Best programming evar XD
			#else
				col = pow(col,2.2);
			#endif
			return col;
		}








	struct VertexShaderInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
	};
	struct VertexToPixel {
		float4 position : POSITION;
		float2 uv_Control : TEXCOORD0;
		float2 uv_Splat0 : TEXCOORD1;
		float2 uv_Splat1 : TEXCOORD2;
		float2 uv_Splat2 : TEXCOORD3;
		float2 uv_Splat3 : TEXCOORD4;
		#define pos position
			UNITY_FOG_COORDS(5)
	#undef pos
		#ifdef SHADOWS_CUBE
			float3 vec : TEXCOORD6;
	#endif
		#ifndef SHADOWS_CUBE
			#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				float4 hpos : TEXCOORD7;
			#endif
		#endif
	};

	struct VertexData {
		float3 worldPos;
		float4 position;
		float3 worldNormal;
		float2 uv_Control;
		float2 uv_Splat0;
		float2 uv_Splat1;
		float2 uv_Splat2;
		float2 uv_Splat3;
		#ifdef SHADOWS_CUBE
			float3 vec;
	#endif
		#ifndef SHADOWS_CUBE
			#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				float4 hpos;
			#endif
		#endif
		float Mask0;
		float Mask1;
		float Mask2;
		float Mask3;
		float Atten;
	};
	//OutputPremultiplied: True
	//UseAlphaGenerate: True
	half4 Diffuse(VertexData vd) {
		half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Splat0
				//Sample parts of the layer:
					half4 Splat0Albedo_Sample1 = tex2D(_Splat0,vd.uv_Splat0);

	Surface = lerp(Surface,half4(Splat0Albedo_Sample1.rgb, 1),vd.Mask0);//8


			//Generate Layer: Splat1
				//Sample parts of the layer:
					half4 Splat1Albedo_Sample1 = tex2D(_Splat1,vd.uv_Splat1);

	Surface = lerp(Surface,half4(Splat1Albedo_Sample1.rgb, 1),vd.Mask1);//2


			//Generate Layer: Splat2
				//Sample parts of the layer:
					half4 Splat2Albedo_Sample1 = tex2D(_Splat2,vd.uv_Splat2);

	Surface = lerp(Surface,half4(Splat2Albedo_Sample1.rgb, 1),vd.Mask2);//2


			//Generate Layer: Splat3
				//Sample parts of the layer:
					half4 Splat3Albedo_Sample1 = tex2D(_Splat3,vd.uv_Splat3);

	Surface = lerp(Surface,half4(Splat3Albedo_Sample1.rgb, 1),vd.Mask3);//2


	return Surface;

}
float Mask_Mask0(VertexData vd) {
	//Set default mask color
		float Mask0 = 0;
		//Generate layers for the C_R channel.
			//Generate Layer: ControlR
				//Sample parts of the layer:
					half4 ControlRMask0_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask0 = ControlRMask0_Sample1.r;//2


	return Mask0;

}
float Mask_Mask1(VertexData vd) {
	//Set default mask color
		float Mask1 = 0;
		//Generate layers for the C_G channel.
			//Generate Layer: ControlG
				//Sample parts of the layer:
					half4 ControlGMask1_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask1 = ControlGMask1_Sample1.g;//2


	return Mask1;

}
float Mask_Mask2(VertexData vd) {
	//Set default mask color
		float Mask2 = 0;
		//Generate layers for the C_B channel.
			//Generate Layer: ControlB
				//Sample parts of the layer:
					half4 ControlBMask2_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask2 = ControlBMask2_Sample1.b;//2


	return Mask2;

}
float Mask_Mask3(VertexData vd) {
	//Set default mask color
		float Mask3 = 0;
		//Generate layers for the C_A channel.
			//Generate Layer: ControlA
				//Sample parts of the layer:
					half4 ControlAMask3_Sample1 = tex2D(_Control,vd.uv_Control);

	Mask3 = ControlAMask3_Sample1.a;//2


	return Mask3;

}
VertexToPixel Vertex(VertexShaderInput v) {
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.uv_Control = TRANSFORM_TEX(v.texcoord, _Control);
	vd.uv_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	vd.uv_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	vd.uv_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	vd.uv_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);




	vtp.position = vd.position;
	vtp.uv_Control = vd.uv_Control;
	vtp.uv_Splat0 = vd.uv_Splat0;
	vtp.uv_Splat1 = vd.uv_Splat1;
	vtp.uv_Splat2 = vd.uv_Splat2;
	vtp.uv_Splat3 = vd.uv_Splat3;
	#define pos position
	UNITY_TRANSFER_FOG(vtp,vtp.pos);
#undef pos
	#ifdef SHADOWS_CUBE
	vtp.vec = vd.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	vtp.hpos = vd.hpos;
		#endif
	#endif
	return vtp;
}
			half4 Pixel(VertexToPixel vtp) : SV_Target {
				half4 outputColor = half4(0,0,0,0);
				half3 outputNormal = half3(0,0,1);
				half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth
				half3 tempDepth = half3(0,0,0);
				VertexData vd;
				UNITY_INITIALIZE_OUTPUT(VertexData,vd);
				vd.uv_Control = vtp.uv_Control;
				vd.uv_Splat0 = vtp.uv_Splat0;
				vd.uv_Splat1 = vtp.uv_Splat1;
				vd.uv_Splat2 = vtp.uv_Splat2;
				vd.uv_Splat3 = vtp.uv_Splat3;
	#ifdef SHADOWS_CUBE
				vd.vec = vtp.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				vd.hpos = vtp.hpos;
		#endif
	#endif
vd.Mask0 = Mask_Mask0(vd);
vd.Mask1 = Mask_Mask1(vd);
vd.Mask2 = Mask_Mask2(vd);
vd.Mask3 = Mask_Mask3(vd);
				half4 outputDiffuse = Diffuse(vd);
				outputColor = outputDiffuse;//10
								UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
				SHADOW_CASTER_FRAGMENT(vd)
				return outputColor;

			}
		ENDCG
		}
	}
	Dependency "AddPassShader" = "Terrain/UnlitTerrainAddPass"
	Fallback "Legacy Shaders/Diffuse"
}


/*
Shader Sandwich Shader
	File Format Version(Float): 3.0
	Begin Shader Base

		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Control"
			ImageDefault(Float): 0
			Image(Text): ""
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "TerrainControl"
			CustomFallback(Text): "_Control"
		End Shader Input


		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Splat0"
			ImageDefault(Float): 0
			Image(Text): ""
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "TerrainSplat0"
			CustomFallback(Text): "_Splat0"
		End Shader Input


		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Splat1"
			ImageDefault(Float): 0
			Image(Text): ""
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "TerrainSplat1"
			CustomFallback(Text): "_Splat1"
		End Shader Input


		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Splat2"
			ImageDefault(Float): 0
			Image(Text): ""
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "TerrainSplat2"
			CustomFallback(Text): "_Splat2"
		End Shader Input


		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Splat3"
			ImageDefault(Float): 0
			Image(Text): ""
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "TerrainSplat3"
			CustomFallback(Text): "_Splat3"
		End Shader Input

		ShaderName(Text): "Terrain/UnlitTerrainFirstPass"
		Tech Lod(Float): 200
		Fallback(Type): Diffuse - {TypeID = 0}
		CustomFallback(Text): "\qLegacy Shaders/Diffuse\q"
		Queue(Type): Custom - {TypeID = 6}
		Custom Queue(Float): -100
		QueueAuto(Toggle): True
		Replacement(Type): Opaque - {TypeID = 2}
		ReplacementAuto(Toggle): True
		Tech Shader Target(Float): 3
		Exclude DX9(Toggle): False

		Begin Masks

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask0"
				LayerListName(Text): "C_R"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "ControlR"
					Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Texture(Texture):  - {Input = 0}
					Color(Vec): 0.5,0.8823529,1,1
					Number(Float): 0.5
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask1"
				LayerListName(Text): "C_G"
				Is Mask(Toggle): True
				EndTag(Text): "g"

				Begin Shader Layer
					Layer Name(Text): "ControlG"
					Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Texture(Texture):  - {Input = 0}
					Color(Vec): 0.5,0.8823529,1,1
					Number(Float): 0.5
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask2"
				LayerListName(Text): "C_B"
				Is Mask(Toggle): True
				EndTag(Text): "b"

				Begin Shader Layer
					Layer Name(Text): "ControlB"
					Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Texture(Texture):  - {Input = 0}
					Color(Vec): 0.5,0.8823529,1,1
					Number(Float): 0.5
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask3"
				LayerListName(Text): "C_A"
				Is Mask(Toggle): True
				EndTag(Text): "a"

				Begin Shader Layer
					Layer Name(Text): "ControlA"
					Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Texture(Texture):  - {Input = 0}
					Color(Vec): 0.5,0.8823529,1,1
					Number(Float): 0.5
				End Shader Layer

			End Shader Layer List

		End Masks

		Begin Shader Pass
			Name(Text): "FirstPass"
			Visible(Toggle): True

			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceDiffuse"
				User Name(Text): "Diffuse"
				Use Custom Lighting(Toggle): True
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend - {TypeID = 0}
				Mix Amount(Float): 1
				Mix Type(Type): Mix - {TypeID = 0}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): True
				Lighting Type(Type): Unity Standard - {TypeID = 1}
				Roughness Or Smoothness(Type): Smoothness - {TypeID = 0}
				Smoothness(Float): 0
				Roughness(Float): 0.7
				Light Size(Float): 0
				Wrap Amount(Float): 0.7
				Wrap Color(Vec): 0.4,0.2,0.2,1
				PBR Quality(Type): Auto - {TypeID = 0}
				Disable Normals(Float): 0
				Use Ambient(Toggle): False
				Use Tangents(Toggle): False

				Begin Shader Layer List

					LayerListUniqueName(Text): "Albedo"
					LayerListName(Text): "Albedo"
					Is Mask(Toggle): False
					EndTag(Text): "rgba"

					Begin Shader Layer
						Layer Name(Text): "Splat0"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): C_R - {ObjectID = 0}
						Texture(Texture):  - {Input = 1}
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Splat1"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): C_G - {ObjectID = 1}
						Texture(Texture):  - {Input = 2}
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Splat2"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): C_B - {ObjectID = 2}
						Texture(Texture):  - {Input = 3}
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Splat3"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): C_A - {ObjectID = 3}
						Texture(Texture):  - {Input = 4}
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

				End Shader Layer List

				Begin Shader Layer List

					LayerListUniqueName(Text): "Diffuse"
					LayerListName(Text): "Diffuse"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "NdotL"
						Layer Type(ObjectArray): SLTData - {ObjectID = 23}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -2}
						Data(Type): Light/NdotL - {TypeID = 7}
						Color(Vec): 1,1,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Attenuation"
						Layer Type(ObjectArray): SLTData - {ObjectID = 23}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -2}
						Data(Type): Light/Attenuation - {TypeID = 1}
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient

			Geometry Ingredients

			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierMisc"
				User Name(Text): "Misc Settings"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				ZWriteMode(Type): Auto - {TypeID = 0}
				ZTestMode(Type): Auto - {TypeID = 0}
				CullMode(Type): Back Faces - {TypeID = 0}
				ShaderModel(Type): Auto - {TypeID = 0}
				Use Fog(Toggle): True
				Use Lightmaps(Toggle): True
				Use Forward Full Shadows(Toggle): True
				Use Forward Vertex Lights(Toggle): False
				Use Shadows(Toggle): True
				Generate Forward Base Pass(Toggle): True
				Generate Forward Add Pass(Toggle): False
				Generate Deferred Pass(Toggle): False
				Generate Shadow Caster Pass(Toggle): True

			End Shader Ingredient

		End Shader Pass

	End Shader Base
End Shader Sandwich Shader
*/
