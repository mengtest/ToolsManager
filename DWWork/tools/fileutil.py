import os
import sys
import stat
from os.path import abspath
import fnmatch
import collections
import errno
import shutil

def file_copytree(src, dst, isResuise = True):
    names = os.listdir(src)
    if not os.path.exists(dst):
        os.makedirs(dst)
    errors = []
    for name in names:
        srcname = os.path.join(src, name)
        dstname = os.path.join(dst, name)
        #try:
        if os.path.isdir(srcname):
            if isResuise:
                file_copytree(srcname, dstname, isResuise)
        else:
                # Will raise a SpecialFileError for unsupported file types
            shutil.copy2(srcname, dstname)
        # catch the Error from the recursive copytree so that we can
        # continue with other files
        #except Error, err:
        #    errors.extend(err.args[0])
        #except EnvironmentError, why:
        #    errors.append((srcname, dstname, str(why)))
    try:
        shutil.copystat(src, dst)
    except OSError, why:
        if WindowsError is not None and isinstance(why, WindowsError):
            # Copying file access times may fail on Windows
            pass
        else:
            errors.extend((src, dst, str(why)))
    if errors:
        raise Error, errors