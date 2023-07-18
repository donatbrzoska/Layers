struct Rakel {
    float length;
    float width;

    float3 anchor;

    float3 position;
    int auto_z_enabled;
    float position_base_z;
    float actual_layer_thickness;
    float pressure;
    float rotation;
    float tilt;

    float edge_z;

    float3 upper_left;
    float3 upper_right;
    float3 lower_left;
    float3 lower_right;

    float3 ul_tilted;
    float3 ur_tilted;
    float3 ll_tilted;
    float3 lr_tilted;
};