#!/bin/bash
echo Running on Mono 2.x
rake mono
echo Running on Mono 3.x
sudo tar xk -C "/opt" -f /opt/mono-3.0.12-bin.tar.bz2
export PATH="/opt/mono/bin:$PATH"
export LD_LIBRARY_PATH="/opt/mono/bin"
rake mono
