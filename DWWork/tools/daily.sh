#!/bin/bash
param1=$1
logdir="/Users/a123/Documents/cgg/build/log/"
logfile="${logdir}AutoBuild${param1}.log"
errfile="${logdir}AutoBuild${param1}.err"
unitylogfile="${logdir}Editor.log"
unitylogfile_="Users/a123/Library/Logs/Unity/Editor.log"
haslogfile=0
haserrfile=0
if [ -f "$logfile" ]; then
    haslogfile=1
fi
if [ -f "$errfile" ]; then
    haserrfile=1
fi

/usr/local/bin/sshpass -p test12345 ssh apple@127.0.0.1 "/Users/a123/Documents/cgg/trunk/tools/build.sh ${param1} > ${logfile} 2>${errfile}"

sleep 300
if test $haslogfile -eq 0; then 
svn add ${logfile}
fi

if test $haserrfile -eq 0; then 
svn add ${errfile}
fi

cp -f ${unitylogfile} ${unitylogfile_}
svn commit -m"auto build log" ${logfile} ${errfile} ${unitylogfile_}

