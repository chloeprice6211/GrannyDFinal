Shader "Hidden/Distortion2D"
{
	Properties
	{
		_Intensity ("Displacement value", Range(0,1)) = 0.01
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Texture ("Displacement map (RGB)", 2D) = "black" {}
	}
	SubShader
	{
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			half _Intensity;

			struct v2f {
			   float4 pos : POSITION;
			   half2 uv : TEXCOORD0;
			};
						   
			v2f vert (appdata_img v){
			   v2f o;
			   o.pos = UnityObjectToClipPos (v.vertex);
			   o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
			   return o; 
			}

			sampler2D _MainTex; sampler2D _Texture;

			float4 frag(v2f_img i) : COLOR {
				half2 n = tex2D(_Texture, i.uv);
				half2 d = n * 2 - 1;
				i.uv += d * _Intensity;
				i.uv = saturate(i.uv);
			 
				float4 c = tex2D(_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
}
