cmake_minimum_required(VERSION 3.2)
SET(ANDROID_NDK "D:/Android/android-ndk-r10e")
SET(GCC_COMPILER_PATH "D:/Program Files/mingw-w64/x86_64-6.2.0-win32-seh-rt_v5-rev0/mingw64/bin/")
SET(CMAKE_MAKE_PROGRAM ${GCC_COMPILER_PATH}/make.exe)
SET(ANDROID_NATIVE_API_LEVEL 15)

project(XGame2)

set(CMAKE_BUILD_TYPE "Release")

file(GLOB_RECURSE XGame2_source_files ${XGame2_SOURCE_DIR}/../XGame2/*.cpp)

add_library(XGame2 SHARED ${XGame2_source_files})

#target_link_libraries(XGame2 -shared-libgcc -shared-libstdc++)
