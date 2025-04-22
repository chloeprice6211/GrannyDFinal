Shader "Hidden/Waves"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Intensity("Noise value", Range(0,10)) = 1
		_ValueX("Height of distorted area", Range(-2,2)) = 1
		_ValueY("Thickness of distorted area", Range(0,4096)) = 1024
		_ValueZ("Shifts the noise", Range(-1,1)) = 0.01

	}
	SubShader
	{
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			half _Intensity; half _ValueX; half _ValueY; half _ValueZ;

			struct v2f {
			   float4 pos : POSITION;
			   half2 uv : TEXCOORD0;
			};

			float rand(float co){
   		 		return (sin(dot(co ,float2(12.8983,78.2338))) * 43758.5453);
			}
			   
			v2f vert (appdata_img v){
			   v2f o;
			   o.pos = UnityObjectToClipPos (v.vertex);
			   o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
			   return o; 
			}

			    
			uniform sampler2D _MainTex; 

			fixed4 frag(v2f i) : COLOR {

				half rnd = rand(rand(i.uv.y + _ValueZ)) * i.uv.y;
				i.uv.x += (rand(i.uv.y) + rnd) * _Intensity * 0.000001f * pow(_ValueY * 1024,-(pow(i.uv.y - _ValueX,2)));

				float4 c = tex2D(_MainTex, i.uv);
				
		       	return c;

         	}
			ENDCG
		}
	}
}

