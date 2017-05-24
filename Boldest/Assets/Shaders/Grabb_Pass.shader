Shader "Unlit/Grabb_Pass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}		
		_DistortTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
	
		// Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;
			sampler2D _DistortTex;
			float _waveValue;
			float _distortAmount;

            half4 frag(v2f i) : SV_Target
            {
				float4 noise = tex2Dproj(_DistortTex,i.grabPos);
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos + float4( (_waveValue * noise.x * _distortAmount) ,0,0,0));
                return bgcolor;
            }
            ENDCG
		}
	}
}
