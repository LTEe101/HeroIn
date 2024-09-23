Shader "ABDeveloper/InterstellarWebGL"
{
	Properties
	{
		_MainTex ("Noise texture", 2D) = "black" {}
		_SpaceColor ("Space Color", Color) = (1,1,1,1)
		_StarsColor ("Stars Color", Color) = (1,1,1,1)
		_StarBrightness ("Star Brightness", Range(0.5,5)) = 1
		_FOV ("Field of view", Range(0.5,2)) = 1
		_Speed("Speed", Range(-10,10)) = 1
		_Zoom("Zoom", Range(0.1,2)) = 0.4
		_StarsCount("Stars count", Range(10,35)) = 25
		
		_Start("Start", float) = 0
		_StarsLengthDuringWarp("Stars Length During Warp", Range(0.1,1)) = 1 

		_FadeIn("FadeIn", float) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#define GAMMA 2.2

			#include "UnityCG.cginc"

			struct inputVert
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct outputVert
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _FOV;
			float _Speed;
			fixed3 _SpaceColor;
			fixed3 _StarsColor;
			float _Zoom;
			float  _Test;
			float _StarBrightness;
			int _StarsCount;
			float _Start;
			float _StarsLengthDuringWarp;
			float4 _TimeManager;
			float _FadeIn;

			float _TimeElapsedSinceBeggining;

			fixed3 toGamma(fixed3 col)
			{
				float gamma = 1/GAMMA;
				return pow(col, gamma);
			}

			fixed4 Noise(int2 x)
			{
				float2 xf2 = float2((x.x+2.5) /256, (x.y+2.5)/256);
				return tex2D(_MainTex, xf2);
			}

			outputVert vert (inputVert i)
			{
				outputVert o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;
				return o;
			}

			fixed4 frag (outputVert i) : SV_Target
			{
				float3 ray;
				ray.xy = _FOV*(i.vertex.xy - _ScreenParams.xy* (0.5))/ _ScreenParams.x;
				ray.z = _Zoom;
				float offset;
				
				_FadeIn -= 1 * _Time.y;
				if(_FadeIn > 0) {
					return _FadeIn;
				}

				if(_Start > 0.1) {
					offset = (_Time.x - _TimeElapsedSinceBeggining - 2.87)* _Speed * 1.5;
				} else
				{	 offset = 2.87;
				}
				_TimeElapsedSinceBeggining = _Time.x;

				float speed2 = (cos(offset)+1.0) * 2.0;
				float speed = speed2 + 0.1;
				offset += sin(offset) * 0.96;

				fixed3 col = _SpaceColor * _FadeIn;

				float3 stp = ray/max(abs(ray.x), abs(ray.y));

				float3 pos = 2.0 * stp + 0.5;

				for(int i = 0; i< _StarsCount; i++)
				{
					float z = Noise(int2(pos.xy)).x;
					z = frac(z- offset);

					float d = 55.0*z - pos.z;

					float w = pow(max(0, 1.0-8.0* length(frac(pos.xy)-0.5)),
					2.0);

					float max1 = 0;
					float max2 = 1.0 - abs(d+speed2 * 0)/(speed * _StarsLengthDuringWarp);

					float3 c = max(max1, max2);
					col+= 1.5*(1.0 - z) * c * w;
					pos+= stp;
					
				}

				// just invert the colors
				//col = 1 - col;
				return fixed4((col*_StarsColor* _StarBrightness), 1.0);
			}
			ENDCG
		}
	}
}
