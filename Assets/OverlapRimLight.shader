// NOTE: 实现了类似 FarCry 5 中伙伴身上的 Rim light 光效果
// 当物体直接可视时，正常渲染；物体被遮挡时，只渲染 Rim Light
// 实现思路：额外添加一个 Pass，ZTest 设为 Greater，并且做 Blend

Shader "MyShaders/Overlap Rim Light Shader"{

	Properties{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Main Tex", 2D) = "white"{}
		// 缺省情况下的 “bump” 会使用模型本身的法线方向，不进行扰动
		// 对应没有 normal map 情况下的普通纹理
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1.0
		_Specular("Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
		_RimColor("Rim Light Color", Color) = (1,1,1,1)
		_ReflectionRate("Reflection Rate", Range(0,20)) = 0.3
		_ReflectionOn("Reflection on(0 or 1)", Int) = 0
		_RefractionRate("Refraction Rate", Range(0,50)) = 0.3
		_RefractionOn("Refraction on(0 or 1)", Int) = 0
	}

	SubShader{
		Tags{"RenderType" = "Opaque" "Queue" = "Geometry"}

		Pass{
			Tags{"LightMode" = "ForwardBase"}

			CGPROGRAM

			// forward bass pass 所必须声明的宏
			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			fixed4 _Specular;
			float _Gloss;
			fixed _ReflectionRate;
			fixed _RefractionRate;
			fixed4 _RimColor;
			half _ReflectionOn;
			half _RefractionOn;


			/* 
			* 因为阴影计算的内置函数使用了上下文变量，
			* 所以这里的变量命名要符合固定规范：
			* a2v 结构体的顶点坐标变量名必须是 vertex，
			* 顶点着色器中输入的 a2v 变量必须叫 v,
			* v2f 结构体的顶点位置变量名必须是 pos，
			*/
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 TtoW0 : TEXCOORD1;  
                float4 TtoW1 : TEXCOORD2;  
                float4 TtoW2 : TEXCOORD3; 
                // 这个函数的参数是下一个可用的插值寄存器的索引值
				// 即如果前面最多用到了 TEXCOORDx，此处参数应为
				// TEXCOORD(x+1)
				SHADOW_COORDS(4)
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBitangent = cross(worldNormal, worldTangent)*v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBitangent.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldPos.z);

				// 调用内置函数进行阴影映射，映射完的值装入
				// v2f SHADOW_COORDS
				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				// Normal 的采样、 unpack、 缩放、重新计算 z 值
				fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				// 把法线坐标从切线空间转换到世界空间
				fixed3 worldNormal = normalize(fixed3(dot(i.TtoW0.xyz, tangentNormal),
					dot(i.TtoW1.xyz, tangentNormal), dot(i.TtoW2.xyz, tangentNormal)));

				fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, lightDir));

				fixed3 halfDir = normalize(lightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, halfDir)), _Gloss);

				// 利用内置函数统一计算光线衰减和阴影采样
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

				return fixed4(ambient + (diffuse + specular ) * atten, 1.0);
			}

			ENDCG
		}

		Pass{
			// Add Pass 除了不计算 ambient 之外和 bass pass 相同
			Tags{"LightMode" = "ForwardAdd"}

			// 每次渲染 Additional Pass 都把渲染结果与之前的混合
			Blend One One

			CGPROGRAM

			// forward add pass 所必须声明的宏
//			#pragma multi_compile_fwdadd

			// 或者换成下面这个，就可以让本物体接受其他物体
			// 在非 directional light 下的投影
			//（或者严格地说，在 add pass 中）
			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"		

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			fixed4 _Specular;
			float _Gloss;	

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 TtoW0 : TEXCOORD1;  
                float4 TtoW1 : TEXCOORD2;  
                float4 TtoW2 : TEXCOORD3; 
				SHADOW_COORDS(4)
			};		

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBitangent = cross(worldNormal, worldTangent)*v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBitangent.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

				fixed3 worldNormal = normalize(fixed3(dot(i.TtoW0.xyz, tangentNormal),
					dot(i.TtoW1.xyz, tangentNormal), dot(i.TtoW2.xyz, tangentNormal)));

				fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

				fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, lightDir));

				fixed3 halfDir = normalize(lightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, halfDir)), _Gloss);

				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

				// 在 add pass 中不再计算 ambient
				return fixed4((diffuse + specular) * atten, 1.0);
			}

			ENDCG
		}

		
		Pass{
			Tags{"LightMode" = "ForwardBase"}
			ZTest Greater
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			// forward bass pass 所必须声明的宏
			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			fixed _ReflectionRate;
			fixed _RefractionRate;
			fixed4 _RimColor;
			half _ReflectionOn;
			half _RefractionOn;


			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 TtoW0 : TEXCOORD1;  
                float4 TtoW1 : TEXCOORD2;  
                float4 TtoW2 : TEXCOORD3; 
				SHADOW_COORDS(4)
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBitangent = cross(worldNormal, worldTangent)*v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBitangent.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldPos.z);

				// 调用内置函数进行阴影映射，映射完的值装入
				// v2f SHADOW_COORDS
				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				// Normal 的采样、 unpack、 缩放、重新计算 z 值
				fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				// 把法线坐标从切线空间转换到世界空间
				fixed3 worldNormal = normalize(fixed3(dot(i.TtoW0.xyz, tangentNormal),
					dot(i.TtoW1.xyz, tangentNormal), dot(i.TtoW2.xyz, tangentNormal)));

				// 计算 rim light
				float adotNV = abs(dot(worldNormal, viewDir));
				float Fr = pow(1-adotNV, _ReflectionRate);
				float Ft = pow(adotNV, _RefractionRate);

				fixed3 rim = saturate(_RimColor.rgb * (Fr*_ReflectionOn + Ft * _RefractionOn));

				return fixed4(rim, 0.5);
			}

			ENDCG
		}
	}
	// 通过回调函数递归查找 shadow caster pass 来向其他物体投射阴影
	FallBack "Specular"
}