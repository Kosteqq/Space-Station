#ifndef COLOR_LAYER_FUNC
#define COLOR_LAYER_FUNC

// In vertex painting acceptable values are:
// for primary layer: 0.1 - 0.19999
// for light layer: 0.2 - 0.29999
// etc

static const float BASE_LAYER_ID = 0.0; 
static const float PRIMARY_LAYER_ID = 1.0; 
static const float LIGHT_LAYER_ID = 2.0; 
static const float SECONDARY_LAYER_ID = 3.0; 
static const float SIDE_LAYER_ID = 4.0; 

void GetColorLayerMask_float(float4 VertexColor, out float BaseLayerMask, out float PrimaryLayerMask,
	out float SecondaryLayerMask, out float LightLayerMask, out float SideLayerMask)
{
	float id = VertexColor.r;
	id = floor(id * 10.0);

	BaseLayerMask		  = id == BASE_LAYER_ID;
	PrimaryLayerMask	  = id == PRIMARY_LAYER_ID;
	SecondaryLayerMask	  = id == SECONDARY_LAYER_ID;
	LightLayerMask		  = id == LIGHT_LAYER_ID;
	SideLayerMask		  = id == SIDE_LAYER_ID;
}

#endif
