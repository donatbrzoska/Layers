#!/usr/bin/env python3
import os

for path, dirnames, filenames in os.walk('./Resources/'):
    for filename in filenames:
        if filename.endswith(".compute"):
            # 1. delete current shader base
            shader_filepath = os.path.join(path, filename)

            with open(shader_filepath, "r") as file:
                shader_content = file.read()
            
            shader_content_split = shader_content.split("\n")

            # this also pops the newline after the shader base end line we look for
            while shader_content_split.pop(0) != "// ###################################### SHADER BASE END ######################################":
                pass

            # 2. add new shader base
            shader_base_filepath = os.path.join(path, "shader_base.hlsl")
            with open(shader_base_filepath, "r") as file:
                shader_base = file.read()

            updated_content = shader_base + "\n".join(shader_content_split)

            with open(shader_filepath, "w") as file:
                file.write(updated_content)