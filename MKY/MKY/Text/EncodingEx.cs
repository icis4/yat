//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#endregion

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "The naming is defined by the according standard."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#XMacChineseTrad", MessageId = "Trad")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ChineseEten", MessageId = "Eten")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#XMacChineseSimp", MessageId = "Simp")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_Europa", MessageId = "Europa")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#Johab", MessageId = "Johab")]

// Justification = "The naming is defined by the according standard."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_KR")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_GU")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20269")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_TA")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_8")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#HZ_GB_2312")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_IA5_Norwegian")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO2022JP_A")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_TE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20004")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_6")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_DE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20936")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20001")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_1")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ChineseEten")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_JP_A")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_MA")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_IA5_Swedish")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_JP")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_AS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_15")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_CN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#Shift_JIS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ASMO_708")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_5")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20261")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_BE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_EBCDIC_KoreanExtended")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_KA")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_OR")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_Europa")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_4")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20949")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_3")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_13")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_IA5")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#KOI8_R")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_IA5_German")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_8I")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_PA")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#KOI8_U")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20005")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_7")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#KS_C_5601_1987")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_2")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP50227")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_CP20003")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_9")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ChineseCNS")]

// Justification = "The naming is defined by the according standard."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM869", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM857", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF32", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM855", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM278", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_KR", MessageId = "EUC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_GU", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM423", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM437", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM737", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM290", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM037", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_TA", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM420", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO2022KR", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM273", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1146", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1142", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF16", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_8", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1047", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM297", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO2022JP_A", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM870", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_TE", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_6", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_DE", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1141", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM858", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM865", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_1", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_JP_A", MessageId = "EUC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_MA", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_JP", MessageId = "EUC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM852", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_AS", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_15", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#EUC_CN", MessageId = "EUC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1147", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1143", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#Shift_JIS", MessageId = "JIS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM863", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ASMO_708", MessageId = "ASMO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM775", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_5", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF7", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM861", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_BE", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM500", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_EBCDIC_KoreanExtended", MessageId = "EBCDIC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM860", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF8", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_KA", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_OR", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF32BE", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_4", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM284", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM850", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM285", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1148", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1144", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1140", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBMThai", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#DOS862", MessageId = "DOS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_3", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM871", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_13", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO2022JP", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#UTF16BE", MessageId = "UTF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#KOI8_R", MessageId = "KOI")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_8I", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ISCII_PA", MessageId = "ISCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM864", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#KOI8_U", MessageId = "KOI")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ASCII", MessageId = "ASCII")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM905", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1026", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_7", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#CSISO2022JP", MessageId = "CSISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM280", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1149", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM1145", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_2", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM880", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM424", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM277", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#ISO8859_9", MessageId = "ISO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#IBM924", MessageId = "IBM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#X_ChineseCNS", MessageId = "CNS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Text.SupportedEncoding.#DOS720", MessageId = "DOS")]

#endregion

namespace MKY.Text
{
	#region Enum SupportedEncoding

