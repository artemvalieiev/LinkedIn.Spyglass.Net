#!/bin/sh

# URL of the file to download
url="https://repo1.maven.org/maven2/com/linkedin/android/spyglass/spyglass/3.0.3/spyglass-3.0.3.aar"

# Destination file name
dest="./src/LinkedIn.Spyglass/LinkedIn.Spyglass/spyglass_3.0.3.aar"

# Use curl to download the file
curl -o $dest $url