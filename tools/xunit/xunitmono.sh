#!/bin/sh
BASEDIR=$(dirname $0)

mono -O=-gshared ${BASEDIR}/xunit.console.clr4.x86.exe $*
