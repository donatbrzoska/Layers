bool f2_eq(float2 a, float2 b)
{
    float err = 0.01;
    return abs(a.x - b.x) < err
        && abs(a.y - b.y) < err;
}

// rotates clockwise
float3 rotate_by_z(float3 vec, float angle, float3 around)
{
    float3 vec_ = vec - around;

    float rad = radians(angle);
    float s = sin(rad);
    float c = cos(rad);
    float3x3 rotation = {
        c, -s, 0,
        s,  c, 0,
        0,  0, 1
    };
    
    float3 result = mul(vec_, rotation);
    return result + around;


    // float3 origin_aligned = vec - around;
    // float2 vec_ = float2(origin_aligned.x, origin_aligned.y);

    // float rad = radians(angle);
    // float s = sin(rad);
    // float c = cos(rad);
    // float2x2 rotation = {
    //     c, -s,
    //     s,  c,
    // };
    
    // float2 result = mul(vec_, rotation);
    // return float3(result.x, result.y, vec.z) + around;
}

// only needed for tilting rectangles
// -> converts float2 to float3, rotates clockwise and then converts back to float2
float2 rotate_by_y(float2 vec, float angle, float2 around)
{
    float2 vec_ = vec - around;
    float3 vec__ = float3(vec_.x, vec_.y, 0);

    float rad = radians(angle);
    float s = sin(rad);
    float c = cos(rad);
    float3x3 rotation = {
        c,  0, -s,
        0,  1,  0,
        s,  0,  c
    };

    float3 result = mul(vec__, rotation);
    float2 result_ = float2(result.x, result.y);
    return result_ + around;
}

// rotates clockwise
float2 rotate(float2 vec, float angle, float2 around)
{
    float2 vec_ = vec - around;

    float rad = radians(angle);
    float s = sin(rad);
    float c = cos(rad);
    float2x2 rotation = {
        c, -s,
        s,  c,
    };

    float2 result = mul(vec_, rotation);
    return result + around;
}

// float2 rotate_around_origin(int2 vec, float angle)
// {
//     float rad = radians(angle);
//     float s = sin(rad);
//     float c = cos(rad);
//     float2x2 mat = {
//         c, -s,
//         s,  c
//     };
//     return mul(vec, mat);
// }

// // b is right from a
// float angle_between(float3 a, float3 b)
// {
//     return acos(dot(a,b)/(length(a)*length(b))); // TODO MAYBE THIS IS BUGGY BECAUSE OF RADS/DEGREES
// }