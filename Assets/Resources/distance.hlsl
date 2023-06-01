
float distance_point_plane(float3 point_position, float3 plane_supp_vec, float3 plane_normal)
{
    float3 normal_0 = normalize(plane_normal);
    return abs(dot(point_position - plane_supp_vec, normal_0));
}