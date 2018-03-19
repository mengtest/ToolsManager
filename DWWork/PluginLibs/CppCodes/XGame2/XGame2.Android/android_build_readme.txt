android编译需要ndk环境，而且是ndk11以上(部分代码用了c++11的语法)

win如下
步骤如下:
	1 cmd 切换路径到本文件夹下source/jni/路径
	2 修改application.mk文件的NDK_TOOLCHAIN_VERSION属性修改为自己ndk的toolchain的版本，比如我当前ndk版本是r11，我的ndk/toochains/下面的路径都是4.9
	3 找到ndk-build.cmd拖拽到cmd中 回车即可
	4 在source的libs下面找到对应的.so文件拷贝到plugins下

