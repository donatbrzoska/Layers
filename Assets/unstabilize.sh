#!/bin/bash

status=`git status`
clean="nothing to commit, working tree clean"

if [[ "$status" != *"$clean"* ]]; then
	git add .
	git stash
fi

git reset --hard HEAD^

if [[ "$status" != *"$clean"* ]]; then
	git stash pop
fi
