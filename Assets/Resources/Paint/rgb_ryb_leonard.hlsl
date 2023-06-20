// https://web.archive.org/web/20130525061042/www.insanit.net/tag/rgb-to-ryb/

// Author: Arah J. Leonard
// Copyright 01AUG09
// Distributed under the LGPL - http://www.gnu.org/copyleft/lesser.html
// ALSO distributed under the The MIT License from the Open Source Initiative (OSI) - http://www.opensource.org/licenses/mit-license.php
// You may use EITHER of these licenses to work with / distribute this source code.
// Enjoy!

// Convert a red-green-blue system to a red-yellow-blue system.
float3 rgb_to_ryb_leonard(float3 RGB)
{
    float r = RGB.x;
    float g = RGB.y;
    float b = RGB.z;

	// Remove the whiteness from the color.
	float w = min(min(r, g), b);
	r = r - w;
	g = g - w;
	b = b - w;

	float mg = max(max(r, g), b);

	// Get the yellow out of the red+green.
	float y = min(r, g);
	r -= y;
	g -= y;

	// If this unfortunate conversion combines blue and green, then cut each in half to preserve the value's maximum range.
	if (b != 0 && g != 0)
    {
		b /= 2.0;
		g /= 2.0;
    }

	// Redistribute the remaining green.
	y += g;
	b += g;

	// Normalize to values.
	float my = max(max(r, y), b);
	if (my != 0)
    {
		float n = mg / my;
		r *= n;
		y *= n;
		b *= n;
    }

	// Add the white back in.
	r += w;
	y += w;
	b += w;

	// And return back the ryb typed accordingly.
	return float3(r, y, b);
}

// Convert a red-yellow-blue system to a red-green-blue system.
float3 ryb_to_rgb_leonard(float3 RYB)
{
    float r = RYB.x;
    float y = RYB.y;
    float b = RYB.z;

	// Remove the whiteness from the color.
	float w = min(min(r, y), b);
	r = r - w;
	y = y - w;
	b = b - w;

	float my = max(max(r, y), b);

	// Get the green out of the yellow and blue
	float g = min(y, b);
	y -= g;
	b -= g;

	if (b != 0 && g != 0)
    {
		b *= 2.0;
		g *= 2.0;
    }

	// Redistribute the remaining yellow.
	r += y;
	g += y;

	// Normalize to values.
	float mg = max(max(r, g), b);
	if (mg != 0)
    {
		float n = my / mg;
		r *= n;
		g *= n;
		b *= n;
    }

	// Add the white back in.
	r += w;
	g += w;
	b += w;

	// And return back the ryb typed accordingly.
	return float3(r, g, b);
}