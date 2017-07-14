#!/bin/sh

name="OpenByVSCode"
d="src/$name/bin/Release"
zip -j "$name.zip" $d/$name.exe

f="readme.txt"
cp README.md $f
echo "[Source](https://github.com/yanxyz/$name)" >> $f
zip -ml "$name.zip" $f
