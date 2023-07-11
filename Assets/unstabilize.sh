#!/bin/bash

git add .
git stash
git reset --hard HEAD^
git stash pop
