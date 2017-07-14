#!/bin/sh

# MSys2 shell
# pacman -S zip unzip

name="OpenByVSCode"
zipfile="$name.zip"

rm $zipfile 2> /dev/null

zip -j "$zipfile" \
    "$name/bin/Release/$name.exe" \
    "$name/Usage.txt" "README.md" "LICENSE"

ini="$name.sample.ini"
cp "$name/$name.ini" $ini
zip -m "$zipfile" $ini
