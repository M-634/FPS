// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASECutom/Dissolve"
{
	Properties
	{
		_Main("Main", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_MetallicMap("MetallicMap", 2D) = "white" {}
		_AOMap("AOMap", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_OpacityMask("OpacityMask", 2D) = "white" {}
		_DissolveAmount("DissolveAmount", Range( 0 , 1)) = 1.76
		_DissolveEgeAmount("DissolveEgeAmount", Range( 0 , 1)) = 1.76
		[HDR]_DissolveEgeColor("DissolveEgeColor", Color) = (0,0.9604411,1,0)
		_NoiseScale("NoiseScale", Range( 0 , 1)) = 0.24
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Main;
		uniform float4 _Main_ST;
		uniform float4 _DissolveEgeColor;
		uniform sampler2D _OpacityMask;
		uniform float _NoiseScale;
		uniform float _DissolveAmount;
		uniform float _DissolveEgeAmount;
		uniform sampler2D _MetallicMap;
		uniform float4 _MetallicMap_ST;
		uniform sampler2D _AOMap;
		uniform float4 _AOMap_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_Main = i.uv_texcoord * _Main_ST.xy + _Main_ST.zw;
			o.Albedo = tex2D( _Main, uv_Main ).rgb;
			float2 uv_TexCoord38 = i.uv_texcoord * ( (0.5 + (_NoiseScale - 0.0) * (3.0 - 0.5) / (1.0 - 0.0)) * float2( 1,1 ) );
			float4 temp_output_28_0 = ( tex2D( _OpacityMask, uv_TexCoord38 ) + (-0.3 + (( 1.0 - _DissolveAmount ) - 0.0) * (1.0 - -0.3) / (1.0 - 0.0)) );
			float4 temp_cast_1 = (_DissolveEgeAmount).xxxx;
			float4 temp_output_30_0 = step( temp_output_28_0 , temp_cast_1 );
			o.Emission = ( _DissolveEgeColor * temp_output_30_0 ).rgb;
			float2 uv_MetallicMap = i.uv_texcoord * _MetallicMap_ST.xy + _MetallicMap_ST.zw;
			o.Metallic = tex2D( _MetallicMap, uv_MetallicMap ).r;
			float2 uv_AOMap = i.uv_texcoord * _AOMap_ST.xy + _AOMap_ST.zw;
			o.Occlusion = tex2D( _AOMap, uv_AOMap ).r;
			o.Alpha = 1;
			clip( temp_output_28_0.r - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
-5.599976;52.8;1920;953;862.7043;525.9217;1.406276;True;True
Node;AmplifyShaderEditor.RangedFloatNode;40;-652.8435,-131.5824;Inherit;False;Property;_NoiseScale;NoiseScale;9;0;Create;True;0;0;0;False;0;False;0.24;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;39;-242.046,54.06371;Inherit;False;Constant;_Vector0;Vector 0;9;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;32;54.83575,221.3194;Inherit;False;1086.202;538.8029;DissolveOpacity;5;28;1;3;6;2;;0.759434,0.9485079,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;42;-282.459,-168.7054;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;70.99058,496.7334;Inherit;False;Property;_DissolveAmount;DissolveAmount;6;0;Create;True;0;0;0;False;0;False;1.76;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-64.13957,-62.55996;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;163.782,-71.21611;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;3;397.9516,502.2744;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;6;575.7876,506.123;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.3;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;33;504.8841,-328.8196;Inherit;False;1115.151;508.6321;DissoveEge;5;31;27;24;26;30;;0.6462264,1,0.6562585,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;542.6453,274.3195;Inherit;True;Property;_OpacityMask;OpacityMask;5;0;Create;True;0;0;0;False;0;False;-1;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;28;906.0361,398.4538;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;26;518.1096,-65.31005;Inherit;False;Property;_DissolveEgeAmount;DissolveEgeAmount;7;0;Create;True;0;0;0;False;0;False;1.76;0.505;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;11;693.9857,-874.9995;Inherit;False;721.4446;511.3954;Base;4;8;10;7;9;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;24;1090.973,-278.8196;Inherit;False;Property;_DissolveEgeColor;DissolveEgeColor;8;1;[HDR];Create;True;0;0;0;False;0;False;0,0.9604411,1,0;0,21.36125,21.36125,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;30;835.5971,-92.35019;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;9;1095.432,-595.8821;Inherit;True;Property;_NormalMap;NormalMap;1;0;Create;True;0;0;0;False;0;False;-1;None;a5798ba7ff8d7984c9b50981d6dd8e6b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;761.5623,-827.9984;Inherit;True;Property;_MetallicMap;MetallicMap;2;0;Create;True;0;0;0;False;0;False;-1;None;7e4a41389ffe24f458e5a1b4b6c554eb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;1094.821,-812.4802;Inherit;True;Property;_Main;Main;0;0;Create;True;0;0;0;False;0;False;-1;None;0ca93b28d8c504946904487d0c1d581e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;743.9857,-593.6041;Inherit;True;Property;_AOMap;AOMap;3;0;Create;True;0;0;0;False;0;False;-1;None;e2d3ffb9e0c5b7a4d95d18b01f2c27e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;1405.03,-110.0028;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;1094.452,-47.07512;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1841.191,-189.7282;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ASECutom/Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;40;0
WireConnection;41;0;42;0
WireConnection;41;1;39;0
WireConnection;38;0;41;0
WireConnection;3;0;2;0
WireConnection;6;0;3;0
WireConnection;1;1;38;0
WireConnection;28;0;1;0
WireConnection;28;1;6;0
WireConnection;30;0;28;0
WireConnection;30;1;26;0
WireConnection;27;0;24;0
WireConnection;27;1;30;0
WireConnection;31;0;30;0
WireConnection;31;1;28;0
WireConnection;0;0;8;0
WireConnection;0;1;9;0
WireConnection;0;2;27;0
WireConnection;0;3;10;0
WireConnection;0;5;7;0
WireConnection;0;10;28;0
ASEEND*/
//CHKSM=E31F1EC8C3B7F32674A0689BE662411262C646AC