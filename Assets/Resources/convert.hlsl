// surface position anchored at center of surface
float3 pixel_to_world_space(int2 pixel, uint texture_resolution, float3 surface_position, float2 surface_size)
{
    float pixel_size = 1/float(texture_resolution);
    float3 positive_surface_aligned = float3(0.5*pixel_size + pixel.x*pixel_size,
                                            0.5*pixel_size + pixel.y*pixel_size,
                                            0);

    float3 surface_lower_left = surface_position - float3(surface_size.x/2, surface_size.y/2, 0);
    float3 surface_aligned = positive_surface_aligned + surface_lower_left;

    return surface_aligned;
}

// Only used for converting rakel anchor in initial rakel state to index space.
// RakelAnchor is relative to initial rakel state lower left, therefore canvas position does not matter
float2 rakel_anchor_to_index_space(float3 anchor, uint texture_resolution)
{
    float3 pixel_count = anchor * texture_resolution;
    float2 index_space = float2(pixel_count.x - 0.5, pixel_count.y - 0.5);
    return index_space;
}