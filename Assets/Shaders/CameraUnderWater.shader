Shader "Hidden/CameraUnderWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
        _DepthMap("Texture", 2D) = "black" {}
        _DepthStart("Depth Start Distance", float) = 1
        _DepthEnd("Depth End Distance", float) = 300
        _DepthColor("Depth Color", float) = (1,1,1,1)
        _DisToCenSq_Plus_depthDisSq_Max("disToCenSq_Plus_depthDisSq_Max", float) = 1.224745
        _ToSeeSome("forDebug", float) = 0.0
    }
    SubShader
    {
        // No culling or depth
        
        //Disable backface culling(Cull off)
        //depth duffer updating during rendering (ZWrite off),
        //Always draw a pixel regradless of depth (ZTest Always)
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture, _MainTex, _DepthMap;
            float _DepthStart, _DepthEnd, _DisToCenSq_Plus_depthDisSq_Max, _ToSeeSome;
            fixed4 _DepthColor;
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos: TEXTCOORD1;
            };

            //We add an extra screenPos attribute to the vertext data, and compute the screen position of each vertex in the vert() function below.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            

            
            //run on every pixel that is seen by the camera.
            //responsible for applying post-processing on the image camera sees
            fixed4 frag (v2f i) : SV_Target
            {

                

                // //the radius approach
                // float base = _DepthEnd - _DepthStart;
                // //float depth = LinearEyeDepth(tex2D(_DepthMap, i.screenPos.xy));
                // //center pixel depth, not current depth
                // float depth = LinearEyeDepth(tex2D(_DepthMap, _ScreenParams.xy/2));//center pixel depth, not current depth
                // depth = saturate((depth - _DepthStart) / base); //the normalized depth
                //
                // float2 screenPos = i.screenPos.xy/ _ScreenParams.xy;//normalized point
                //
                //
                //  // // Correct the screen position based on the aspect ratio of the screen
                //  // float2 screenPos = i.screenPos.xy / i.screenPos.w;
                //  // screenPos.y *= _ScreenParams.y / _ScreenParams.x;
                //
                // // Calculate the distance from the screen center in aspect-corrected coordinates
                // float3 screenCenter = float3(0.5, 0.5, 0);
                // float3 pixel =  float3(screenPos.xy, depth);
                // float dis3D = distance(pixel, screenCenter);
                // dis3D = saturate(dis3D/_DisToCenSq_Plus_depthDisSq_Max);
                // if(dis3D<_ToSeeSome) return half4(1.0,1.0,1.0,1.0);
                // fixed4 depthColor = (0.5 * glstate_lightmodel_ambient + _DepthColor * 0.5) * glstate_lightmodel_ambient.w;
                // fixed4 col = tex2D(_MainTex, i.screenPos);
                // col = lerp(col, depthColor, dis3D);
                // return col;

                float normalized_max_distance_toward_center = 0.7071f;
                // Sample the depth texture at the pixel's screen position and convert to linear depth
                float depth = LinearEyeDepth(tex2D(_DepthMap, i.screenPos.xy));
                depth = saturate((depth - _DepthStart) / (_DepthEnd - _DepthStart));
                // Calculate the distance from the screen center
                float2 screenCenter = float2(0.5, 0.5); // Assuming the screen coordinates go from 0 to 1
                float2 screenPosNormalized = i.screenPos.xy / i.screenPos.w; // Perspective divide
                screenPosNormalized.y *= _ScreenParams.y / _ScreenParams.x;
                float radialDistance = distance(screenPosNormalized, screenCenter);
                // Here you map radialDistance to your intensity range, say 0 (center) to 1 (edge)
                // You can adjust the 'fogScale' to control how quickly the fog effect falls off from the cente
                // // Adjust this value as needed
                //!!!!!!!!!!!!!!!!!!THIS!!!!!!!!!!!!!!!!!!!!!!
                float fogIntensity = saturate(radialDistance/normalized_max_distance_toward_center);//* fogScale for costum intensity control
                // Now, blend this fogIntensity with your depth value to get the final color effect
                // The depth value gives you the color based on camera depth, and fogIntensity
                // adjusts it based on distance from the screen center.
                // Apply fog intensity to the depth-based color
                fixed4 depthColor = (0.5 * glstate_lightmodel_ambient + _DepthColor * 0.5) * glstate_lightmodel_ambient.w;
                fixed4 col = tex2D(_MainTex, i.screenPos);
                // Blend between the original color and the depth color based on depth and fog intensity
                col = lerp(col, depthColor, depth);
                //!!!!!!!!!!!!!!AND THIS Together CREATE a edge FOG EFFECT!!!!!!!!!!!!!!!!!!!!!
                col = lerp(col, depthColor, fogIntensity);
                return col;

                
            }
            ENDCG
        }
    }
}