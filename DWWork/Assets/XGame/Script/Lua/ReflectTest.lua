--------------------------------------------------------------------------------
--   File      : ReflectTest.lua
--   author    : guoliang
--   function   : lua 调用C#反射测试
--   date      : 2018-1-14
--   copyright : Copyright 2017 DW Inc.
--------------------------------------------------------------------------------

ReflectTest = {}

function ReflectTest.Test()
	--获取对应类的静态对象实例
	local cnetSys = WrapSys.Reflection_GetProperty("CNetSys","TInstance",nil)
	--调用类的实例方法
    WrapSys.Reflection_CallMethodByObjName("PrintR", "CNetSys", "TInstance",{})
    --调用类的实例方法(带参数)
    WrapSys.Reflection_CallMethodByObjName("TestRSet", "CNetSys", "TInstance", {DwObject.New("what fuck")})
    --调用类的实例方法(带返回参数)
    local str = WrapSys.Reflection_CallMethodByObjName("GetR", "CNetSys", "TInstance",{})
    DwDebug.LogError("get test func " ..tostring(str))
    --设置类的实例对象的属性
    WrapSys.Reflection_SetField("CNetSys", "m_testStr", cnetSys, "set success")
    -- 获取类的实例对象的属性
    local str1 = WrapSys.Reflection_GetField("CNetSys", "m_testStr", cnetSys)
    DwDebug.LogError("get GetField " ..tostring(str1))

end