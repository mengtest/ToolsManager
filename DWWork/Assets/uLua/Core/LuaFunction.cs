using System;
using System.Collections.Generic;
using System.Text;

namespace LuaInterface
{
    public class LuaFunction : LuaBase
    {        
        internal LuaCSFunction function;        
        ObjectTranslator translator = null;
        IntPtr L;        
       
        public LuaFunction(int reference, LuaState interpreter)
        {
            _Reference = reference;
            this.function = null;
            _Interpreter = interpreter;
            L = _Interpreter.L;
            translator = _Interpreter.translator;            
        }

        public LuaFunction(LuaCSFunction function, LuaState interpreter)
        {
            _Reference = 0;
            this.function = function;
            _Interpreter = interpreter;
            L = _Interpreter.L;
            translator = _Interpreter.translator;            
        }

        public LuaFunction(int reference, IntPtr l)
        {
            _Reference = reference;
            this.function = null;             
            L = l;
            translator = ObjectTranslator.FromState(L);
            _Interpreter = translator.interpreter;            
        }

        /*
         * Calls the function casting return values to the types
         * in returnTypes
         */
        internal object[] call(object[] args, Type[] returnTypes)
        {            
            int nArgs = 0;
            LuaDLL.lua_getglobal(L, "traceback");            
            int oldTop = LuaDLL.lua_gettop(L);

            if (!LuaDLL.lua_checkstack(L, args.Length + 6))
            {
                LuaDLL.lua_pop(L, 1);
                throw new LuaException("Lua stack overflow");
            }
               
            push(L);

            if (args != null)
            {
                nArgs = args.Length;

                for (int i = 0; i < args.Length; i++)
                {                    
                    if (args[i] == null)
                    {
                        LuaDLL.lua_pushnil(L);
                    }
                    else
                    {
                        PushArgs(L, args[i]);
                    }
                }
            }

            int error = LuaDLL.lua_pcall(L, nArgs, -1, -nArgs-2);            

            if (error != 0)
            {
                string err = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_settop(L, oldTop);                
                LuaDLL.lua_pop(L, 1);
                if (err == null) err = "Unknown Lua Error";
                throw new LuaScriptException(err.ToString(), "");                              
            }

            object[] ret = returnTypes != null ? translator.popValues(L, oldTop, returnTypes) : translator.popValues(L, oldTop);
            LuaDLL.lua_pop(L, 1);            
            return ret;
        }       

        /*
         * Calls the function and returns its return values inside
         * an array
         */
        public object[] Call(params object[] args)
        {
            return call(args, null);
        }

       public int BeginPCall()
        {
            LuaDLL.lua_getglobal(L, "traceback");
            int oldTop = LuaDLL.lua_gettop(L);
            push(L);
            return oldTop;
        }

       public bool PCall(int oldTop, int args)
        {
            if (LuaDLL.lua_pcall(L, args, -1, -args - 2) != 0)
            {
                string err = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_settop(L, oldTop);
                LuaDLL.lua_pop(L, 1);
                if (err == null) err = "Unknown Lua Error";
                throw new LuaScriptException(err.ToString(), "");
            }

            return true;
        }

        public  object[] EndPCall(int oldTop)
        {
            object[] ret = translator.popValues(L, oldTop);
            LuaDLL.lua_pop(L, 1);
            return ret;
        }
        public IntPtr GetLuaState()
        {
            return L;
        }

        /// <summary>
        ///  取消注释，消除因为变长数组产生的gc
        /// </summary>
        /// <returns></returns>
        public object[] Call()
        {
            int oldTop = BeginPCall();

            if (PCall(oldTop, 0))
            {
                return EndPCall(oldTop);
            }

            return null;
        }
        /// <summary>
        ///  取消注释，消除因为变长数组产生的gc
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t1"></param>
        /// <returns></returns>
        public object[] Call1Args(object t1)
        {
            int oldTop = BeginPCall();

            if (t1 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t1);
            }
            if (PCall(oldTop, 1))
            {
                return EndPCall(oldTop);
            }

