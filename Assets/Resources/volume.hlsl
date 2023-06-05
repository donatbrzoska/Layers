
// ###################################### HELPER ######################################

float ratio_to_value(float ratio, float value_min, float value_max)
{
    float d_value = value_max - value_min;
    return value_min + ratio * d_value;
}

float map_value_to_domain(
    float value, float value_domain_min, float value_domain_max,
    float target_domain_min, float target_domain_max)
{
    float d_range = target_domain_max - target_domain_min;

    float d_value = value_domain_max - value_domain_min;
    float part_value = (value - value_domain_min) / d_value;

    return target_domain_min + part_value * d_range;
}

float clamp_to_ratio(float value)
{
    return clamp(value, 0, 1);
}


// ###################################### EMIT ######################################

// usable domain of function is 0 .. emit_distance_max
// returns values in range 0 .. 1
float emit_volume_distance_function(float distance, float emit_distance_max)
{
    return - pow(2 / emit_distance_max * distance - 1, 2) + 1;
}

float emit_volume_distance(float distance, float emit_distance_max, float emit_volume_min, float emit_volume_max)
{
    // no mapping is needed because distance domain is also function domain
    float f_result = emit_volume_distance_function(distance, emit_distance_max);
    float ratio = clamp_to_ratio(f_result);

    float volume = ratio_to_value(ratio, emit_volume_min, emit_volume_max);
    return volume;
}


float2 emit_volume_tilt_function_domain_x()
{
    return float2(0, 1);
}

// returns value in range 1 .. 0
float emit_volume_tilt_function(float x)
{
    return exp(-4 * x);
}

float emit_volume_tilt(float tilt, float tilt_max, float emit_volume_min, float emit_volume_max)
{
    // mapping is needed because tilt angle domain is different from function domain
    float tilt_mapped_to_function_domain = map_value_to_domain(
        tilt, 0, tilt_max,
        emit_volume_tilt_function_domain_x().x, emit_volume_tilt_function_domain_x().y);
    float f_result = emit_volume_tilt_function(tilt_mapped_to_function_domain);
    float ratio = clamp_to_ratio(f_result);

    float volume = ratio_to_value(ratio, emit_volume_min, emit_volume_max);
    return volume;
}


float emit_volume(
    float distance, float emit_distance_max,
    float tilt, float tilt_max,
    float emit_volume_min, float emit_volume_max)
{
    float volume_distance = emit_volume_distance(distance, emit_distance_max, emit_volume_min, emit_volume_max);
    float volume_tilt = emit_volume_tilt(tilt, tilt_max, emit_volume_min, volume_distance);
    return volume_tilt;
}


// ###################################### PICKUP ######################################

// returns value in range 1 .. 0
float pickup_volume_distance_function(float distance, float pickup_distance_max)
{
    return -1 / pickup_distance_max * distance + 1;
}

float pickup_volume_distance(float distance, float pickup_distance_max, float pickup_volume_min, float pickup_volume_max)
{
    // no mapping is needed because distance domain is also function domain
    float f_result = pickup_volume_distance_function(distance, pickup_distance_max);
    float ratio = clamp_to_ratio(f_result);

    float volume = ratio_to_value(ratio, pickup_volume_min, pickup_volume_max);
    return volume;
}


float2 pickup_volume_tilt_function_domain_x()
{
    return float2(-1, 0);
}

// returns value in range 1 .. 0
float pickup_volume_tilt_function(float x)
{
    return exp(3 * x);
}

float pickup_volume_tilt(float tilt, float tilt_max, float pickup_volume_min, float pickup_volume_max)
{
    // mapping is needed because tilt angle domain is different from function domain
    float tilt_mapped_to_function_domain = map_value_to_domain(
        tilt, 0, tilt_max,
        pickup_volume_tilt_function_domain_x().x, pickup_volume_tilt_function_domain_x().y);
    float f_result = pickup_volume_tilt_function(tilt_mapped_to_function_domain);
    float ratio = clamp_to_ratio(f_result);

    float volume = ratio_to_value(ratio, pickup_volume_min, pickup_volume_max);
    return volume;
}


float pickup_volume(
    float distance, float pickup_distance_max,
    float tilt, float tilt_max,
    float pickup_volume_min, float pickup_volume_max)
{
    float volume_distance = pickup_volume_distance(distance, pickup_distance_max, pickup_volume_min, pickup_volume_max);
    float volume_tilt = pickup_volume_tilt(tilt, tilt_max, pickup_volume_min, volume_distance);
    return volume_tilt;
}