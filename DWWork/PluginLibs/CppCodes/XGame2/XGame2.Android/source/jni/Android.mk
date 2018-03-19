# Copyright (C) 2009 The Android Open Source Project
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
APP_CPPFLAGS 	:= -frtti -std=c++11 
LOCAL_MODULE    := XGame2

LOCAL_CFLAGS    := -Werror
LOCAL_LDLIBS	:= -llog -lGLESv2 -landroid
APP_CPPFLAGS 	:= -frtti -std=c++11
LOCAL_SRC_FILES := 	../../../XGame2/ActorAttribute.cpp \
					../../../XGame2/ActorAttribute.h \
					../../../XGame2/FastEvent.h \
					../../../XGame2/FastEventImplement.h \
					../../../XGame2/Main.cpp \
					../../../XGame2/Main.h \
					../../../XGame2/PreInclude.h \
					../../../XGame2/tea.cpp	\
					../../../XGame2/tea.h \
					../../../XGame2/Utilities.cpp \
					../../../XGame2/Utilities.h \
					../../../XGame2/VariadicMacros.h \
					../../../XGame2/EZFunBitCoverter.h \
					../../../XGame2/EZFunBitCoverter.cpp \
					../../../XGame2/Bounds.h \
					../../../XGame2/Bounds.cpp \
					../../../XGame2/SceneNodeDistanceRefresh.h \
					../../../XGame2/SceneNodeDistanceRefresh.cpp 

include $(BUILD_SHARED_LIBRARY)