            return null;
        }
        /// <summary>
        ///  取消注释，消除因为变长数组产生的gc
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public object[] Call2Args(object t1, object t2)
        {
            int oldTop = BeginPCall();

            if (t1 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t1);
            }
            if (t2 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t2);
            }
            if (PCall(oldTop, 2))
            {
                return EndPCall(oldTop);
            }

            return null;
        }
        /// <summary>
        /// 取消注释，消除因为变长数组产生的gc
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <returns></returns>
        public object[] Call3Args(object t1, object t2, object t3)
        {
            int oldTop = BeginPCall();

            if (t1 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t1);
            }
            if (t2 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t2);
            }
            if (t3 == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                PushArgs(L, t3);
            }

            if (PCall(oldTop, 3))
            {
                return EndPCall(oldTop);
            }

            return null;
        }




        //public object[] Call<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        //{
        //    int oldTop = BeginPCall();

        //    PushArgs(L, t1);
        //    PushArgs(L, t2);
        //    PushArgs(L, t3);
        //    PushArgs(L, t4);

        //    if (PCall(oldTop, 4))
        //    {
        //        return EndPCall(oldTop);
        //    }

        //    return null;
        //}

            /// <summary>
            /// 针对发包到lua优化
            /// </summary>
            /// <param name="t1"></param>
            /// <param name="t2"></param>
            /// <param name="t3"></param>
            /// <param name="t4"></param>
            /// <param name="t5"></param>
            /// <returns></returns>
        public object[] SendNetMessage(string t1,int t2, int t3, int t4,int t5, bool t6, byte[] t7)
        {
            int oldTop = BeginPCall();
            LuaScriptMgr.Push(L, t1);
            LuaScriptMgr.Push(L, t2);
            LuaScriptMgr.Push(L, t3);
            LuaScriptMgr.Push(L, t4);
            LuaScriptMgr.Push(L,t5);
            LuaScriptMgr.Push(L, t6);
            if (t7 != null && t7.Length != 0)
            {
                LuaDLL.lua_pushlstring(L, t7, t7.Length);
                //var str = new string(t5.buffer);
            }
            else
            {
                LuaDLL.lua_pushnil(L);
            }
            if (PCall(oldTop, 7))
            {
                return EndPCall(oldTop);
            }

            return null;
        }


        public object[] SendHttpNetMessage(string msgKey, int cmd,int error,int seq,byte[] body)
        {
            int oldTop = BeginPCall();
            LuaScriptMgr.Push(L, msgKey);
            LuaScriptMgr.Push(L, cmd);
            LuaScriptMgr.Push(L, error);
            LuaScriptMgr.Push(L, seq);
            if (body != null && body.Length != 0)
            {
                LuaDLL.lua_pushlstring(L, body, body.Length);
            }
            else
            {
                LuaDLL.lua_pushnil(L);
            }
            if (PCall(oldTop,5))
            {
                return EndPCall(oldTop);
            }

            return null;
        }

        void PushArgs(IntPtr L, object o)
        {
            Type type = o.GetType();

            if (type.IsArray)
            {
                LuaScriptMgr.PushArray(L, o);
            }
            else if (type.IsEnum)
            {
                LuaScriptMgr.PushEnum(L, o);
            }
            else
            {
                translator.push(L, o);
            }
        }

        /*
         * Pushes the function into the Lua stack
         */
        internal void push(IntPtr luaState)
        {
            if (_Reference != 0)
            {
                LuaDLL.lua_getref(luaState, _Reference);
            }
            else
            {
                _Interpreter.pushCSFunction(function);
            }
        }

        public override string ToString()
        {
            return "function";
        }

        public override bool Equals(object o)
        {
            if (o is LuaFunction)
            {
                LuaFunction l = (LuaFunction)o;
                if (this._Reference != 0 && l._Reference != 0)
                    return _Interpreter.compareRef(l._Reference, this._Reference);
                else
                    return this.function == l.function;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            if (_Reference != 0)
                return _Reference;
            else
                return function.GetHashCode();
        }
    }

}
