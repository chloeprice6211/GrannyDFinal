Shader "Hidden/CRTBulge"
{
	Properties
	{
		_Intensity ("Screen size", float) = 1.71
		_MainTex ("Base (RGB)", 2D) = "white" {}
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

			fixed4 frag(v2f i) : COLOR {
				float2 uv = i.uv - 0.5f;
				float d = 1 / (sqrt(1 - uv.x * uv.x - uv.y * uv.y) * tan(_Intensity * 0.5f)) ;
				float2 p = uv * d + 0.5f;
				float4 c;
				if(p.x>=1 || p.y>=1 || p.x<=0 || p.y<=0) 
					c = float4(0,0,0,1);
				else 
					c = tex2D(_MainTex, p);
				return c;
			}
			ENDCG
		}
	}
}