	/// <summary>
	/// Encodings that are supported by .NET, currently 140 encodings.
	/// </summary>
	/// <remarks>
	/// Enum value corresponds to code page.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Text is given by the MSDN.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1631:DocumentationMustMeetCharacterPercentage", Justification = "Text is given by the MSDN.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1632:DocumentationTextMustMeetMinimumCharacterLength", Justification = "Text is given by the MSDN.")]
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "IDs of encodings are given by the according encoding standards.")]
	public enum SupportedEncoding
	{
		//------------------------------------------------------------------------------------------
		// ASCII and Unicode
		//------------------------------------------------------------------------------------------

		/// <summary>US-ASCII.</summary>
		ASCII = 20127,

	/////// <summary>Unicode (UTF-7).</summary>
	////UTF7 = 65000 removed as that encoding belongs to the class of Base64 and Quoted-Printable (FR #407).

		/// <summary>Unicode (UTF-8).</summary>
		UTF8 = 65001,

		/// <summary>Unicode.</summary>
		UTF16 = 1200,

		/// <summary>Unicode (Big-Endian).</summary>
		UTF16BE = 1201,

		/// <summary>Unicode (UTF-32).</summary>
		UTF32 = 12000,

		/// <summary>Unicode (UTF-32 Big-Endian).</summary>
		UTF32BE = 12001,

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

		/// <summary>Korean (Wansung).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Wansung' is 'Wansung'...")]
		X_CP20949 = 20949,

		/// <summary>Korean.</summary>
		KS_C_5601_1987 = 949,

		/// <summary>Korean (Johab).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Johab' is 'Johab'...")]
		Johab = 1361,

		//------------------------------------------------------------------------------------------
		// X
		//------------------------------------------------------------------------------------------

		/// <summary>Europa.</summary>
		X_Europa = 29001,

		/// <summary>Chinese Traditional (CNS).</summary>
		X_ChineseCNS = 20000,

		/// <summary>Chinese Traditional (Eten).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Eten' is 'Eten'...")]
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't know 'Devanagari', what a shame! This is the character set used for Sanskrit!")]
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
		X_ISCII_PA = 57011
	}

	#endregion

	/// <summary>
	/// Extended enum EncodingEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class EncodingEx : EnumEx, IEquatable<EncodingEx>
	{
		/// <summary>
		/// Default is <see cref="SupportedEncoding.UTF8"/> which corresponds to <see cref="Encoding.UTF8"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Encoding.Default"/>:
		/// "Because all <see cref="Encoding.Default"/> encodings based on ANSI code pages lose data,
		/// consider using the <see cref="Encoding.UTF8"/> encoding instead."
		/// and
		/// "On .NET Core, the 'Default' property always returns the 'UTF8Encoding'."
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <remarks>
		/// Must be implemented as const (instead of a readonly <see cref="Encoding"/>) as that
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public const SupportedEncoding Default = SupportedEncoding.UTF8; // = Encoding.UTF8.CodePage;

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

		private static EncodingInfoEx[] staticInfos =
		{
			new EncodingInfoEx(SupportedEncoding.ASCII,   "ASCII (ISO646-US)",         Encoding.ASCII),
		////new EncodingInfoEx(SupportedEncoding.UTF7,    "Unicode UTF-7",             Encoding.UTF7) removed as that encoding belongs to the class of Base64 and Quoted-Printable (FR #407).
			new EncodingInfoEx(SupportedEncoding.UTF8,    "Unicode UTF-8",             Encoding.UTF8),
			new EncodingInfoEx(SupportedEncoding.UTF16,   "Unicode UTF-16",            Encoding.Unicode),
			new EncodingInfoEx(SupportedEncoding.UTF16BE, "Unicode UTF-16 Big Endian", Encoding.BigEndianUnicode),
			new EncodingInfoEx(SupportedEncoding.UTF32,   "Unicode UTF-32",            Encoding.UTF32),
			new EncodingInfoEx(SupportedEncoding.UTF32BE, "Unicode UTF-32 Big Endian", new UTF32Encoding(true, false))
		};

		/// <summary>
		/// Default is <see cref="Default"/>.
		/// </summary>
		public EncodingEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public EncodingEx(int codePage)
			: this((SupportedEncoding)codePage)
		{
		}

		/// <summary></summary>
		public EncodingEx(SupportedEncoding encoding)
			: base(encoding)
		{
		}

		#region CodePage/Name/DisplayName/Encoding/IsDefault
		//==========================================================================================
		// CodePage/Name/DisplayName/Encoding/IsDefault
		//==========================================================================================

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

				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an encoding that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Human readable encoding name.
		/// </summary>
		public virtual string DisplayName
		{
			get
			{
				foreach (EncodingInfoEx info in staticInfos)
				{
					if ((SupportedEncoding)UnderlyingEnum == info.SupportedEncoding)
						return (info.BetterDisplayName);
				}

				foreach (EncodingInfo info in Encoding.GetEncodings())
				{
					if ((int)((SupportedEncoding)UnderlyingEnum) == info.CodePage)
						return (info.DisplayName);
				}

				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an encoding that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Returns encoding object.
		/// </summary>
		public virtual Encoding Encoding
		{
			get
			{
				// Cached encodings, including 'Default':
				foreach (EncodingInfoEx info in staticInfos)
				{
					if ((SupportedEncoding)UnderlyingEnum == info.SupportedEncoding)
					{
						if (info.Encoding != null)
							return (info.Encoding);
						else
							break;
					}
				}

				// Other encodings:
				return (Encoding.GetEncoding(CodePage));
			}
		}

		/// <summary>
		/// Returns the encoding object for the given encoding.
		/// </summary>
		public static Encoding GetEncoding(SupportedEncoding encoding)
		{
			return (new EncodingEx(encoding).Encoding);
		}

		/// <summary>
		/// Returns whether this instance represents the default encoding.
		/// </summary>
		public virtual bool IsDefault
		{
			get
			{
				return (((SupportedEncoding)UnderlyingEnum) == Default);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current encoding uses single-byte code points.
		/// </summary>
		public virtual bool IsSingleByte
		{
			get { return (Encoding.GetEncoding(CodePage).IsSingleByte); }
		}

		/// <summary>
		/// Gets a value indicating whether the current encoding uses multi-byte code points.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiByte'?")]
		public virtual bool IsMultiByte
		{
			get { return (!IsSingleByte); }
		}

		/// <summary>
		/// Returns whether this instance represents one of the Unicode encodings (UTF/UCS).
		/// </summary>
		/// <remarks>
		/// UTF-7 is *not* a Unicode encoding.
		/// </remarks>
		public virtual bool IsUnicode
		{
			get
			{
				switch ((SupportedEncoding)CodePage)
				{
					case SupportedEncoding.UTF8:
					case SupportedEncoding.UTF16:
					case SupportedEncoding.UTF16BE:
					case SupportedEncoding.UTF32:
					case SupportedEncoding.UTF32BE:
					{
						return (true);
					}

					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary>
		/// Returns whether this instance represents one of the big-endian Unicode encodings (UTF16BE or UTF32BE).
		/// </summary>
		public virtual bool IsUnicodeBigEndian
		{
			get
			{
				switch ((SupportedEncoding)CodePage)
				{
					case SupportedEncoding.UTF16BE:
					case SupportedEncoding.UTF32BE:
					{
						return (true);
					}

					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary>
		/// Returns the minimum byte count of this instance.
		/// </summary>
		/// <remarks>
		/// Method instead of property for same signature as <see cref="Encoding.GetMaxByteCount(int)"/>,
		/// though without 'charCount' parameter (method is limited to return count for a single character).
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remark above.")]
		public virtual int GetMinByteCount()
		{
			switch ((SupportedEncoding)CodePage)
			{
				case SupportedEncoding.UTF32:
				case SupportedEncoding.UTF32BE:
				{
					return (4);
				}

				case SupportedEncoding.UTF16:
				case SupportedEncoding.UTF16BE:
				{
					return (2);
				}

				case SupportedEncoding.UTF8:
				default: // Covers all SBCS as well as UTF-7 and all non-Unicode DBCS/MBCS (they are all ASCII compatible).
				{
					return (1);
				}
			}
		}

		/// <summary>
		/// Returns the fragment byte count if this instance represents one of the Unicode encodings.
		/// </summary>
		/// <remarks>
		/// Value equals <see cref="GetMinByteCount()"/> for Unicode encodings.
		/// </remarks>
		public virtual int UnicodeFragmentByteCount
		{
			get
			{
				if (IsUnicode)
					return (GetMinByteCount());
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is no Unicode encoding!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation,
		/// which is "DisplayName [CodePage]".
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			var sb = new StringBuilder(DisplayName + " [" + CodePage + "]");

			if (IsDefault)
				sb.Append(" (Default)");

			return (sb.ToString());
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as EncodingEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(EncodingEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (base.Equals(other));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(EncodingEx lhs, EncodingEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(EncodingEx lhs, EncodingEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <summary>
		/// Returns all available encodings in a useful order.
		/// </summary>
		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static EncodingEx[] GetItems()
		{
			var a = new List<EncodingEx>(256); // Preset the required capacity to improve memory management; 256 is a large enough value.

			// ASCII and Unicode
			a.Add(new EncodingEx(SupportedEncoding.ASCII));				// US-ASCII
		////a.Add(new EncodingEx(SupportedEncoding.UTF7));				// Unicode (UTF-7) removed as that encoding belongs to the class of Base64 and Quoted-Printable (FR #407).
			a.Add(new EncodingEx(SupportedEncoding.UTF8));				// Unicode (UTF-8)
			a.Add(new EncodingEx(SupportedEncoding.UTF16));				// Unicode
			a.Add(new EncodingEx(SupportedEncoding.UTF16BE));			// Unicode (Big-Endian)
			a.Add(new EncodingEx(SupportedEncoding.UTF32));				// Unicode (UTF-32)
			a.Add(new EncodingEx(SupportedEncoding.UTF32BE));			// Unicode (UTF-32 Big-Endian)

			// ISO
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_1));			// Western European (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_2));			// Central European (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_3));			// Latin 3 (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_4));			// Baltic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_5));			// Cyrillic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_6));			// Arabic (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_7));			// Greek (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_8));			// Hebrew (ISO-Visual)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_8I));		// Hebrew (ISO-Logical)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_9));			// Turkish (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_13));		// Estonian (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO8859_15));		// Latin 9 (ISO)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022JP));			// Japanese (JIS)
			a.Add(new EncodingEx(SupportedEncoding.CSISO2022JP));		// Japanese (JIS-Allow 1 byte Kana)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022JP_A));		// Japanese (JIS-Allow 1 byte Kana - SO/SI)
			a.Add(new EncodingEx(SupportedEncoding.ISO2022KR));			// Korean (ISO)

			// Windows
			a.Add(new EncodingEx(SupportedEncoding.Windows1250));		// Central European (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1251));		// Cyrillic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1252));		// Western European (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1253));		// Greek (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1254));		// Turkish (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1255));		// Hebrew (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1256));		// Arabic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1257));		// Baltic (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows1258));		// Vietnamese (Windows)
			a.Add(new EncodingEx(SupportedEncoding.Windows874));		// Thai (Windows)

			// Mac
			a.Add(new EncodingEx(SupportedEncoding.Macintosh));			// Western European (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCE));			// Central European (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacJapanese));		// Japanese (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacChineseTrad));	// Chinese Traditional (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacChineseSimp));	// Chinese Simplified (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacKorean));		// Korean (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacArabic));		// Arabic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacHebrew));		// Hebrew (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacGreek));			// Greek (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCyrillic));		// Cyrillic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacRomanian));		// Romanian (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacUkrainian));		// Ukrainian (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacThai));			// Thai (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacIcelandic));		// Icelandic (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacTurkish));		// Turkish (Mac)
			a.Add(new EncodingEx(SupportedEncoding.XMacCroatian));		// Croatian (Mac)

			// Unix
			a.Add(new EncodingEx(SupportedEncoding.EUC_JP));			// Japanese (JIS 0208-1990 and 0212-1990)
			a.Add(new EncodingEx(SupportedEncoding.EUC_JP_A));			// Japanese (EUC)
			a.Add(new EncodingEx(SupportedEncoding.EUC_CN));			// Chinese Simplified (EUC)
			a.Add(new EncodingEx(SupportedEncoding.EUC_KR));			// Korean (EUC)

			// IBM EBCDIC
			a.Add(new EncodingEx(SupportedEncoding.IBM1047));			// IBM Latin-1
			a.Add(new EncodingEx(SupportedEncoding.IBM924));			// IBM Latin-1
			a.Add(new EncodingEx(SupportedEncoding.IBM500));			// IBM EBCDIC (International)
			a.Add(new EncodingEx(SupportedEncoding.IBM037));			// IBM EBCDIC (US-Canada)
			a.Add(new EncodingEx(SupportedEncoding.IBM870));			// IBM EBCDIC (Multilingual Latin-2)
			a.Add(new EncodingEx(SupportedEncoding.IBM423));			// IBM EBCDIC (Greek)
			a.Add(new EncodingEx(SupportedEncoding.CP875));				// IBM EBCDIC (Greek Modern)
			a.Add(new EncodingEx(SupportedEncoding.IBM880));			// IBM EBCDIC (Cyrillic Russian)
			a.Add(new EncodingEx(SupportedEncoding.CP1025));			// IBM EBCDIC (Cyrillic Serbian-Bulgarian)
			a.Add(new EncodingEx(SupportedEncoding.IBM905));			// IBM EBCDIC (Turkish)
			a.Add(new EncodingEx(SupportedEncoding.IBM1026));			// IBM EBCDIC (Turkish Latin-5)
			a.Add(new EncodingEx(SupportedEncoding.IBM1140));			// IBM EBCDIC (US-Canada-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1141));			// IBM EBCDIC (Germany-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1142));			// IBM EBCDIC (Denmark-Norway-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1143));			// IBM EBCDIC (Finland-Sweden-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1144));			// IBM EBCDIC (Italy-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1145));			// IBM EBCDIC (Spain-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1146));			// IBM EBCDIC (UK-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1147));			// IBM EBCDIC (France-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1148));			// IBM EBCDIC (International-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM1149));			// IBM EBCDIC (Icelandic-Euro)
			a.Add(new EncodingEx(SupportedEncoding.IBM273));			// IBM EBCDIC (Germany)
			a.Add(new EncodingEx(SupportedEncoding.IBM277));			// IBM EBCDIC (Denmark-Norway)
			a.Add(new EncodingEx(SupportedEncoding.IBM278));			// IBM EBCDIC (Finland-Sweden)
			a.Add(new EncodingEx(SupportedEncoding.IBM871));			// IBM EBCDIC (Icelandic)
			a.Add(new EncodingEx(SupportedEncoding.IBM280));			// IBM EBCDIC (Italy)
			a.Add(new EncodingEx(SupportedEncoding.IBM284));			// IBM EBCDIC (Spain)
			a.Add(new EncodingEx(SupportedEncoding.IBM285));			// IBM EBCDIC (UK)
			a.Add(new EncodingEx(SupportedEncoding.IBM297));			// IBM EBCDIC (France)
			a.Add(new EncodingEx(SupportedEncoding.IBM420));			// IBM EBCDIC (Arabic)
			a.Add(new EncodingEx(SupportedEncoding.IBM424));			// IBM EBCDIC (Hebrew)
			a.Add(new EncodingEx(SupportedEncoding.IBM290));			// IBM EBCDIC (Japanese katakana)
			a.Add(new EncodingEx(SupportedEncoding.IBMThai));			// IBM EBCDIC (Thai)
			a.Add(new EncodingEx(SupportedEncoding.X_EBCDIC_KoreanExtended)); // IBM EBCDIC (Korean Extended)

			// IBM OEM
			a.Add(new EncodingEx(SupportedEncoding.IBM437));			// OEM United States
			a.Add(new EncodingEx(SupportedEncoding.IBM855));			// OEM Cyrillic
			a.Add(new EncodingEx(SupportedEncoding.IBM858));			// OEM Multilingual Latin I

			// DOS
			a.Add(new EncodingEx(SupportedEncoding.IBM850));			// Western European (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM852));			// Central European (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM737));			// Greek (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM869));			// Greek, Modern (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM775));			// Baltic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.CP866));				// Cyrillic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM860));			// Portuguese (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM861));			// Icelandic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM863));			// French Canadian (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM865));			// Nordic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM857));			// Turkish (DOS)
			a.Add(new EncodingEx(SupportedEncoding.DOS720));			// Arabic (DOS)
			a.Add(new EncodingEx(SupportedEncoding.IBM864));			// Arabic (864)
			a.Add(new EncodingEx(SupportedEncoding.DOS862));			// Hebrew (DOS)

			// Misc
			a.Add(new EncodingEx(SupportedEncoding.KOI8_R));			// Cyrillic (KOI8-R)
			a.Add(new EncodingEx(SupportedEncoding.KOI8_U));			// Cyrillic (KOI8-U)
			a.Add(new EncodingEx(SupportedEncoding.ASMO_708));			// Arabic (ASMO 708)
			a.Add(new EncodingEx(SupportedEncoding.Shift_JIS));			// Japanese (Shift-JIS)
			a.Add(new EncodingEx(SupportedEncoding.GB2312));			// Chinese Simplified (GB2312)
			a.Add(new EncodingEx(SupportedEncoding.GB18030));			// Chinese Simplified (GB18030)
			a.Add(new EncodingEx(SupportedEncoding.HZ_GB_2312));		// Chinese Simplified (HZ)
			a.Add(new EncodingEx(SupportedEncoding.Big5));				// Chinese Traditional (Big5)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20936));			// Chinese Simplified (GB2312-80)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20949));			// Korean Wansung
			a.Add(new EncodingEx(SupportedEncoding.KS_C_5601_1987));	// Korean
			a.Add(new EncodingEx(SupportedEncoding.Johab));				// Korean (Johab)

			// X
			a.Add(new EncodingEx(SupportedEncoding.X_Europa));			// Europa
			a.Add(new EncodingEx(SupportedEncoding.X_ChineseCNS));		// Chinese Traditional (CNS)
			a.Add(new EncodingEx(SupportedEncoding.X_ChineseEten));		// Chinese Traditional (Eten)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5));				// Western European (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_German));		// German (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_Swedish));		// Swedish (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_IA5_Norwegian));	// Norwegian (IA5)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20001));			// TCA Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20003));			// IBM5550 Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20004));			// TeleText Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP20005));			// Wang Taiwan
			a.Add(new EncodingEx(SupportedEncoding.X_CP50227));			// Chinese Simplified (ISO-2022)
			a.Add(new EncodingEx(SupportedEncoding.X_CP20261));			// T.61
			a.Add(new EncodingEx(SupportedEncoding.X_CP20269));			// ISO-6937

			// X-ISCII
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_DE));		// ISCII Devanagari
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_BE));		// ISCII Bengali
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_TA));		// ISCII Tamil
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_TE));		// ISCII Telugu
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_AS));		// ISCII Assamese
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_OR));		// ISCII Oriya
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_KA));		// ISCII Kannada
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_MA));		// ISCII Malayalam
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_GU));		// ISCII Gujarati
			a.Add(new EncodingEx(SupportedEncoding.X_ISCII_PA));		// ISCII Punjabi

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static EncodingEx Parse(string s)
		{
			EncodingEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid encoding string! String must a code page number, or one of the predefined encodings."));
		}

		/// <summary></summary>
		public static EncodingEx Parse(Encoding encoding)
		{
			return (new EncodingEx(encoding.CodePage));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out EncodingEx result)
		{
			if (s != null)
				s = s.Trim();

			foreach (EncodingInfoEx info in staticInfos)
			{
				if (StringEx.EqualsOrdinalIgnoreCase(s, info.BetterDisplayName))
				{
					result = new EncodingEx(info.SupportedEncoding);
					return (true);
				}
			}

			foreach (EncodingInfo info in Encoding.GetEncodings())
			{
				if (StringEx.EqualsOrdinalIgnoreCase(s, info.Name))
				{
					result = new EncodingEx((SupportedEncoding)info.CodePage);
					return (true);
				}

				if (StringEx.EqualsOrdinalIgnoreCase(s, info.DisplayName))
				{
					result = new EncodingEx((SupportedEncoding)info.CodePage);
					return (true);
				}
			}

			// Invalid string!
			result = null;
			return (false);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
			return (encoding.Encoding);
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
			return (new EncodingEx(codePage));
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

		#region General Auxiliary
		//==========================================================================================
		// General Auxiliary
		//==========================================================================================

		/// <summary>
		/// Returns <c>true</c> whether environment recommends to use a BOM or not.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item><description>Windows recommends BOM.</description></item>
		/// <item><description>Unix, Linux,... does not recommend BOM.</description></item>
		/// <item><description>Other environments unknown, thus also not recommending BOM.</description></item>
		/// </list>
		/// </remarks>
		public static bool EnvironmentRecommendsByteOrderMarks
		{
			get
			{
				return (EnvironmentEx.IsWindows);
			}
		}

		/// <summary>
		/// Returns the environment recommended UTF-8 encoding, i.e. with or without a BOM.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item><description>Windows recommends BOM.</description></item>
		/// <item><description>Unix, Linux,... does not recommend BOM.</description></item>
		/// <item><description>Other environments unknown, thus also not recommending BOM.</description></item>
		/// </list>
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UTF", Justification = "Same spelling as 'Encoding.UTF8'.")]
		public static Encoding EnvironmentRecommendedUTF8Encoding
		{
			get
			{
				if (EnvironmentRecommendsByteOrderMarks)
					return (Encoding.UTF8); // = UTF8Encoding(true, false);
				else
					return (new UTF8Encoding(false));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
