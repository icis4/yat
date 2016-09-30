//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Runtime.Serialization;

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
		protected FormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
