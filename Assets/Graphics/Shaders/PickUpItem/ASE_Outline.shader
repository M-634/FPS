// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASECutom/Outline"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Main("Main", 2D) = "white" {}
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		_MetallicAmount("MetallicAmount", Range( 0 , 1)) = 1
		_Metallic("Metallic", 2D) = "white" {}
		_Sommthness("Sommthness", Range( 0 , 1)) = 0.5
		_OutlineColor("OutlineColor", Color) = (1,1,1,0)
		_OutlineWidth("OutlineWidth", Float) = 0
		_OutlineOpecity("OutlineOpecity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input
		{
			half filler;
		};
		uniform float4 _OutlineColor;
		uniform float _OutlineOpecity;
		uniform float _Cutoff = 0.5;
		uniform float _OutlineWidth;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineWidth;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = (_OutlineColor).rgba.rgb;
			clip( _OutlineOpecity - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Main;
		uniform float4 _Main_ST;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _MetallicAmount;
		uniform float _Sommthness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_Main = i.uv_texcoord * _Main_ST.xy + _Main_ST.zw;
			o.Albedo = tex2D( _Main, uv_Main ).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Metallic = ( tex2D( _Metallic, uv_Metallic ).r * _MetallicAmount );
			o.Smoothness = _Sommthness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
331;160;1920;852;864.7048;576.4802;1.044372;True;False
Node;AmplifyShaderEditor.ColorNode;14;-679.5566,-29.94592;Inherit;False;Property;_OutlineColor;OutlineColor;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;15;-427.0935,-87.76086;Inherit;True;True;True;True;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-435.3127,129.2066;Inherit;False;Property;_OutlineOpecity;OutlineOpecity;7;0;Create;True;0;0;0;False;0;False;0;0.67;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-447.2843,240.7501;Inherit;False;Property;_OutlineWidth;OutlineWidth;6;0;Create;True;0;0;0;False;0;False;0;0.013;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-215.6914,-327.7437;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;-1;None;e22ba5f1aab63d7409ed0a8b8e8f6fe4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-154.313,-126.7934;Inherit;False;Property;_MetallicAmount;MetallicAmount;2;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-229.6516,-754.6199;Inherit;True;Property;_Main;Main;0;0;Create;True;0;0;0;False;0;False;-1;None;3c12e0dfd453eb445b74f5111df4c7ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-203.0347,-532.6117;Inherit;True;Property;_NormalMap;NormalMap;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;b1580d30ea0592340aee7888459cdda5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;230.687,-297.7934;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-25.31299,-51.7934;Inherit;False;Property;_Sommthness;Sommthness;4;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;12;-131.2904,68.47723;Inherit;False;0;True;Masked;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;456,-356;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ASECutom/Outline;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;14;0
WireConnection;20;0;8;1
WireConnection;20;1;19;0
WireConnection;12;0;15;0
WireConnection;12;2;17;0
WireConnection;12;1;10;0
WireConnection;0;0;1;0
WireConnection;0;1;7;0
WireConnection;0;3;20;0
WireConnection;0;4;18;0
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=4A99F8D96CB5A3D3332E582E0381C96F995EEDB3