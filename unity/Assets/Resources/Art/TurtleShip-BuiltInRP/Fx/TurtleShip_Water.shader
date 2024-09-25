// Made with Amplify Shader Editor v1.9.1.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tripolygon/TurtleShip_Water"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0,0.6284904,1,0)
		_DeepColor("DeepColor", Color) = (0,0.2071772,0.3301887,0)
		_WaterDepth("WaterDepth", Float) = 0
		_WaterDepth_Range("WaterDepth_Range", Float) = 0
		_WaterDepth_Contrast("WaterDepth_Contrast", Float) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.2
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.9
		[Normal]_WaterNormal("WaterNormal", 2D) = "bump" {}
		_WaveTile("WaveTile", Float) = 3
		_WaveSpeed("WaveSpeed", Float) = 3
		_FoamRange("FoamRange", Float) = 0
		_FoamDistance("FoamDistance", Float) = 0
		_FoamContrast("FoamContrast", Float) = 0
		_FoamDistortion("FoamDistortion", Float) = 1
		_FoamColor("FoamColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#pragma surface surf StandardSpecular alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _WaterNormal;
		uniform float _WaveTile;
		uniform float _WaveSpeed;
		uniform float4 _FoamColor;
		uniform float4 _MainColor;
		uniform float4 _DeepColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _WaterDepth;
		uniform float _WaterDepth_Range;
		uniform float _WaterDepth_Contrast;
		uniform float _FoamDistance;
		uniform float _FoamRange;
		uniform float _FoamDistortion;
		uniform float _FoamContrast;
		uniform float _Smoothness;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 temp_cast_0 = (_WaveTile).xx;
			float2 uv_TexCoord34 = i.uv_texcoord * temp_cast_0;
			float mulTime36 = _Time.y * _WaveSpeed;
			float3 temp_output_33_0 = BlendNormals( tex2D( _WaterNormal, ( ( uv_TexCoord34 * float2( 0.8,0.7 ) ) + ( mulTime36 * float2( 1,0.9 ) ) ) ).rgb , tex2D( _WaterNormal, ( uv_TexCoord34 + ( mulTime36 * float2( -0.7,-0.8 ) ) ) ).rgb );
			o.Normal = temp_output_33_0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth22 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth22 = abs( ( screenDepth22 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterDepth ) );
			float saferPower85 = abs( ( distanceDepth22 + _WaterDepth_Range ) );
			float temp_output_88_0 = saturate( pow( saferPower85 , _WaterDepth_Contrast ) );
			float4 lerpResult21 = lerp( _MainColor , _DeepColor , temp_output_88_0);
			float screenDepth28 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth28 = abs( ( screenDepth28 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _FoamDistance ) );
			float saferPower61 = abs( ( ( distanceDepth28 + _FoamRange ) + ( (temp_output_33_0).x * _FoamDistortion ) ) );
			float temp_output_62_0 = saturate( pow( saferPower61 , _FoamContrast ) );
			float4 lerpResult66 = lerp( _FoamColor , lerpResult21 , temp_output_62_0);
			o.Albedo = lerpResult66.rgb;
			float lerpResult83 = lerp( 0.0 , 0.1 , temp_output_62_0);
			float3 temp_cast_6 = (lerpResult83).xxx;
			o.Specular = temp_cast_6;
			float lerpResult81 = lerp( 0.0 , _Smoothness , temp_output_62_0);
			o.Smoothness = lerpResult81;
			float temp_output_79_0 = ( _Opacity + ( 1.0 - temp_output_62_0 ) );
			o.Alpha = saturate( temp_output_79_0 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19102
Node;AmplifyShaderEditor.SaturateNode;62;-431.257,454.3237;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;28;-1372.061,410.5125;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1321.068,560.3432;Inherit;False;Property;_FoamRange;FoamRange;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1051.112,450.7901;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-747.9947,540.4614;Inherit;False;Property;_FoamContrast;FoamContrast;12;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;66;-6.02713,-6.38883;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-123.0722,230.4899;Inherit;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;78;-134.473,471.2386;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-2937.916,939.3083;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-3140.277,960.8574;Inherit;False;Property;_WaveTile;WaveTile;8;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2663.277,917.8574;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.8,0.7;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-2501.277,925.8574;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-2492.277,1049.857;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2675.277,1204.857;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2680.277,1100.857;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;1.1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;36;-2958.277,1075.857;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-3163.277,1075.857;Inherit;False;Property;_WaveSpeed;WaveSpeed;9;0;Create;True;0;0;0;False;0;False;3;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;43;-2889.277,1162.857;Inherit;False;Constant;_Wave1_Dir;Wave1_Dir;8;0;Create;True;0;0;0;False;0;False;1,0.9;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;44;-2886.277,1285.857;Inherit;False;Constant;_Wave2_Dir;Wave2_Dir;8;0;Create;True;0;0;0;False;0;False;-0.7,-0.8;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.BlendNormalsNode;33;-1772.632,674.2119;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;75;-1449.245,781.828;Inherit;False;True;False;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1228.245,822.828;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1795.57,503.9306;Inherit;False;Property;_FoamDistance;FoamDistance;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-910.5573,577.6642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;68;-273.5091,-234.3427;Inherit;False;Property;_FoamColor;FoamColor;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;83;559.7,267.9586;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;487.4083,496.7896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;262.4782,485.9716;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;735.0922,226.546;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;314.7682,141.0267;Inherit;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;0;False;0;False;0.9;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;315.7794,280.1964;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;0.1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;17;-1190.145,-343.0535;Inherit;False;Property;_MainColor;MainColor;0;0;Create;True;0;0;0;False;0;False;0,0.6284904,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;20;-1174.145,-167.0534;Inherit;False;Property;_DeepColor;DeepColor;1;0;Create;True;0;0;0;False;0;False;0,0.2071772,0.3301887,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;-464.4613,-116.2152;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;22;-1245.391,83.10035;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;84;-918.9893,41.49823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1469.391,99.10035;Inherit;False;Property;_WaterDepth;WaterDepth;2;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1190.858,227.5901;Inherit;False;Property;_WaterDepth_Range;WaterDepth_Range;3;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-945.1202,251.2286;Inherit;False;Property;_WaterDepth_Contrast;WaterDepth_Contrast;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;88;-506.2305,75.48065;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-2416.371,675.2821;Inherit;True;Property;_WaterNormal;WaterNormal;7;1;[Normal];Create;True;0;0;0;False;0;False;None;None;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;91;-2113.452,646.0859;Inherit;True;Property;_TextureSample0;Texture Sample 0;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;92;-2098.977,837.4601;Inherit;True;Property;_TextureSample1;Texture Sample 0;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;85;-713.9974,54.58282;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;61;-569.5185,457.6799;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-1564.591,885.8138;Inherit;False;Property;_FoamDistortion;FoamDistortion;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;457.892,387.2788;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;93;1025.428,71.77885;Float;False;True;-1;3;ASEMaterialInspector;0;0;StandardSpecular;Tripolygon/TurtleShip_Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;0;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;0;61;0
WireConnection;28;0;69;0
WireConnection;60;0;28;0
WireConnection;60;1;64;0
WireConnection;66;0;68;0
WireConnection;66;1;21;0
WireConnection;66;2;62;0
WireConnection;78;0;62;0
WireConnection;34;0;35;0
WireConnection;38;0;34;0
WireConnection;39;0;38;0
WireConnection;39;1;41;0
WireConnection;40;0;34;0
WireConnection;40;1;42;0
WireConnection;42;0;36;0
WireConnection;42;1;44;0
WireConnection;41;0;36;0
WireConnection;41;1;43;0
WireConnection;36;0;37;0
WireConnection;33;0;91;0
WireConnection;33;1;92;0
WireConnection;75;0;33;0
WireConnection;76;0;75;0
WireConnection;76;1;77;0
WireConnection;80;0;60;0
WireConnection;80;1;76;0
WireConnection;83;1;59;0
WireConnection;83;2;62;0
WireConnection;72;0;79;0
WireConnection;79;0;24;0
WireConnection;79;1;78;0
WireConnection;81;1;27;0
WireConnection;81;2;62;0
WireConnection;21;0;17;0
WireConnection;21;1;20;0
WireConnection;21;2;88;0
WireConnection;22;0;23;0
WireConnection;84;0;22;0
WireConnection;84;1;86;0
WireConnection;88;0;85;0
WireConnection;91;0;31;0
WireConnection;91;1;39;0
WireConnection;92;0;31;0
WireConnection;92;1;40;0
WireConnection;85;0;84;0
WireConnection;85;1;87;0
WireConnection;61;0;80;0
WireConnection;61;1;65;0
WireConnection;90;0;88;0
WireConnection;90;1;79;0
WireConnection;93;0;66;0
WireConnection;93;1;33;0
WireConnection;93;3;83;0
WireConnection;93;4;81;0
WireConnection;93;9;72;0
ASEEND*/
//CHKSM=A1E64A50ACB1D66B391F41445E7BCEC1BF1CC80E