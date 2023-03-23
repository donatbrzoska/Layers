#!/usr/bin/env python3
import sys
import os

if len(sys.argv) < 2:
    print("Usage: python3 update_numthreads.py \"x,y,z\"")
    sys.exit(1)
else:
    numthreads = sys.argv[1]

for path, dirnames, filenames in os.walk('./'):
    for filename in filenames:
        if filename.endswith(".compute"):
            # print('{} {} {}'.format(repr(path), repr(dirnames), repr(filenames)))
            filepath = os.path.join(path, filename)
            # print(filepath)

            with open(filepath, "r") as file:
                content = file.read()
            
            content_split = content.split("\n")

            # look for line and replace
            for i, line in enumerate(content_split):
                if "numthreads(" in line:
                    content_split[i] = f"[numthreads({numthreads})]"
            
            content = "\n".join(content_split)

            # print(content)

            with open(filepath, "w") as file:
                file.write(content)