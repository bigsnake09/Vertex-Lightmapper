#include "UnityCG.cginc"

float Remap(float value, float from1, float to1, float from2, float to2)
{
	return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}

float InverseLerp(float a, float b, float value)
{
	if (a != b) return clamp((value - a) / (b - a), 0, 1);
	else return 0.0f;
}