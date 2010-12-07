//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.Text
{
	#region Enum SupportedEncoding

	/// <summary>
	/// Encodings that are supported by .NET.
	/// </summary>
	/// <remarks>
	/// Enum value corresponds to code page.
	/// </remarks>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "IDs of encodings are given by the according encoding standards.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Text is given by the MSDN.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Text is given by the MSDN.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1632:DocumentationTextMustMeetMinimumCharacterLength", Justification = "Text is given by the MSDN.")]
	public enum SupportedEncoding
	{
		//------------------------------------------------------------------------------------------
		// ASCII and Unicode
		//------------------------------------------------------------------------------------------

		/// <summary>US-ASCII.</summary>
		ASCII = 20127,

		/// <summary>Unicode (UTF-7).</summary>
		UTF7 = 65000,

		/// <summary>Unicode (UTF-8).</summary>
		UTF8 = 65001,

		/// <summary>Unicode.</summary>
		UTF16 = 1200,

		/// <summary>Unicode (Big-Endian).</summary>
		UTF16BE = 1201,

		/// <summary>Unicode (UTF-32).</summary>
		UTF32 = 65005,

		/// <summary>Unicode (UTF-32 Big-Endian).</summary>
		UTF32BE = 65006,

		//------------------------------------------------------------------------------------------
		// ISO
		//------------------------------------------------------------------------------------------

		/// <summary>Western European (ISO).</summary>
		ISO8859_1 = 28591,

		/// <summary>Central European (ISO).</summary>
		ISO8859_2 = 28592,

		/// <summary>Latin 3 (ISO).</summary>
		ISO8859_3 = 28593,

		/// <summary>Baltic (ISO).</summary>
		ISO8859_4 = 28594,

		/// <summary>Cyrillic (ISO).</summary>
		ISO8859_5 = 28595,

		/// <summary>Arabic (ISO).</summary>
		ISO8859_6 = 28596,

		/// <summary>Greek (ISO).</summary>
		ISO8859_7 = 28597,

		/// <summary>Hebrew (ISO-Visual).</summary>
		ISO8859_8 = 28598,

		/// <summary>Hebrew (ISO-Logical).</summary>
		ISO8859_8I = 38598,

		/// <summary>Turkish (ISO).</summary>
		ISO8859_9 = 28599,

		/// <summary>Estonian (ISO).</summary>
		ISO8859_13 = 28603,

		/// <summary>Latin 9 (ISO).</summary>
		ISO8859_15 = 28605,

		/// <summary>Japanese (JIS).</summary>
		ISO2022JP = 50220,

		/// <summary>Japanese (JIS-Allow 1 byte Kana).</summary>
		CSISO2022JP = 50221,

		/// <summary>Japanese (JIS-Allow 1 byte Kana - SO/SI).</summary>
		ISO2022JP_A = 50222,

		/// <summary>Korean (ISO).</summary>
		ISO2022KR = 50225,

		//------------------------------------------------------------------------------------------
		// Windows
		//------------------------------------------------------------------------------------------

		/// <summary>Western European (Windows).</summary>
		Windows1252 = 1252,

		/// <summary>Central European (Windows).</summary>
		Windows1250 = 1250,

		/// <summary>Cyrillic (Windows).</summary>
		Windows1251 = 1251,

		/// <summary>Greek (Windows).</summary>
		Windows1253 = 1253,

		/// <summary>Turkish (Windows).</summary>
		Windows1254 = 1254,

		/// <summary>Hebrew (Windows).</summary>
		Windows1255 = 1255,

		/// <summary>Arabic (Windows).</summary>
		Windows1256 = 1256,

		/// <summary>Baltic (Windows).</summary>
		Windows1257 = 1257,

		/// <summary>Vietnamese (Windows).</summary>
		Windows1258 = 1258,

		/// <summary>Thai (Windows).</summary>
		Windows874 = 874,

		//------------------------------------------------------------------------------------------
		// Mac
		//------------------------------------------------------------------------------------------

		/// <summary>Western European (Mac).</summary>
		Macintosh = 10000,

		/// <summary>Central European (Mac).</summary>
		XMacCE = 10029,

		/// <summary>Japanese (Mac).</summary>
		XMacJapanese = 10001,

		/// <summary>Chinese Traditional (Mac).</summary>
		XMacChineseTrad = 10002,

		/// <summary>Chinese Simplified (Mac).</summary>
		XMacChineseSimp = 10008,

		/// <summary>Korean (Mac).</summary>
		XMacKorean = 10003,

		/// <summary>Arabic (Mac).</summary>
		XMacArabic = 10004,

		/// <summary>Hebrew (Mac).</summary>
		XMacHebrew = 10005,

		/// <summary>Greek (Mac).</summary>
		XMacGreek = 10006,

		/// <summary>Cyrillic (Mac).</summary>
		XMacCyrillic = 10007,

		/// <summary>Romanian (Mac).</summary>
		XMacRomanian = 10010,

		/// <summary>Ukrainian (Mac).</summary>
		XMacUkrainian = 10017,

		/// <summary>Thai (Mac).</summary>
		XMacThai = 10021,

		/// <summary>Icelandic (Mac).</summary>
		XMacIcelandic = 10079,

		/// <summary>Turkish (Mac).</summary>
		XMacTurkish = 10081,

		/// <summary>Croatian (Mac).</summary>
		XMacCroatian = 10082,

		//------------------------------------------------------------------------------------------
		// Unix
		//------------------------------------------------------------------------------------------

		/// <summary>Japanese (JIS 0208-1990 and 0212-1990).</summary>
		EUC_JP = 20932,

		/// <summary>Japanese (EUC).</summary>
		EUC_JP_A = 51932,

		/// <summary>Chinese Simplified (EUC).</summary>
		EUC_CN = 51936,

		/// <summary>Korean (EUC).</summary>
		EUC_KR = 51949,

		//------------------------------------------------------------------------------------------
		// IBM EBCDIC
		//------------------------------------------------------------------------------------------

		/// <summary>IBM Latin-1.</summary>
		IBM1047 = 1047,

		/// <summary>IBM Latin-1.</summary>
		IBM924 = 20924,

		/// <summary>IBM EBCDIC (International).</summary>
		IBM500 = 500,

		/// <summary>IBM EBCDIC (US-Canada).</summary>
		IBM037 = 37,

		/// <summary>IBM EBCDIC (Multilingual Latin-2).</summary>
		IBM870 = 870,

		/// <summary>IBM EBCDIC (Greek).</summary>
		IBM423 = 20423,

		/// <summary>IBM EBCDIC (Greek Modern).</summary>
		CP875 = 875,

		/// <summary>IBM EBCDIC (Cyrillic Russian).</summary>
		IBM880 = 20880,

		/// <summary>IBM EBCDIC (Cyrillic Serbian-Bulgarian).</summary>
		CP1025 = 21025,

		/// <summary>IBM EBCDIC (Turkish).</summary>
		IBM905 = 20905,

		/// <summary>IBM EBCDIC (Turkish Latin-5).</summary>
		IBM1026 = 1026,

		/// <summary>IBM EBCDIC (US-Canada-Euro).</summary>
		IBM1140 = 1140,

		/// <summary>IBM EBCDIC (Germany-Euro).</summary>
		IBM1141 = 1141,

		/// <summary>IBM EBCDIC (Denmark-Norway-Euro).</summary>
		IBM1142 = 1142,

		/// <summary>IBM EBCDIC (Finland-Sweden-Euro).</summary>
		IBM1143 = 1143,

		/// <summary>IBM EBCDIC (Italy-Euro).</summary>
		IBM1144 = 1144,

		/// <summary>IBM EBCDIC (Spain-Euro).</summary>
		IBM1145 = 1145,

		/// <summary>IBM EBCDIC (UK-Euro).</summary>
		IBM1146 = 1146,

		/// <summary>IBM EBCDIC (France-Euro).</summary>
		IBM1147 = 1147,

		/// <summary>IBM EBCDIC (International-Euro).</summary>
		IBM1148 = 1148,

		/// <summary>IBM EBCDIC (Icelandic-Euro).</summary>
		IBM1149 = 1149,

		/// <summary>IBM EBCDIC (Germany).</summary>
		IBM273 = 20273,

		/// <summary>IBM EBCDIC (Denmark-Norway).</summary>
		IBM277 = 20277,

		/// <summary>IBM EBCDIC (Finland-Sweden).</summary>
		IBM278 = 20278,

		/// <summary>IBM EBCDIC (Icelandic).</summary>
		IBM871 = 20871,

		/// <summary>IBM EBCDIC (Italy).</summary>
		IBM280 = 20280,

		/// <summary>IBM EBCDIC (Spain).</summary>
		IBM284 = 20284,

		/// <summary>IBM EBCDIC (UK).</summary>
		IBM285 = 20285,

		/// <summary>IBM EBCDIC (France).</summary>
		IBM297 = 20297,

		/// <summary>IBM EBCDIC (Arabic).</summary>
		IBM420 = 20420,

		/// <summary>IBM EBCDIC (Hebrew).</summary>
		IBM424 = 20424,

		/// <summary>IBM EBCDIC (Japanese katakana).</summary>
		IBM290 = 20290,

		/// <summary>IBM EBCDIC (Thai).</summary>
		IBMThai = 20838,

		/// <summary>IBM EBCDIC (Korean Extended).</summary>
		X_EBCDIC_KoreanExtended = 20833,

		//------------------------------------------------------------------------------------------
		// IBM OEM
		//------------------------------------------------------------------------------------------

		/// <summary>OEM United States.</summary>
		IBM437 = 437,

		/// <summary>OEM Cyrillic.</summary>
		IBM855 = 855,

		/// <summary>OEM Multilingual Latin I.</summary>
		IBM858 = 858,

		//------------------------------------------------------------------------------------------
		// DOS
		//------------------------------------------------------------------------------------------

		/// <summary>Western European (DOS).</summary>
		IBM850 = 850,

		/// <summary>Central European (DOS).</summary>
		IBM852 = 852,

		/// <summary>Greek (DOS).</summary>
		IBM737 = 737,

		/// <summary>Greek, Modern (DOS).</summary>
		IBM869 = 869,

		/// <summary>Baltic (DOS).</summary>
		IBM775 = 775,

		/// <summary>Cyrillic (DOS).</summary>
		CP866 = 866,

		/// <summary>Portuguese (DOS).</summary>
		IBM860 = 860,

		/// <summary>Icelandic (DOS).</summary>
		IBM861 = 861,

		/// <summary>French Canadian (DOS).</summary>
		IBM863 = 863,

		/// <summary>Nordic (DOS).</summary>
		IBM865 = 865,

		/// <summary>Turkish (DOS).</summary>
		IBM857 = 857,

		/// <summary>Arabic (DOS).</summary>
		DOS720 = 720,

		/// <summary>Arabic (864).</summary>
		IBM864 = 864,

		/// <summary>Hebrew (DOS).</summary>
		DOS862 = 862,

		//------------------------------------------------------------------------------------------
		// Misc
		//------------------------------------------------------------------------------------------

		/// <summary>Cyrillic (KOI8-R).</summary>
		KOI8_R = 20866,

		/// <summary>Cyrillic (KOI8-U).</summary>
		KOI8_U = 21866,

		/// <summary>Arabic (ASMO 708).</summary>
		ASMO_708 = 708,

		/// <summary>Japanese (Shift-JIS).</summary>
		Shift_JIS = 932,

		/// <summary>Chinese Simplified (GB2312).</summary>
		GB2312 = 936,

		/// <summary>Chinese Simplified (GB18030).</summary>
		GB18030 = 54936,

		/// <summary>Chinese Simplified (HZ).</summary>
		HZ_GB_2312 = 52936,

		/// <summary>Chinese Traditional (Big5).</summary>
		Big5 = 950,

		/// <summary>Chinese Simplified (GB2312-80).</summary>
		X_CP20936 = 20936,

		/// <summary>Korean Wansung.</summary>
		X_CP20949 = 20949,

		/// <summary>Korean.</summary>
		KS_C_5601_1987 = 949,

		/// <summary>Korean (Johab).</summary>
		Johab = 1361,

		//------------------------------------------------------------------------------------------
		// X
		//------------------------------------------------------------------------------------------

		/// <summary>Europa.</summary>
		X_Europa = 29001,

		/// <summary>Chinese Traditional (CNS).</summary>
		X_ChineseCNS = 20000,

		/// <summary>Chinese Traditional (Eten).</summary>
		X_ChineseEten = 20002,

		/// <summary>Western European (IA5).</summary>
		X_IA5 = 20105,

		/// <summary>German (IA5).</summary>
		X_IA5_German = 20106,

		/// <summary>Swedish (IA5).</summary>
		X_IA5_Swedish = 20107,

		/// <summary>Norwegian (IA5).</summary>
		X_IA5_Norwegian = 20108,

		/// <summary>TCA Taiwan.</summary>
		X_CP20001 = 20001,

		/// <summary>IBM5550 Taiwan.</summary>
		X_CP20003 = 20003,

		/// <summary>TeleText Taiwan.</summary>
		X_CP20004 = 20004,

		/// <summary>Wang Taiwan.</summary>
		X_CP20005 = 20005,

		/// <summary>Chinese Simplified (ISO-2022).</summary>
		X_CP50227 = 50227,

		/// <summary>T.61.</summary>
		X_CP20261 = 20261,

		/// <summary>ISO-6937.</summary>
		X_CP20269 = 20269,

		//------------------------------------------------------------------------------------------
		// X-ISCII
		//------------------------------------------------------------------------------------------

		/// <summary>ISCII Devanagari.</summary>
		X_ISCII_DE = 57002,

		/// <summary>ISCII Bengali.</summary>
		X_ISCII_BE = 57003,

		/// <summary>ISCII Tamil.</summary>
		X_ISCII_TA = 57004,

		/// <summary>ISCII Telugu.</summary>
		X_ISCII_TE = 57005,

		/// <summary>ISCII Assamese.</summary>
		X_ISCII_AS = 57006,

		/// <summary>ISCII Oriya.</summary>
		X_ISCII_OR = 57007,

		/// <summary>ISCII Kannada.</summary>
		X_ISCII_KA = 57008,

		/// <summary>ISCII Malayalam.</summary>
		X_ISCII_MA = 57009,

		/// <summary>ISCII Gujarati.</summary>
		X_ISCII_GU = 57010,

		/// <summary>ISCII Punjabi.</summary>
		X_ISCII_PA = 57011,
	}

	#endregion

	/// <summary>
	/// Extended enum XEncoding.
	/// </summary>
	public class EncodingEx : EnumEx
	{
		private struct EncodingInfoEx
		{
			public SupportedEncoding SupportedEncoding;
			public string BetterDisplayName;
			public Encoding Encoding;

			public EncodingInfoEx(SupportedEncoding supportedEncoding, string betterDisplayName, Encoding encoding)
			{
				SupportedEncoding = supportedEncoding;
				BetterDisplayName = betterDisplayName;
				Encoding = encoding;
			}
		}

		private static EncodingInfoEx[] infos;

		static EncodingEx()
		{
			List<EncodingInfoEx> l = new List<EncodingInfoEx>();
			l.Add(new EncodingInfoEx(SupportedEncoding.ASCII,   "ASCII",                     Encoding.ASCII));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF7,    "Unicode UTF-7",             Encoding.UTF7));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF8,    "Unicode UTF-8",             Encoding.UTF8));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF16,   "Unicode UTF-16",            Encoding.Unicode));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF16BE, "Unicode UTF-16 Big Endian", Encoding.BigEndianUnicode));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF32,   "Unicode UTF-32",            Encoding.UTF32));
			l.Add(new EncodingInfoEx(SupportedEncoding.UTF32BE, "Unicode UTF-32 Big Endian", new UTF32Encoding(true, false)));
			infos = l.ToArray(); 
		}

		/// <summary>
		/// Default is <see cref="Encoding.Default"/>.
		/// </summary>
		public EncodingEx()
			: base((SupportedEncoding)Encoding.Default.CodePage)
		{
		}

		/// <summary></summary>
		protected EncodingEx(SupportedEncoding type)
			: base(type)
		{
		}

		#region CodePage/Name/DisplayName/Encoding/IsDefault/ToString/HashCode

		/// <summary>
		/// Encoding code page.
		/// </summary>
		public virtual int CodePage
		{
			get
			{
				return ((int)((SupportedEncoding)UnderlyingEnum));
			}
		}

		/// <summary>
		/// Unified encoding name.
		/// </summary>
		public virtual string Name
		{
			get
			{
				foreach (EncodingInfo info in Encoding.GetEncodings())
				{
					if ((int)((SupportedEncoding)UnderlyingEnum) == info.CodePage)
						return (info.Name);
				}
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		/// <summary>
		/// Human readable encoding name.
		/// </summary>
		public virtual string DisplayName
		{
			get
			{
				foreach (EncodingInfoEx info in infos)
				{
					if ((SupportedEncoding)UnderlyingEnum == info.SupportedEncoding)
						return (info.BetterDisplayName);
				}
				foreach (EncodingInfo info in Encoding.GetEncodings())
				{
					if ((int)((SupportedEncoding)UnderlyingEnum) == info.CodePage)
						return (info.DisplayName);
				}
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		/// <summary>
		/// Returns encoding object.
		/// </summary>
		public virtual Encoding GetEncoding()
		{
			// default encoding
			if (IsDefault)
				return (Encoding.Default);

			// cached encodings
			foreach (EncodingInfoEx info in infos)
			{
				if ((SupportedEncoding)UnderlyingEnum == info.SupportedEncoding)
				{
					if (info.Encoding != null)
						return (info.Encoding);
					else
						break;
				}
			}

			// other encodings
			return (Encoding.GetEncoding(CodePage));
		}

		/// <summary>
		/// Returns whether this instance is using the default encoding.
		/// </summary>
		public virtual bool IsDefault
		{
			get
			{
				return ((int)((SupportedEncoding)UnderlyingEnum) == Encoding.Default.CodePage);
			}
		}

		/// <summary>
		/// Returns "DisplayName [CodePage]".
		/// </summary>
		public override string ToString()
		{
			if (IsDefault)
				return ("Default (" + DisplayName + ") [" + CodePage + "]");
			else
				return (DisplayName + " [" + CodePage + "]");
		}

		/// <summary>
		/// Returns code page.
		/// </summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region GetItems

		/// <summary>
		/// Returns all available encodings in a useful order.
		/// </summary>
		public static EncodingEx[] GetItems()
		{
			List<EncodingEx> a = new List<EncodingEx>();

			a.Add(new EncodingEx((SupportedEncoding)Encoding.Default.CodePage));

			// ASCII and Unicode
			a.Add(new EncodingEx(SupportedEncoding.ASCII));					// US-ASCII
			a.Add(new EncodingEx(SupportedEncoding.UTF7));					// Unicode (UTF-7)
			a.Add(new EncodingEx(SupportedEncoding.UTF8));					// Unicode (UTF-8)
			a.Add(new EncodingEx(SupportedEncoding.UTF16));					// Unicode
			a.Add(new EncodingEx(SupportedEncoding.UTF16BE));				// Unicode (Big-Endian)
			a.Add(new EncodingEx(SupportedEncoding.UTF32));					// Unicode (UTF-32)
			a.Add(new EncodingEx(SupportedEncoding.UTF32BE));				// Unicode (UTF-32 Big-Endian)

			// ISO
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_1));				// Western European (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_2));				// Central European (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_3));				// Latin 3 (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_4));				// Baltic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_5));				// Cyrillic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_6));				// Arabic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_7));				// Greek (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_8));				// Hebrew (ISO-Visual)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_8I));				// Hebrew (ISO-Logical)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_9));				// Turkish (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_13));				// Estonian (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_15));				// Latin 9 (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022JP));				// Japanese (JIS)
			a.Add(new EncodingEx(SupportedEncoding.CSISO2022JP));			// Japanese (JIS-Allow 1 byte Kana)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022JP_A));			// Japanese (JIS-Allow 1 byte Kana - SO/SI)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022KR));				// Korean (ISO)

			// Windows
			a.Add(new EncodingEx(SupportedEncoding.Windows1252));			// Western European (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1250));			// Central European (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1251));			// Cyrillic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1253));			// Greek (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1254));			// Turkish (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1255));			// Hebrew (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1256));			// Arabic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1257));			// Baltic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1258));			// Vietnamese (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows874));				// Thai (Windows)

			// Mac
			a.Add(new EncodingEx(SupportedEncoding.Macintosh));				// Western European (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCE));					// Central European (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacJapanese));			// Japanese (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacChineseTrad));		// Chinese Traditional (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacChineseSimp));		// Chinese Simplified (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacKorean));				// Korean (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacArabic));				// Arabic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacHebrew));				// Hebrew (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacGreek));				// Greek (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCyrillic));			// Cyrillic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacRomanian));			// Romanian (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacUkrainian));			// Ukrainian (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacThai));				// Thai (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacIcelandic));			// Icelandic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacTurkish));			// Turkish (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCroatian));			// Croatian (Mac)

			// Unix
			a.Add(new EncodingEx(SupportedEncoding.EUC_JP));					// Japanese (JIS 0208-1990 and 0212-1990)
			a.Add(new EncodingEx(SupportedEncoding.EUC_JP_A));				// Japanese (EUC)
			a.Add(new EncodingEx(SupportedEncoding.EUC_CN));					// Chinese Simplified (EUC)
			a.Add(new EncodingEx(SupportedEncoding.EUC_KR));					// Korean (EUC)

			// IBM EBCDIC
			a.Add(new EncodingEx(SupportedEncoding.IBM1047));				// IBM Latin-1
			a.Add(new EncodingEx(SupportedEncoding.IBM924));					// IBM Latin-1
			a.Add(new EncodingEx(SupportedEncoding.IBM500));					// IBM EBCDIC (International)
			a.Add(new EncodingEx(SupportedEncoding.IBM037));					// IBM EBCDIC (US-Canada)
			a.Add(new EncodingEx(SupportedEncoding.IBM870));					// IBM EBCDIC (Multilingual Latin-2)
			a.Add(new EncodingEx(SupportedEncoding.IBM423));					// IBM EBCDIC (Greek)
			a.Add(new EncodingEx(SupportedEncoding.CP875));					// IBM EBCDIC (Greek Modern)
			a.Add(new EncodingEx(SupportedEncoding.IBM880));					// IBM EBCDIC (Cyrillic Russian)
			a.Add(new EncodingEx(SupportedEncoding.CP1025));					// IBM EBCDIC (Cyrillic Serbian-Bulgarian)
			a.Add(new EncodingEx(SupportedEncoding.IBM905));					// IBM EBCDIC (Turkish)
			a.Add(new EncodingEx(SupportedEncoding.IBM1026));				// IBM EBCDIC (Turkish Latin-5)
			a.Add(new EncodingEx(SupportedEncoding.IBM1140));				// IBM EBCDIC (US-Canada-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1141));				// IBM EBCDIC (Germany-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1142));				// IBM EBCDIC (Denmark-Norway-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1143));				// IBM EBCDIC (Finland-Sweden-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1144));				// IBM EBCDIC (Italy-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1145));				// IBM EBCDIC (Spain-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1146));				// IBM EBCDIC (UK-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1147));				// IBM EBCDIC (France-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1148));				// IBM EBCDIC (International-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1149));				// IBM EBCDIC (Icelandic-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM273));					// IBM EBCDIC (Germany)
			a.Add(new EncodingEx(SupportedEncoding.IBM277));					// IBM EBCDIC (Denmark-Norway)
			a.Add(new EncodingEx(SupportedEncoding.IBM278));					// IBM EBCDIC (Finland-Sweden)
			a.Add(new EncodingEx(SupportedEncoding.IBM871));					// IBM EBCDIC (Icelandic)
			a.Add(new EncodingEx(SupportedEncoding.IBM280));					// IBM EBCDIC (Italy)
			a.Add(new EncodingEx(SupportedEncoding.IBM284));					// IBM EBCDIC (Spain)
			a.Add(new EncodingEx(SupportedEncoding.IBM285));					// IBM EBCDIC (UK)
			a.Add(new EncodingEx(SupportedEncoding.IBM297));					// IBM EBCDIC (France)
			a.Add(new EncodingEx(SupportedEncoding.IBM420));					// IBM EBCDIC (Arabic)
			a.Add(new EncodingEx(SupportedEncoding.IBM424));					// IBM EBCDIC (Hebrew)
			a.Add(new EncodingEx(SupportedEncoding.IBM290));					// IBM EBCDIC (Japanese katakana)
			a.Add(new EncodingEx(SupportedEncoding.IBMThai));				// IBM EBCDIC (Thai)
			a.Add(new EncodingEx(SupportedEncoding.X_EBCDIC_KoreanExtended)); // IBM EBCDIC (Korean Extended)

			// IBM OEM
			a.Add(new EncodingEx(SupportedEncoding.IBM437));					// OEM United States
			a.Add(new EncodingEx(SupportedEncoding.IBM855));					// OEM Cyrillic
			a.Add(new EncodingEx(SupportedEncoding.IBM858));					// OEM Multilingual Latin I

			// DOS
			a.Add(new EncodingEx(SupportedEncoding.IBM850));					// Western European (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM852));					// Central European (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM737));					// Greek (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM869));					// Greek, Modern (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM775));					// Baltic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.CP866));					// Cyrillic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM860));					// Portuguese (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM861));					// Icelandic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM863));					// French Canadian (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM865));					// Nordic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM857));					// Turkish (DOS)
			a.Add(new EncodingEx(SupportedEncoding.DOS720));					// Arabic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM864));					// Arabic (864)
			a.Add(new EncodingEx(SupportedEncoding.DOS862));					// Hebrew (DOS)

			// Misc
			a.Add(new EncodingEx(SupportedEncoding.KOI8_R));					// Cyrillic (KOI8-R)
			a.Add(new EncodingEx(SupportedEncoding.KOI8_U));					// Cyrillic (KOI8-U)
			a.Add(new EncodingEx(SupportedEncoding.ASMO_708));				// Arabic (ASMO 708)
			a.Add(new EncodingEx(SupportedEncoding.Shift_JIS));				// Japanese (Shift-JIS)
			a.Add(new EncodingEx(SupportedEncoding.GB2312));					// Chinese Simplified (GB2312)
			a.Add(new EncodingEx(SupportedEncoding.GB18030));				// Chinese Simplified (GB18030)
			a.Add(new EncodingEx(SupportedEncoding.HZ_GB_2312));				// Chinese Simplified (HZ)
			a.Add(new EncodingEx(SupportedEncoding.Big5));					// Chinese Traditional (Big5)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20936));				// Chinese Simplified (GB2312-80)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20949));				// Korean Wansung
			a.Add(new EncodingEx(SupportedEncoding.KS_C_5601_1987));			// Korean
			a.Add(new EncodingEx(SupportedEncoding.Johab));					// Korean (Johab)

			// X
			a.Add(new EncodingEx(SupportedEncoding.X_Europa));				// Europa
			a.Add(new EncodingEx(SupportedEncoding.X_ChineseCNS));			// Chinese Traditional (CNS)
			a.Add(new EncodingEx(SupportedEncoding.X_ChineseEten));			// Chinese Traditional (Eten)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5));					// Western European (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_German));			// German (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_Swedish));			// Swedish (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_Norwegian));		// Norwegian (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20001));				// TCA Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20003));				// IBM5550 Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20004));				// TeleText Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20005));				// Wang Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP50227));				// Chinese Simplified (ISO-2022)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20261));				// T.61
			a.Add(new EncodingEx(SupportedEncoding.X_CP20269));				// ISO-6937

			// X-ISCII
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_DE));				// ISCII Devanagari
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_BE));				// ISCII Bengali
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_TA));				// ISCII Tamil
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_TE));				// ISCII Telugu
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_AS));				// ISCII Assamese
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_OR));				// ISCII Oriya
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_KA));				// ISCII Kannada
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_MA));				// ISCII Malayalam
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_GU));				// ISCII Gujarati
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_PA));				// ISCII Punjabi

			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static EncodingEx Parse(int codePage)
		{
			return (new EncodingEx((SupportedEncoding)codePage));
		}

		/// <summary></summary>
		public static EncodingEx Parse(string encoding)
		{
			EncodingEx result;

			if (TryParse(encoding, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("encoding", encoding, "Invalid encoding."));
		}

		/// <summary></summary>
		public static EncodingEx Parse(Encoding encoding)
		{
			return (Parse(encoding.CodePage));
		}

		/// <summary></summary>
		public static bool TryParse(string encoding, out EncodingEx result)
		{
			foreach (EncodingInfoEx info in infos)
			{
				if (StringEx.EqualsOrdinalIgnoreCase(encoding, info.BetterDisplayName))
				{
					result = new EncodingEx(info.SupportedEncoding);
					return (true);
				}
			}

			foreach (EncodingInfo info in Encoding.GetEncodings())
			{
				if (StringEx.EqualsOrdinalIgnoreCase(encoding, info.Name))
				{
					result = new EncodingEx((SupportedEncoding)info.CodePage);
					return (true);
				}

				if (StringEx.EqualsOrdinalIgnoreCase(encoding, info.DisplayName))
				{
					result = new EncodingEx((SupportedEncoding)info.CodePage);
					return (true);
				}
			}

			result = null;
			return (false);
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SupportedEncoding(EncodingEx encoding)
		{
			return ((SupportedEncoding)encoding.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator EncodingEx(SupportedEncoding encoding)
		{
			return (new EncodingEx(encoding));
		}

		/// <summary></summary>
		public static implicit operator Encoding(EncodingEx encoding)
		{
			return (encoding.GetEncoding());
		}

		/// <summary></summary>
		public static implicit operator EncodingEx(Encoding encoding)
		{
			return (Parse(encoding));
		}

		/// <summary></summary>
		public static implicit operator int(EncodingEx encoding)
		{
			return (encoding.CodePage);
		}

		/// <summary></summary>
		public static implicit operator EncodingEx(int codePage)
		{
			return (Parse(codePage));
		}

		/// <summary></summary>
		public static implicit operator string(EncodingEx encoding)
		{
			return (encoding.ToString());
		}

		/// <summary></summary>
		public static implicit operator EncodingEx(string encoding)
		{
			return (Parse(encoding));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
