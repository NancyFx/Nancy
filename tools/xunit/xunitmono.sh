#!/bin/sh
BASEDIR=$(dirname $0)

mono ${BASEDIR}/xunit.console.clr4.x86.exe $*
