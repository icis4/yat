//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[Serializable]
	public class FormatException : System.FormatException
	{
		/// <summary></summary>
		public FormatException()
		{
		}

		/// <summary></summary>
		public FormatException(string message)
			: this(message, null)
		{
		}

		/// <summary></summary>
		public FormatException(string message, Exception innerException)
			: base
			(
			message + Environment.NewLine + Environment.NewLine +
			Parser.FormatHelp + Environment.NewLine + Environment.NewLine +
			Parser.KeywordHelp + Environment.NewLine + Environment.NewLine,
			innerException
			)
		{
		}

		/// <summary></summary>
		protected FormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
