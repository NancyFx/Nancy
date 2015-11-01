#!/bin/sh
BASEDIR=$(dirname $0)

mono -O=-gshared ${BASEDIR}/xunit.console.x86.exe $*
