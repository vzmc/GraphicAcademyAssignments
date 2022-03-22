#pragma once

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

float3 Direction;
half3 Color;

void GetLightInfo()
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
#else
    Light light = GetMainLight();
    Direction = light.direction;
    Color = light.color;
    //DistanceAttenuation = light.distanceAttenuation;
    //ShadowAttenuation = light.shadowAttenuation;
#endif
}
