#APP_MODULES :=  libSubA
APP_OPTIM := release
#APP_STL := stlport_static
#APP_ABI := all
#APP_ABI := armeabi armeabi-v7a x86 mips
APP_ABI := armeabi-v7a x86 
NDK_TOOLCHAIN_VERSION := 4.9
APP_CPPFLAGS := -frtti -std=c++11 
APP_STL := gnustl_static