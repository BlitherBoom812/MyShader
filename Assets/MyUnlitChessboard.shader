Shader "Unlit/MyChessboard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			fixed4 _Color;
			
            struct appdata {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD;
            };

            struct v2f {
                float4 pos: SV_POSITION;
                float2 uv: TEXCOORD;
            };

			v2f vert ( appdata v ) // 应用程序到几何（顶点着色器）的映射
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed checkerboard (float2 uv : TEXCOORD) : COLOR
			{
				// float2 uv2 = uv * 2.0 - 1.0;
				// uv2.y *= -1.0;
				// uv2 += 0.5;
				// uv2.y *= 2.0;
				// uv2.y += 0.5;
				// return (uv2.x < 0.5) ? 0 : 1;
				float2 repeatuv = uv * 10;
				float2 c = floor(repeatuv) / 2;
				float checker = frac(c.x + c.y) * 2.0;
				return checker;
			}

			fixed4 frag (v2f i) : SV_Target // 光栅（片段着色器）到像素的映射
			{
				fixed col = checkerboard(i.uv);
				return col * _Color;
			}
			ENDCG
		}
	}	
}