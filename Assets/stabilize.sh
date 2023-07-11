#!/bin/bash

git add .
git stash
./stabilize.py
git commit -a -m "tmp stabilize"
git stash pop
