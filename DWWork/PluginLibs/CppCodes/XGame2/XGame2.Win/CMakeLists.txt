cmake_minimum_required(VERSION 3.2)
#SET(GCC_COMPILER_PATH "D:/Program Files/mingw-w64/i686-6.2.0-win32-dwarf-rt_v5-rev0/mingw32/bin")
#SET(GCC_COMPILER_PATH "D:/ProgramSoft/MinGW64/mingw64/bin")
#SET(CMAKE_C_COMPILER ${GCC_COMPILER_PATH}/gcc.exe)
#SET(CMAKE_CXX_COMPILER ${GCC_COMPILER_PATH}/g++.exe)
#SET(CMAKE_MAKE_PROGRAM ${GCC_COMPILER_PATH}/make.exe)

project(XGame2)

set(CMAKE_BUILD_TYPE "Release")

SET(CMAKE_CXX_FLAGS "-std=c++0x")

set(CMAKE_SHARED_LIBRARY_PRWEFIX "")

file(GLOB_RECURSE XGame2_source_files ${XGame2_SOURCE_DIR}/../XGame2/*.cpp)

add_library(XGame2 SHARED ${XGame2_source_files})

target_link_libraries(XGame2 -static-libgcc -static-libstdc++ -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic)
