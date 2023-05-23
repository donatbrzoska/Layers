#!/usr/bin/env python3
import os
import itertools

for path, dirnames, filenames in itertools.chain(os.walk('./Scripts/'), os.walk('./Resources/'), os.walk('./Tests/')):
    for filename in filenames:
        if filename.endswith(".cs"):
            filepath = os.path.join(path, filename)

            with open(filepath, "r") as file:
                content = file.read()
            
            content_fixed = content.replace("\r\n", "\n")

            with open(filepath, "w") as file:
                file.write(content_fixed)