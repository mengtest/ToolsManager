:: ����Cmake·��
path   %path%;D:\ProgramSoft\cmak361\bin
:: ����mingw��·��
:: ��������64λmingw·��
path   %path%;D:\ProgramSoft\mingw64\mingw64\bin
set Plat=x86_64
:: ��������32λmingw·��
:: path   %path%;D:\ProgramSoft\mingw32\mingw32\bin
::set Plat=x86
cmake clean
cmake . -G"MinGW Makefiles"
make clean
make
copy "./libXGame2.dll" "../Libs/Win/%Plat%/XGame2.dll"
pause