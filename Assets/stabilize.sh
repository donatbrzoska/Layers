#!/bin/bash

status=`git status`
clean="nothing to commit, working tree clean"

if [[ "$status" != *"$clean"* ]]; then
	git add .
	git stash
fi

./stabilize.py
git commit -a -m "tmp stabilize"

if [[ "$status" != *"$clean"* ]]; then
        git stash pop
fi
