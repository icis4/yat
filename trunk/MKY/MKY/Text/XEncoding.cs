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

using MKY.Types;

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
	public class XEncoding : XEnum
	{
		private struct XEncodingInfo
		{
			public SupportedEncoding SupportedEncoding;
			public string BetterDisplayName;
			public Encoding Encoding;

			public XEncodingInfo(SupportedEncoding supportedEncoding, string betterDisplayName, Encoding encoding)
			{
				SupportedEncoding = supportedEncoding;
				BetterDisplayName = betterDisplayName;
				Encoding = encoding;
			}
		}

		private static XEncodingInfo[] infos;

		static XEncoding()
		{
			List<XEncodingInfo> l = new List<XEncodingInfo>();
			l.Add(new XEncodingInfo(SupportedEncoding.ASCII,   "ASCII",                     Encoding.ASCII));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF7,    "Unicode UTF-7",             Encoding.UTF7));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF8,    "Unicode UTF-8",             Encoding.UTF8));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF16,   "Unicode UTF-16",            Encoding.Unicode));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF16BE, "Unicode UTF-16 Big Endian", Encoding.BigEndianUnicode));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF32,   "Unicode UTF-32",            Encoding.UTF32));
			l.Add(new XEncodingInfo(SupportedEncoding.UTF32BE, "Unicode UTF-32 Big Endian", new UTF32Encoding(true, false)));
			infos = l.ToArray(); 
		}

		/// <summary>
		/// Default is <see cref="Encoding.Default"/>.
		/// </summary>
		public XEncoding()
			: base((SupportedEncoding)Encoding.Default.CodePage)
		{
		}

		/// <summary></summary>
		protected XEncoding(SupportedEncoding type)
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
				foreach (XEncodingInfo info in infos)
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
			foreach (XEncodingInfo info in infos)
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
		public static XEncoding[] GetItems()
		{
			List<XEncoding> a = new List<XEncoding>();

			a.Add(new XEncoding((SupportedEncoding)Encoding.Default.CodePage));

			// ASCII and Unicode
			a.Add(new XEncoding(SupportedEncoding.ASCII));					// US-ASCII
			a.Add(new XEncoding(SupportedEncoding.UTF7));					// Unicode (UTF-7)
			a.Add(new XEncoding(SupportedEncoding.UTF8));					// Unicode (UTF-8)
			a.Add(new XEncoding(SupportedEncoding.UTF16));					// Unicode
			a.Add(new XEncoding(SupportedEncoding.UTF16BE));				// Unicode (Big-Endian)
			a.Add(new XEncoding(SupportedEncoding.UTF32));					// Unicode (UTF-32)
			a.Add(new XEncoding(SupportedEncoding.UTF32BE));				// Unicode (UTF-32 Big-Endian)

			// ISO
			a.Add(new XEncoding(SupportedEncoding.ISO8859_1));				// Western European (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_2));				// Central European (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_3));				// Latin 3 (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_4));				// Baltic (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_5));				// Cyrillic (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_6));				// Arabic (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_7));				// Greek (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_8));				// Hebrew (ISO-Visual)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_8I));				// Hebrew (ISO-Logical)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_9));				// Turkish (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_13));				// Estonian (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO8859_15));				// Latin 9 (ISO)
			a.Add(new XEncoding(SupportedEncoding.ISO2022JP));				// Japanese (JIS)
			a.Add(new XEncoding(SupportedEncoding.CSISO2022JP));			// Japanese (JIS-Allow 1 byte Kana)
			a.Add(new XEncoding(SupportedEncoding.ISO2022JP_A));			// Japanese (JIS-Allow 1 byte Kana - SO/SI)
			a.Add(new XEncoding(SupportedEncoding.ISO2022KR));				// Korean (ISO)

			// Windows
			a.Add(new XEncoding(SupportedEncoding.Windows1252));			// Western European (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1250));			// Central European (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1251));			// Cyrillic (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1253));			// Greek (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1254));			// Turkish (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1255));			// Hebrew (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1256));			// Arabic (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1257));			// Baltic (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows1258));			// Vietnamese (Windows)
			a.Add(new XEncoding(SupportedEncoding.Windows874));				// Thai (Windows)

			// Mac
			a.Add(new XEncoding(SupportedEncoding.Macintosh));				// Western European (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacCE));					// Central European (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacJapanese));			// Japanese (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacChineseTrad));		// Chinese Traditional (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacChineseSimp));		// Chinese Simplified (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacKorean));				// Korean (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacArabic));				// Arabic (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacHebrew));				// Hebrew (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacGreek));				// Greek (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacCyrillic));			// Cyrillic (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacRomanian));			// Romanian (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacUkrainian));			// Ukrainian (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacThai));				// Thai (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacIcelandic));			// Icelandic (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacTurkish));			// Turkish (Mac)
			a.Add(new XEncoding(SupportedEncoding.XMacCroatian));			// Croatian (Mac)

			// Unix
			a.Add(new XEncoding(SupportedEncoding.EUC_JP));					// Japanese (JIS 0208-1990 and 0212-1990)
			a.Add(new XEncoding(SupportedEncoding.EUC_JP_A));				// Japanese (EUC)
			a.Add(new XEncoding(SupportedEncoding.EUC_CN));					// Chinese Simplified (EUC)
			a.Add(new XEncoding(SupportedEncoding.EUC_KR));					// Korean (EUC)

			// IBM EBCDIC
			a.Add(new XEncoding(SupportedEncoding.IBM1047));				// IBM Latin-1
			a.Add(new XEncoding(SupportedEncoding.IBM924));					// IBM Latin-1
			a.Add(new XEncoding(SupportedEncoding.IBM500));					// IBM EBCDIC (International)
			a.Add(new XEncoding(SupportedEncoding.IBM037));					// IBM EBCDIC (US-Canada)
			a.Add(new XEncoding(SupportedEncoding.IBM870));					// IBM EBCDIC (Multilingual Latin-2)
			a.Add(new XEncoding(SupportedEncoding.IBM423));					// IBM EBCDIC (Greek)
			a.Add(new XEncoding(SupportedEncoding.CP875));					// IBM EBCDIC (Greek Modern)
			a.Add(new XEncoding(SupportedEncoding.IBM880));					// IBM EBCDIC (Cyrillic Russian)
			a.Add(new XEncoding(SupportedEncoding.CP1025));					// IBM EBCDIC (Cyrillic Serbian-Bulgarian)
			a.Add(new XEncoding(SupportedEncoding.IBM905));					// IBM EBCDIC (Turkish)
			a.Add(new XEncoding(SupportedEncoding.IBM1026));				// IBM EBCDIC (Turkish Latin-5)
			a.Add(new XEncoding(SupportedEncoding.IBM1140));				// IBM EBCDIC (US-Canada-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1141));				// IBM EBCDIC (Germany-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1142));				// IBM EBCDIC (Denmark-Norway-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1143));				// IBM EBCDIC (Finland-Sweden-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1144));				// IBM EBCDIC (Italy-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1145));				// IBM EBCDIC (Spain-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1146));				// IBM EBCDIC (UK-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1147));				// IBM EBCDIC (France-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1148));				// IBM EBCDIC (International-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM1149));				// IBM EBCDIC (Icelandic-Euro)
			a.Add(new XEncoding(SupportedEncoding.IBM273));					// IBM EBCDIC (Germany)
			a.Add(new XEncoding(SupportedEncoding.IBM277));					// IBM EBCDIC (Denmark-Norway)
			a.Add(new XEncoding(SupportedEncoding.IBM278));					// IBM EBCDIC (Finland-Sweden)
			a.Add(new XEncoding(SupportedEncoding.IBM871));					// IBM EBCDIC (Icelandic)
			a.Add(new XEncoding(SupportedEncoding.IBM280));					// IBM EBCDIC (Italy)
			a.Add(new XEncoding(SupportedEncoding.IBM284));					// IBM EBCDIC (Spain)
			a.Add(new XEncoding(SupportedEncoding.IBM285));					// IBM EBCDIC (UK)
			a.Add(new XEncoding(SupportedEncoding.IBM297));					// IBM EBCDIC (France)
			a.Add(new XEncoding(SupportedEncoding.IBM420));					// IBM EBCDIC (Arabic)
			a.Add(new XEncoding(SupportedEncoding.IBM424));					// IBM EBCDIC (Hebrew)
			a.Add(new XEncoding(SupportedEncoding.IBM290));					// IBM EBCDIC (Japanese katakana)
			a.Add(new XEncoding(SupportedEncoding.IBMThai));				// IBM EBCDIC (Thai)
			a.Add(new XEncoding(SupportedEncoding.X_EBCDIC_KoreanExtended)); // IBM EBCDIC (Korean Extended)

			// IBM OEM
			a.Add(new XEncoding(SupportedEncoding.IBM437));					// OEM United States
			a.Add(new XEncoding(SupportedEncoding.IBM855));					// OEM Cyrillic
			a.Add(new XEncoding(SupportedEncoding.IBM858));					// OEM Multilingual Latin I

			// DOS
			a.Add(new XEncoding(SupportedEncoding.IBM850));					// Western European (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM852));					// Central European (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM737));					// Greek (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM869));					// Greek, Modern (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM775));					// Baltic (DOS)
			a.Add(new XEncoding(SupportedEncoding.CP866));					// Cyrillic (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM860));					// Portuguese (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM861));					// Icelandic (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM863));					// French Canadian (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM865));					// Nordic (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM857));					// Turkish (DOS)
			a.Add(new XEncoding(SupportedEncoding.DOS720));					// Arabic (DOS)
			a.Add(new XEncoding(SupportedEncoding.IBM864));					// Arabic (864)
			a.Add(new XEncoding(SupportedEncoding.DOS862));					// Hebrew (DOS)

			// Misc
			a.Add(new XEncoding(SupportedEncoding.KOI8_R));					// Cyrillic (KOI8-R)
			a.Add(new XEncoding(SupportedEncoding.KOI8_U));					// Cyrillic (KOI8-U)
			a.Add(new XEncoding(SupportedEncoding.ASMO_708));				// Arabic (ASMO 708)
			a.Add(new XEncoding(SupportedEncoding.Shift_JIS));				// Japanese (Shift-JIS)
			a.Add(new XEncoding(SupportedEncoding.GB2312));					// Chinese Simplified (GB2312)
			a.Add(new XEncoding(SupportedEncoding.GB18030));				// Chinese Simplified (GB18030)
			a.Add(new XEncoding(SupportedEncoding.HZ_GB_2312));				// Chinese Simplified (HZ)
			a.Add(new XEncoding(SupportedEncoding.Big5));					// Chinese Traditional (Big5)
			a.Add(new XEncoding(SupportedEncoding.X_CP20936));				// Chinese Simplified (GB2312-80)
			a.Add(new XEncoding(SupportedEncoding.X_CP20949));				// Korean Wansung
			a.Add(new XEncoding(SupportedEncoding.KS_C_5601_1987));			// Korean
			a.Add(new XEncoding(SupportedEncoding.Johab));					// Korean (Johab)

			// X
			a.Add(new XEncoding(SupportedEncoding.X_Europa));				// Europa
			a.Add(new XEncoding(SupportedEncoding.X_ChineseCNS));			// Chinese Traditional (CNS)
			a.Add(new XEncoding(SupportedEncoding.X_ChineseEten));			// Chinese Traditional (Eten)
			a.Add(new XEncoding(SupportedEncoding.X_IA5));					// Western European (IA5)
			a.Add(new XEncoding(SupportedEncoding.X_IA5_German));			// German (IA5)
			a.Add(new XEncoding(SupportedEncoding.X_IA5_Swedish));			// Swedish (IA5)
			a.Add(new XEncoding(SupportedEncoding.X_IA5_Norwegian));		// Norwegian (IA5)
			a.Add(new XEncoding(SupportedEncoding.X_CP20001));				// TCA Taiwan
			a.Add(new XEncoding(SupportedEncoding.X_CP20003));				// IBM5550 Taiwan
			a.Add(new XEncoding(SupportedEncoding.X_CP20004));				// TeleText Taiwan
			a.Add(new XEncoding(SupportedEncoding.X_CP20005));				// Wang Taiwan
			a.Add(new XEncoding(SupportedEncoding.X_CP50227));				// Chinese Simplified (ISO-2022)
			a.Add(new XEncoding(SupportedEncoding.X_CP20261));				// T.61
			a.Add(new XEncoding(SupportedEncoding.X_CP20269));				// ISO-6937

			// X-ISCII
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_DE));				// ISCII Devanagari
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_BE));				// ISCII Bengali
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_TA));				// ISCII Tamil
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_TE));				// ISCII Telugu
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_AS));				// ISCII Assamese
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_OR));				// ISCII Oriya
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_KA));				// ISCII Kannada
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_MA));				// ISCII Malayalam
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_GU));				// ISCII Gujarati
			a.Add(new XEncoding(SupportedEncoding.X_ISCII_PA));				// ISCII Punjabi

			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XEncoding Parse(int codePage)
		{
			return (new XEncoding((SupportedEncoding)codePage));
		}

		/// <summary></summary>
		public static XEncoding Parse(string encoding)
		{
			XEncoding result;

			if (TryParse(encoding, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("encoding", encoding, "Invalid encoding."));
		}

		/// <summary></summary>
		public static XEncoding Parse(Encoding encoding)
		{
			return (Parse(encoding.CodePage));
		}

		/// <summary></summary>
		public static bool TryParse(string encoding, out XEncoding result)
		{
			foreach (XEncodingInfo info in infos)
			{
				if (string.Compare(encoding, info.BetterDisplayName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = new XEncoding(info.SupportedEncoding);
					return (true);
				}
			}

			foreach (EncodingInfo info in Encoding.GetEncodings())
			{
				if (string.Compare(encoding, info.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = new XEncoding((SupportedEncoding)info.CodePage);
					return (true);
				}

				if (string.Compare(encoding, info.DisplayName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = new XEncoding((SupportedEncoding)info.CodePage);
					return (true);
				}
			}

			result = null;
			return (false);
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SupportedEncoding(XEncoding encoding)
		{
			return ((SupportedEncoding)encoding.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XEncoding(SupportedEncoding encoding)
		{
			return (new XEncoding(encoding));
		}

		/// <summary></summary>
		public static implicit operator Encoding(XEncoding encoding)
		{
			return (encoding.GetEncoding());
		}

		/// <summary></summary>
		public static implicit operator XEncoding(Encoding encoding)
		{
			return (Parse(encoding));
		}

		/// <summary></summary>
		public static implicit operator int(XEncoding encoding)
		{
			return (encoding.CodePage);
		}

		/// <summary></summary>
		public static implicit operator XEncoding(int codePage)
		{
			return (Parse(codePage));
		}

		/// <summary></summary>
		public static implicit operator string(XEncoding encoding)
		{
			return (encoding.ToString());
		}

		/// <summary></summary>
		public static implicit operator XEncoding(string encoding)
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
