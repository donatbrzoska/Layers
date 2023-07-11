#!/usr/bin/env python3

# NOTE:
# This is actually a hack, because it is unclear, why uints cause crashes
# under certain circumstances.
# There may be an undiscovered underflow, leading to an infinite loop.

import os

for path, dirnames, filenames in os.walk('./Resources/'):
    for filename in filenames:
        if filename.endswith(".compute") or filename.endswith(".hlsl"):
            filepath = os.path.join(path, filename)

            with open(filepath, "r") as file:
                content = file.read()
            
            content_fixed = content.replace("uint", "int")

            with open(filepath, "w") as file:
                file.write(content_fixed)