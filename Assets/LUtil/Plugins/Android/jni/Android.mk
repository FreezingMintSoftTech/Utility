LOCAL_PATH := $(call my-dir)

##################################
# md5
include $(CLEAR_VARS)

LOCAL_MODULE := xxHash
LOCAL_SRC_FILES:= ../../iOS/xxHash.c\

#LOCAL_CPPFLAGS := -D_DEBUG
LOCAL_C_INCLUDES := $(APP_C_INCLUDES)
#LOCAL_LDLIBS    := -llog
include $(BUILD_SHARED_LIBRARY)

