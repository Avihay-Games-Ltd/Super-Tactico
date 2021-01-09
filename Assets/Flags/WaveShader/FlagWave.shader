// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Selfmade/FlagWave" 
{ 

Properties 
{ 
    _Color ("Main Color", Color) = (1,1,1,1) 
    _MainTex ("Texture", 2D) = "white" { }
    _WaveSpeed ("Wave Speed", Range(0.0, 150.0)) = 50.0 
} 

SubShader 
{ 
    Pass 
    { 
       CULL Off 
        
      CGPROGRAM 
      #pragma vertex vert 
      #pragma fragment frag 
      #include "UnityCG.cginc" 
      #include "AutoLight.cginc" 
        
      float4 _Color; 
      sampler2D _MainTex;
      float _WaveSpeed;

      // vertex input: position, normal 
      struct appdata { 
          float4 vertex : POSITION; 
          float4 texcoord : TEXCOORD0; 
      }; 
        
      struct v2f { 
          float4 pos : POSITION; 
          float2 uv: TEXCOORD0; 
      }; 
        
      v2f vert (appdata v) { 
          v2f o; 

        float sinOff=v.vertex.x+v.vertex.y+v.vertex.z; 
        float t=-_Time*_WaveSpeed; 
        float fx=v.texcoord.x; 
        float fy=v.texcoord.x*v.texcoord.y; 

        v.vertex.x+=sin(t*1.45+sinOff)*fx*0.5; 
        v.vertex.y=sin(t*3.12+sinOff)*fx*0.5-fy*0.9; 
        v.vertex.z-=sin(t*2.2+sinOff)*fx*0.2; 
    
          o.pos = UnityObjectToClipPos( v.vertex ); 
          o.uv = v.texcoord; 

         return o; 
      } 
        
      float4 frag (v2f i) : COLOR 
      { 
         half4 color = tex2D(_MainTex, i.uv); 
         return color; 
      } 

      ENDCG 

      SetTexture [_MainTex] {combine texture} 
    } 
} 
   Fallback "VertexLit" 
}