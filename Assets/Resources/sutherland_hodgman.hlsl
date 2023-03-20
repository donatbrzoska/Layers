bool vertex_inside(float2 vertex, float2 edge_v1, float2 edge_v2)
{
    return (edge_v1.x - vertex.x) * (edge_v2.y - vertex.y) >= (edge_v1.y - vertex.y) * (edge_v2.x - vertex.x);
}

float2 compute_intersection(float2 a, float2 b, float2 p, float2 q)
{
    if (a.y == b.y && b.y == p.y && p.y == q.y) { // lines identical with same y
        if (a.x < p.x) {
            return float2(min(p.x, q.x), a.y);
        } else {
            return float2(max(p.x, q.x), a.y);
        }
    } else if (a.x == b.x && b.x == p.x && p.x == q.x) { // lines identical with same x
        if (a.y < p.y) {
            return float2(a.x, min(p.y, q.y));
        } else {
            return float2(a.x, max(p.y, q.y));
        }
    } else {
        float A1 = b.y - a.y;
        float B1 = a.x - b.x;
        float C1 = A1 * a.x + B1 * a.y;

        float A2 = q.y - p.y;
        float B2 = p.x - q.x;
        float C2 = A2 * p.x + B2 * p.y;

        float det = A1 * B2 - A2 * B1;
        float x = (B2 * C1 - B1 * C2) / det;
        float y = (A1 * C2 - A2 * C1) / det;

        return float2(x, y);
    }
}

// inspired by Java implementation from https://rosettacode.org/wiki/Sutherland-Hodgman_polygon_clipping
// TODO No support for differing canvas and rakel resolutions
float calculate_exact_overlap(int2 adjacent_reservoir_pixel, float2 reservoir_pixel, float reservoir_pixel_rotation)
{
    // bool debug_this = adjacent_reservoir_pixel.x == 0 && adjacent_reservoir_pixel.y == 1
    //                && f2_eq(reservoir_pixel, float2(0.13, 0.87));
    // if (debug_this){
    //     Debug[XY(id.x, id.y, CalculationSize.x)] = float2(7,7);
    // }

    // 1. calculate clip and subject polygon -> adjacent_reservoir_pixel is clipping reservoir_pixel (back translated and rotated from canvas pixel)
    // vertices are stored in counter clockwise order
    float2 clip_poly[8];
    uint clip_len = 0;
    float2 subj_poly[8];
    uint subj_len = 0;

    float2 ll = float2(-0.5, -0.5);
    float2 ul = float2(-0.5,  0.5);
    float2 ur = float2( 0.5,  0.5);
    float2 lr = float2( 0.5, -0.5);

    float2 adjacent_reservoir_pixel_ = float2(adjacent_reservoir_pixel.x, adjacent_reservoir_pixel.y);
    clip_poly[0] = adjacent_reservoir_pixel_ + ll;
    clip_poly[1] = adjacent_reservoir_pixel_ + lr;
    clip_poly[2] = adjacent_reservoir_pixel_ + ur;
    clip_poly[3] = adjacent_reservoir_pixel_ + ul;
    clip_len = 4;

    // TODO this doesn't work for a curved or tilted rakel
    subj_poly[0] = reservoir_pixel + rotate(ll, reservoir_pixel_rotation, float2(0,0));
    subj_poly[1] = reservoir_pixel + rotate(lr, reservoir_pixel_rotation, float2(0,0));
    subj_poly[2] = reservoir_pixel + rotate(ur, reservoir_pixel_rotation, float2(0,0));
    subj_poly[3] = reservoir_pixel + rotate(ul, reservoir_pixel_rotation, float2(0,0));
    subj_poly[4] = float2(0,0);
    subj_poly[5] = float2(0,0);
    subj_poly[6] = float2(0,0);
    subj_poly[7] = float2(0,0);
    subj_len = 4;

    // 2. calculate intersection polygon
    float2 input_list[8];
    uint input_len;
    float2 output_list[] = subj_poly; // put subject polygon to output_list as this should be the input of the next (first) iteration
    uint output_len = subj_len;
    for (uint i=0; i<clip_len; i++) {
        input_list = output_list; // input is always output of last iteration = current state of clipped subject
        input_len = output_len;

        output_len = 0; // reset output_list

        float2 clip_a = clip_poly[i];
        float2 clip_b = clip_poly[(i == clip_len-1) ? 0 : (i+1)]; // wrap array around in last iteration
        for (uint j=0; j<input_len; j++) {
            float2 subj_a = input_list[j];
            float2 subj_b = input_list[(j == input_len-1) ? 0 : (j+1)]; // wrap array around in last iteration

            float2 intersection = compute_intersection(clip_a, clip_b, subj_a, subj_b);
            // challenge: vertices must get inserted in order
            if (vertex_inside(subj_b, clip_a, clip_b)) {
                if (!vertex_inside(subj_a, clip_a, clip_b)) {
                    output_list[output_len] = intersection;
                    output_len++;
                }
                output_list[output_len] = subj_b;
                output_len++;
            } else if (vertex_inside(subj_a, clip_a, clip_b)) {
                output_list[output_len] = intersection;
                output_len++;
            }
        }
    }

    // 3. calculate area of intersection polygon == overlap 0..1
    float area = 0;
    for (uint k=0; k<output_len; k++) {
        float2 v_i = output_list[k];
        float2 v_i_1 = output_list[(k == output_len-1) ? 0 : (k+1)]; // wrap array around in last iteration

        area += v_i.x * v_i_1.y - v_i_1.x * v_i.y;
    }
    area *= 0.5;
    return area;
}