Shader "Terrain/FlatTerrainAddPass"
{
    Properties
    {
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}

		_Color ("Color", Color) = (1, 1, 1, 1)
		_Shininess("Shininess", Float) = 10
		_ShininessMultiplier("ShininessAmount", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags 
		{ 
			"SplatCount" = "4"
			"Queue" = "Geometry-99"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
        LOD 200
		Blend One One

		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			uniform float _Shininess;
			uniform float _ShininessMultiplier;
			uniform float4 _Color;

			struct v2g
			{
				float4 pos : SV_POSITION;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
				float3 vertex : TEXCOORD1;
				float3 vertexLighting : TEXCOORD2;
				half fogDepth: TEXCOORD3;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float3 norm : NORMAL;
				float2 uv : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 vertexLighting : TEXCOORD2;
				LIGHTING_COORDS(3, 4)
				half fogDepth: TEXCOORD5;
			};

			v2g vert(appdata_full v)
			{
				v2g o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.norm = v.normal;
				o.uv = v.texcoord;
				o.vertex = v.vertex;

				float3 vertexLighting = float3(0, 0, 0);
				#ifdef VERTEXLIGHT_ON
					for (int index = 0; index < 4; index++)
					{
						float3 normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
						float3 lightPosition = float3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]);
						float3 vertexToLightSource = lightPosition - mul(unity_ObjectToWorld, v.vertex);
						float3 lightDir = normalize(vertexToLightSource);
						float distanceSquared = dot(vertexToLightSource, vertexToLightSource);
						float attenuation = 1.0 / (1.0 + unity_4LightAtten0[index] * distanceSquared);

						vertexLighting += attenuation * unity_LightColor[index].rgb * _Color.rgb * saturate(dot(normalDir, lightDir));
					}
				#endif
				o.vertexLighting = vertexLighting;

				o.fogDepth = length(UnityObjectToClipPos(v.vertex));

				#if defined(FOG_LINEAR)
					o.fogDepth = clamp(o.fogDepth * unity_FogParams.z + unity_FogParams.w, 0.0, 1.0);
				#elif defined(FOG_EXP)
					o.fogDepth = exp2(-(o.fogDepth * unity_FogParams.y));
				#elif defined(FOG_EXP2)
					o.fogDepth = exp2(-(o.fogDepth * unity_FogParams.y)*(o.fogDepth * unity_FogParams.y));
				#else
					o.fogDepth = 1.0;
				#endif

				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{
				float3 v0 = IN[0].pos.xyz;
				float3 v1 = IN[1].pos.xyz;
				float3 v2 = IN[2].pos.xyz;

				g2f o;
				o.norm = normalize(IN[0].norm + IN[1].norm + IN[2].norm);
				o.uv = (IN[0].uv + IN[1].uv + IN[2].uv) / 3;
				o.vertexLighting = (IN[0].vertexLighting + IN[1].vertexLighting + IN[2].vertexLighting) / 3;
				o.posWorld = mul(unity_ObjectToWorld, (IN[0].vertex + IN[1].vertex + IN[2].vertex) / 3);

				o.pos = IN[0].pos;
				o.fogDepth = IN[0].fogDepth;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);

				o.pos = IN[1].pos;
				o.fogDepth = IN[1].fogDepth;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);

				o.pos = IN[2].pos;
				o.fogDepth = IN[2].fogDepth;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);
			}

			half4 frag(g2f IN) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.posWorld.xyz);
				float3 normalDir = normalize(mul(float4(IN.norm, 0.0), unity_WorldToObject).xyz);
				float3 vertexToLight = _WorldSpaceLightPos0.w == 0 ? _WorldSpaceLightPos0.xyz : _WorldSpaceLightPos0.xyz - IN.posWorld.xyz;
				float3 lightDir = normalize(vertexToLight);

				//Ambient
				float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb;
				UNITY_LIGHT_ATTENUATION(atten, IN, IN.posWorld);
				float3 diffuseReflection = atten * _LightColor0.rgb * saturate(dot(normalDir, lightDir));

				//Specular
				float3 specularReflection = float3(0.0, 0.0, 0.0);
				if (dot(normalDir, lightDir) >= 0.0)
				{
					specularReflection = atten * _LightColor0.rgb * pow(max(0.0, dot(reflect(-lightDir, normalDir), viewDir)), _Shininess);
				}

				//Terrain Textures
				fixed4 splat_control = tex2D(_Control, IN.uv);
				fixed3 col;
				col = splat_control.r * tex2D(_Splat0, IN.uv).rgb;
				col += splat_control.g * tex2D(_Splat1, IN.uv).rgb;
				col += splat_control.b * tex2D(_Splat2, IN.uv).rgb;
				col += splat_control.a * tex2D(_Splat3, IN.uv).rgb;

				//Color
				float4 colorTex = float4((IN.vertexLighting + ambientLight + diffuseReflection + (specularReflection * _ShininessMultiplier)) * col * _Color, 1);

				//Color w fog
				return lerp(unity_FogColor, colorTex, IN.fogDepth);
			}

			ENDCG
		}

		/*
		Pass
		{
			Tags {"LightMode" = "ForwardAdd"}
			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile_fwdadd_fullshadows

			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			uniform float _Shininess;

			struct v2g
			{
				float3 norm : NORMAL;
				float3 vertex : TEXCOORD0;
				float3 uv : TEXCOORD1;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float3 norm : NORMAL;
				float4 posWorld : TEXCOORD0;
				float3 uv : TEXCOORD1;
				LIGHTING_COORDS(3, 4)
			};

			// hack because TRANSFER_VERTEX_TO_FRAGMENT has harcoded requirement for 'v.vertex'
			struct unityTransferVertexToFragmentSucksHack
			{
				float4 vertex : SV_POSITION;
			};

			appdata_full vert(appdata_full v)
			{
				appdata_full o;
				o = v;
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle appdata_full IN[3], inout TriangleStream<g2f> triStream)
			{
				g2f o;
				o.norm = normalize((IN[0].normal + IN[1].normal + IN[2].normal) / 3);
				o.uv = (IN[0].texcoord + IN[1].texcoord + IN[2].texcoord) / 3;

				unityTransferVertexToFragmentSucksHack v;

				v.vertex = IN[0].vertex;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);

				v.vertex = IN[1].vertex;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);

				v.vertex = IN[2].vertex;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				triStream.Append(o);
			}

			half4 frag(g2f IN) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.posWorld.xyz);
				float3 normalDir = normalize(mul(float4(IN.norm, 0.0), unity_WorldToObject).xyz);
				float3 vertexToLight = _WorldSpaceLightPos0.w == 0 ? _WorldSpaceLightPos0.xyz : _WorldSpaceLightPos0.xyz - IN.posWorld.xyz;
				float3 lightDir = normalize(vertexToLight);

				UNITY_LIGHT_ATTENUATION(atten, IN, IN.posWorld.xyz);

				float3 specularReflection = float3(0.0, 0.0, 0.0);
				if (dot(normalDir, lightDir) >= 0.0)
				{
					specularReflection = atten * _LightColor0.rgb * pow(max(0.0, dot(reflect(-lightDir, normalDir), viewDir)), _Shininess);
				}

				//Terrain Textures
				fixed4 splat_control = tex2D(_Control, IN.uv);
				fixed3 col;
				col = splat_control.r * tex2D(_Splat0, IN.uv).rgb;
				col += splat_control.g * tex2D(_Splat1, IN.uv).rgb;
				col += splat_control.b * tex2D(_Splat2, IN.uv).rgb;
				col += splat_control.a * tex2D(_Splat3, IN.uv).rgb;

				//Color
				float3 diffuseReflection = atten * _LightColor0.rgb * max(0.0, dot(normalDir, lightDir));
				return float4((diffuseReflection + specularReflection) * col, 0);
			}

			ENDCG
		}
		*/
    }
    FallBack "Diffuse"
}