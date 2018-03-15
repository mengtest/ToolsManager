#set(CMAKE_C_FLAGS “{CMAKE_C_FLAGS} -arch armv7 arm64”)
#set(CMAKE_CXX_FLAGS “{CMAKE_CXX_FLAGS} -arch armv7 arm64”)
#set(CMAKE_OSX_DEPLOYMENT_TARGET 6.0)
#set(CMAKE_XCODE_ATTRIBUTE_IPHONEOS_DEPLOYMENT_TARGET 6.0)
#set(CMAKE_OSX_SYSROOT iphoneos)
#set(CMAKE_XCODE_EFFECTIVE_PLATFORMS “-iphoneos”)
#set(CMAKE_SUPPRESS_REGENERATION true)

cmake_minimum_required(VERSION 3.2)

project(XGame2)

file(GLOB_RECURSE XGame2_source_files ${XGame2_SOURCE_DIR}/../XGame2/*.cpp)

set_target_properties(${XGame2} PROPERTIES XCODE_ATTRIBUTE_PRODUCT_NAME XGAME)

add_library(XGame2 STATIC ${XGame2_source_files})

