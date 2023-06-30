bool f2_eq(float2 a, float2 b)
{
    float err = 0.01;
    return abs(a.x - b.x) < err
        && abs(a.y - b.y) < err;
}

float FLOAT_PRECISION()
{
    return 0.01;
}

bool floats_equal(float a, float b)
{
    return abs(a - b) < FLOAT_PRECISION();
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

float3 rotate_by_y(float3 vec, float angle, float3 around)
{
    float3 vec_ = vec - around;

    float rad = radians(angle);
    float s = sin(rad);
    float c = cos(rad);
    float3x3 rotation = {
        c,  0, -s,
        0,  1,  0,
        s,  0,  c
    };

    float3 result = mul(vec_, rotation);
    return result + around;
}

float2 rotate_by_y_2D(float2 vec, float angle, float2 around)
{
    float3 vec_ = float3(vec.x, vec.y, 0);
    float3 around_ = float3(around.x, around.y, 0);

    float3 rotated = rotate_by_y(vec_, angle, around_);
    return float2(rotated.x, rotated.y);
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