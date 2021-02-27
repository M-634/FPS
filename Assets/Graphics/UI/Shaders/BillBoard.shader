Shader "Unlit/BillBoard"
{
    //UIにおけるビルボードを実装する汎用的なシェーダ
    //colorはimageの頂点カラーを使用する
    //カメラとの距離が指定した距離以下なら描画する。
    Properties
    {
        _InvisibleDistance("InvisibleDistance",float) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
        
            #include "UnityCG.cginc"

            float _InvisibleDistance; 

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos: TEXCOORD0;
                float4 color : COLOR;
            };

        
            v2f vert (appdata v)
            {
                v2f o;
            
                //常にカメラを向く(y軸回転)
                float3 viewPos = UnityObjectToViewPos(float3(0,0,0));
                float3 scaleRotatePos = mul((float3x3)unity_ObjectToWorld,v.vertex);
                float3x3 viewRotateY = float3x3(
                    1,UNITY_MATRIX_V._m01,0,
                    0,UNITY_MATRIX_V._m11,0,
                    0,UNITY_MATRIX_V._m21,-1
                );
                viewPos += mul(viewRotateY,scaleRotatePos);
                o.vertex = mul(UNITY_MATRIX_P,float4(viewPos,1));
 
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               //カメラとオブジェクトの距離を取得
               float cameraToObjLength = length(_WorldSpaceCameraPos - i.worldPos);
               //Lerpを使ってAlphaを変化
               fixed alpha = fixed(lerp(i.color.a,0,cameraToObjLength / _InvisibleDistance));
               i.color.a = alpha;
               //Alphaが0以下なら描画しない
               clip(alpha);

               return i.color;
            }
            ENDCG
        }
    }
}
