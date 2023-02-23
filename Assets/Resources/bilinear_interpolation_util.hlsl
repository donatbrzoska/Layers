// inspired by https://stackoverflow.com/a/27162334
// returns the ratio of the max possible overlap
// NOTE that this has to be adjusted for a curved rakel
float calculate_unrotated_overlap(int2 pixel, uint pixel_size, float2 position, uint position_pixel_size)
{
    float pixel_size_half = 0.5 * pixel_size;
    float adj_square_x_min = (float)pixel.x - pixel_size_half;
    float adj_square_x_max = (float)pixel.x + pixel_size_half;
    float adj_square_y_min = (float)pixel.y - pixel_size_half;
    float adj_square_y_max = (float)pixel.y + pixel_size_half;

    float position_pixel_size_half = 0.5 * position_pixel_size;
    float pos_square_x_min = position.x - position_pixel_size_half;
    float pos_square_x_max = position.x + position_pixel_size_half;
    float pos_square_y_min = position.y - position_pixel_size_half;
    float pos_square_y_max = position.y + position_pixel_size_half;

    float dx = min(adj_square_x_max, pos_square_x_max) - max(adj_square_x_min, pos_square_x_min);
    float dy = min(adj_square_y_max, pos_square_y_max) - max(adj_square_y_min, pos_square_y_min);
    dx = max(0, dx);
    dy = max(0, dy);

    float overlap = (dx*dy);
    return overlap;
}