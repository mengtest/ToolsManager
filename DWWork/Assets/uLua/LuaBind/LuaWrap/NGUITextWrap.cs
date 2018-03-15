using System;
using UnityEngine;
using System.Text;
using LuaInterface;

public class NGUITextWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Update", Update),
		new LuaMethod("Prepare", Prepare),
		new LuaMethod("GetSymbol", GetSymbol),
		new LuaMethod("GetGlyphWidth", GetGlyphWidth),
		new LuaMethod("GetGlyph", GetGlyph),
		new LuaMethod("ParseAlpha", ParseAlpha),
		new LuaMethod("ParseColor", ParseColor),
		new LuaMethod("ParseColor24", ParseColor24),
		new LuaMethod("ParseColor32", ParseColor32),
		new LuaMethod("EncodeColor", EncodeColor),
		new LuaMethod("EncodeAlpha", EncodeAlpha),
		new LuaMethod("EncodeColor24", EncodeColor24),
		new LuaMethod("EncodeColor32", EncodeColor32),
		new LuaMethod("ParseSymbol", ParseSymbol),
		new LuaMethod("IsHex", IsHex),
		new LuaMethod("StripSymbols", StripSymbols),
		new LuaMethod("Align", Align),
		new LuaMethod("GetExactCharacterIndex", GetExactCharacterIndex),
		new LuaMethod("GetApproximateCharacterIndex", GetApproximateCharacterIndex),
		new LuaMethod("EndLine", EndLine),
		new LuaMethod("CalculatePrintedSize", CalculatePrintedSize),
		new LuaMethod("CalculateOffsetToFit", CalculateOffsetToFit),
		new LuaMethod("GetEndOfLineThatFits", GetEndOfLineThatFits),
		new LuaMethod("WrapText", WrapText),
		new LuaMethod("Print", Print),
		new LuaMethod("PrintApproximateCharacterPositions", PrintApproximateCharacterPositions),
		new LuaMethod("PrintExactCharacterPositions", PrintExactCharacterPositions),
		new LuaMethod("PrintCaretAndSelection", PrintCaretAndSelection),
		new LuaMethod("New", _CreateNGUIText),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("bitmapFont", get_bitmapFont, set_bitmapFont),
		new LuaField("dynamicFont", get_dynamicFont, set_dynamicFont),
		new LuaField("glyph", get_glyph, set_glyph),
		new LuaField("fontSize", get_fontSize, set_fontSize),
		new LuaField("fontScale", get_fontScale, set_fontScale),
		new LuaField("pixelDensity", get_pixelDensity, set_pixelDensity),
		new LuaField("fontStyle", get_fontStyle, set_fontStyle),
		new LuaField("alignment", get_alignment, set_alignment),
		new LuaField("tint", get_tint, set_tint),
		new LuaField("rectWidth", get_rectWidth, set_rectWidth),
		new LuaField("rectHeight", get_rectHeight, set_rectHeight),
		new LuaField("regionWidth", get_regionWidth, set_regionWidth),
		new LuaField("regionHeight", get_regionHeight, set_regionHeight),
		new LuaField("maxLines", get_maxLines, set_maxLines),
		new LuaField("gradient", get_gradient, set_gradient),
		new LuaField("gradientBottom", get_gradientBottom, set_gradientBottom),
		new LuaField("gradientTop", get_gradientTop, set_gradientTop),
		new LuaField("encoding", get_encoding, set_encoding),
		new LuaField("spacingX", get_spacingX, set_spacingX),
		new LuaField("spacingY", get_spacingY, set_spacingY),
		new LuaField("premultiply", get_premultiply, set_premultiply),
		new LuaField("symbolStyle", get_symbolStyle, set_symbolStyle),
		new LuaField("finalSize", get_finalSize, set_finalSize),
		new LuaField("finalSpacingX", get_finalSpacingX, set_finalSpacingX),
		new LuaField("finalLineHeight", get_finalLineHeight, set_finalLineHeight),
		new LuaField("baseline", get_baseline, set_baseline),
		new LuaField("useSymbols", get_useSymbols, set_useSymbols),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNGUIText(IntPtr L)
	{
		LuaDLL.luaL_error(L, "NGUIText class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(NGUIText));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "NGUIText", typeof(NGUIText), regs, fields, null);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_bitmapFont(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.bitmapFont);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dynamicFont(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.dynamicFont);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_glyph(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, NGUIText.glyph);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fontSize(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.fontSize);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fontScale(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.fontScale);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pixelDensity(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.pixelDensity);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_fontStyle(IntPtr L)
	{
		LuaScriptMgr.PushEnum(L, NGUIText.fontStyle);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_alignment(IntPtr L)
	{
		LuaScriptMgr.PushEnum(L, NGUIText.alignment);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_tint(IntPtr L)
	{
		LuaScriptMgr.PushValue(L, NGUIText.tint);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rectWidth(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.rectWidth);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rectHeight(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.rectHeight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_regionWidth(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.regionWidth);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_regionHeight(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.regionHeight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_maxLines(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.maxLines);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_gradient(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.gradient);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_gradientBottom(IntPtr L)
	{
		LuaScriptMgr.PushValue(L, NGUIText.gradientBottom);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_gradientTop(IntPtr L)
	{
		LuaScriptMgr.PushValue(L, NGUIText.gradientTop);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_encoding(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.encoding);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_spacingX(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.spacingX);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_spacingY(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.spacingY);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_premultiply(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.premultiply);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_symbolStyle(IntPtr L)
	{
		LuaScriptMgr.PushEnum(L, NGUIText.symbolStyle);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_finalSize(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.finalSize);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_finalSpacingX(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.finalSpacingX);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_finalLineHeight(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.finalLineHeight);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_baseline(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.baseline);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_useSymbols(IntPtr L)
	{
		LuaScriptMgr.Push(L, NGUIText.useSymbols);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_bitmapFont(IntPtr L)
	{
		NGUIText.bitmapFont = LuaScriptMgr.GetNetObject<UIFont>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dynamicFont(IntPtr L)
	{
		NGUIText.dynamicFont = LuaScriptMgr.GetNetObject<Font>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_glyph(IntPtr L)
	{
		NGUIText.glyph = LuaScriptMgr.GetNetObject<NGUIText.GlyphInfo>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fontSize(IntPtr L)
	{
		NGUIText.fontSize = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fontScale(IntPtr L)
	{
		NGUIText.fontScale = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pixelDensity(IntPtr L)
	{
		NGUIText.pixelDensity = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_fontStyle(IntPtr L)
	{
		NGUIText.fontStyle = LuaScriptMgr.GetNetObject<FontStyle>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_alignment(IntPtr L)
	{
		NGUIText.alignment = LuaScriptMgr.GetNetObject<NGUIText.Alignment>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_tint(IntPtr L)
	{
		NGUIText.tint = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_rectWidth(IntPtr L)
	{
		NGUIText.rectWidth = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_rectHeight(IntPtr L)
	{
		NGUIText.rectHeight = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_regionWidth(IntPtr L)
	{
		NGUIText.regionWidth = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_regionHeight(IntPtr L)
	{
		NGUIText.regionHeight = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_maxLines(IntPtr L)
	{
		NGUIText.maxLines = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_gradient(IntPtr L)
	{
		NGUIText.gradient = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_gradientBottom(IntPtr L)
	{
		NGUIText.gradientBottom = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_gradientTop(IntPtr L)
	{
		NGUIText.gradientTop = LuaScriptMgr.GetNetObject<Color>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_encoding(IntPtr L)
	{
		NGUIText.encoding = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_spacingX(IntPtr L)
	{
		NGUIText.spacingX = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_spacingY(IntPtr L)
	{
		NGUIText.spacingY = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_premultiply(IntPtr L)
	{
		NGUIText.premultiply = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_symbolStyle(IntPtr L)
	{
		NGUIText.symbolStyle = LuaScriptMgr.GetNetObject<NGUIText.SymbolStyle>(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_finalSize(IntPtr L)
	{
		NGUIText.finalSize = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_finalSpacingX(IntPtr L)
	{
		NGUIText.finalSpacingX = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_finalLineHeight(IntPtr L)
	{
		NGUIText.finalLineHeight = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_baseline(IntPtr L)
	{
		NGUIText.baseline = (float)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_useSymbols(IntPtr L)
	{
		NGUIText.useSymbols = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			NGUIText.Update();
			return 0;
		}
		else if (count == 1)
		{
			bool arg0 = LuaScriptMgr.GetBoolean(L, 1);
			NGUIText.Update(arg0);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUIText.Update");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Prepare(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		NGUIText.Prepare(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSymbol(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		BMSymbol o = NGUIText.GetSymbol(arg0,arg1,arg2);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetGlyphWidth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		float o = NGUIText.GetGlyphWidth(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetGlyph(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		NGUIText.GlyphInfo o = NGUIText.GetGlyph(arg0,arg1);
		LuaScriptMgr.PushObject(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParseAlpha(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		float o = NGUIText.ParseAlpha(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParseColor(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		Color o = NGUIText.ParseColor(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParseColor24(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		Color o = NGUIText.ParseColor24(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParseColor32(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		Color o = NGUIText.ParseColor32(arg0,arg1);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EncodeColor(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 1)
		{
			Color arg0 = LuaScriptMgr.GetNetObject<Color>(L, 1);
			string o = NGUIText.EncodeColor(arg0);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			Color arg1 = LuaScriptMgr.GetNetObject<Color>(L, 2);
			string o = NGUIText.EncodeColor(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUIText.EncodeColor");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EncodeAlpha(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		float arg0 = (float)LuaScriptMgr.GetNumber(L, 1);
		string o = NGUIText.EncodeAlpha(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EncodeColor24(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Color arg0 = LuaScriptMgr.GetNetObject<Color>(L, 1);
		string o = NGUIText.EncodeColor24(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EncodeColor32(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Color arg0 = LuaScriptMgr.GetNetObject<Color>(L, 1);
		string o = NGUIText.EncodeColor32(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ParseSymbol(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 2)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int arg1 = LuaScriptMgr.GetNetObject<int>(L, 2);
			bool o = NGUIText.ParseSymbol(arg0,ref arg1);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg1);
			return 2;
		}
		else if (count == 10)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			int arg1 = LuaScriptMgr.GetNetObject<int>(L, 2);
			BetterList<Color> arg2 = LuaScriptMgr.GetNetObject<BetterList<Color>>(L, 3);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
			int arg4 = LuaScriptMgr.GetNetObject<int>(L, 5);
			bool arg5 = LuaScriptMgr.GetNetObject<bool>(L, 6);
			bool arg6 = LuaScriptMgr.GetNetObject<bool>(L, 7);
			bool arg7 = LuaScriptMgr.GetNetObject<bool>(L, 8);
			bool arg8 = LuaScriptMgr.GetNetObject<bool>(L, 9);
			bool arg9 = LuaScriptMgr.GetNetObject<bool>(L, 10);
			bool o = NGUIText.ParseSymbol(arg0,ref arg1,arg2,arg3,ref arg4,ref arg5,ref arg6,ref arg7,ref arg8,ref arg9);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg1);
			LuaScriptMgr.Push(L, arg4);
			LuaScriptMgr.Push(L, arg5);
			LuaScriptMgr.Push(L, arg6);
			LuaScriptMgr.Push(L, arg7);
			LuaScriptMgr.Push(L, arg8);
			LuaScriptMgr.Push(L, arg9);
			return 8;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUIText.ParseSymbol");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsHex(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		char arg0 = (char)LuaScriptMgr.GetNumber(L, 1);
		bool o = NGUIText.IsHex(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StripSymbols(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = NGUIText.StripSymbols(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Align(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		float arg2 = (float)LuaScriptMgr.GetNumber(L, 3);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 4);
		NGUIText.Align(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetExactCharacterIndex(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 1);
		BetterList<int> arg1 = LuaScriptMgr.GetNetObject<BetterList<int>>(L, 2);
		Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		int o = NGUIText.GetExactCharacterIndex(arg0,arg1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetApproximateCharacterIndex(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		BetterList<Vector3> arg0 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 1);
		BetterList<int> arg1 = LuaScriptMgr.GetNetObject<BetterList<int>>(L, 2);
		Vector2 arg2 = LuaScriptMgr.GetNetObject<Vector2>(L, 3);
		int o = NGUIText.GetApproximateCharacterIndex(arg0,arg1,arg2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EndLine(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		StringBuilder arg0 = LuaScriptMgr.GetNetObject<StringBuilder>(L, 1);
		NGUIText.EndLine(ref arg0);
		LuaScriptMgr.PushObject(L, arg0);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculatePrintedSize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		Vector2 o = NGUIText.CalculatePrintedSize(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CalculateOffsetToFit(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int o = NGUIText.CalculateOffsetToFit(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetEndOfLineThatFits(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		string o = NGUIText.GetEndOfLineThatFits(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WrapText(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetNetObject<string>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			bool o = NGUIText.WrapText(arg0,out arg1,arg2);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg1);
			return 2;
		}
		else if (count == 5)
		{
			string arg0 = LuaScriptMgr.GetLuaString(L, 1);
			string arg1 = LuaScriptMgr.GetNetObject<string>(L, 2);
			bool arg2 = LuaScriptMgr.GetBoolean(L, 3);
			bool arg3 = LuaScriptMgr.GetBoolean(L, 4);
			bool arg4 = LuaScriptMgr.GetBoolean(L, 5);
			bool o = NGUIText.WrapText(arg0,out arg1,arg2,arg3,arg4);
			LuaScriptMgr.Push(L, o);
			LuaScriptMgr.Push(L, arg1);
			return 2;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: NGUIText.WrapText");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Print(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 4);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		BetterList<Vector3> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<Vector2> arg2 = LuaScriptMgr.GetNetObject<BetterList<Vector2>>(L, 3);
		BetterList<Color32> arg3 = LuaScriptMgr.GetNetObject<BetterList<Color32>>(L, 4);
		NGUIText.Print(arg0,arg1,arg2,arg3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PrintApproximateCharacterPositions(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		BetterList<Vector3> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<int> arg2 = LuaScriptMgr.GetNetObject<BetterList<int>>(L, 3);
		NGUIText.PrintApproximateCharacterPositions(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PrintExactCharacterPositions(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		BetterList<Vector3> arg1 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 2);
		BetterList<int> arg2 = LuaScriptMgr.GetNetObject<BetterList<int>>(L, 3);
		NGUIText.PrintExactCharacterPositions(arg0,arg1,arg2);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PrintCaretAndSelection(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		string arg0 = LuaScriptMgr.GetLuaString(L, 1);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 2);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 3);
		BetterList<Vector3> arg3 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 4);
		BetterList<Vector3> arg4 = LuaScriptMgr.GetNetObject<BetterList<Vector3>>(L, 5);
		NGUIText.PrintCaretAndSelection(arg0,arg1,arg2,arg3,arg4);
		return 0;
	}
}

