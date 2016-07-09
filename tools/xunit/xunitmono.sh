#!/bin/sh
BASEDIR=$(dirname $0)
TESTDIR=$(dirname $1)

# we need to copy xunit to the test dir so AppDomain.CurrentDomain.BaseDirectory
# points to the test dir instead of the xunit dir when using -noappdomain
# as Nancy relies on this to locate views.
# -noappdomain works around https://bugzilla.xamarin.com/show_bug.cgi?id=39251
cp ${BASEDIR}/xunit.console.x86.exe ${TESTDIR}
mono -O=-gshared ${TESTDIR}/xunit.console.x86.exe $* -noappdomain
