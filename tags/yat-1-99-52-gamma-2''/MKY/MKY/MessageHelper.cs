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
// MKY Version 1.0.17
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

namespace MKY
{
	/// <summary>
	/// User message utility methods.
	/// </summary>
	public static class MessageHelper
	{
		/// <summary>
		/// The default invalid execution preamble is <![CDATA["Program execution should never get here! "]]>.
		/// </summary>
		/// <remarks>
		/// Note the adjacent space.
		/// </remarks>
		public const string InvalidExecutionPreambleDefault = "Program execution should never get here! ";

		/// <summary>
		/// The default support request message is <![CDATA["Support may be requested at the origin of this software."]]>.
		/// </summary>
		public const string RequestSupportDefault = "Support may be requested at the origin of this software.";

		/// <summary>
		/// The default feature request message is <![CDATA["New features can be requested at the origin of this software."]]>.
		/// </summary>
		public const string RequestFeatureDefault = "New features can be requested at the origin of this software.";

		/// <summary>
		/// The default bug submission message is <![CDATA["Please report this issue at the origin of this software."]]>.
		/// </summary>
		public const string SubmitBugDefault = "Please report this issue at the origin of this software.";

		private static string staticInvalidExecutionPreamble = InvalidExecutionPreambleDefault;
		private static string staticRequestSupport           = RequestSupportDefault;
		private static string staticRequestFeature           = RequestFeatureDefault;
		private static string staticSubmitBug                = SubmitBugDefault;

		/// <summary>
		/// The currently active invalid execution preamble.
		/// By default <see cref="InvalidExecutionPreambleDefault"/>.
		/// </summary>
		public static string InvalidExecutionPreamble
		{
			get { return (staticInvalidExecutionPreamble); }
			set { staticInvalidExecutionPreamble = value;  }
		}

		/// <summary>
		/// The currently active support request message.
		/// By default <see cref="RequestSupportDefault"/>.
		/// </summary>
		public static string RequestSupport
		{
			get { return (staticRequestSupport); }
			set { staticRequestSupport = value;  }
		}

		/// <summary>
		/// The currently active feature request message.
		/// By default <see cref="RequestFeatureDefault"/>.
		/// </summary>
		public static string RequestFeature
		{
			get { return (staticRequestFeature); }
			set { staticRequestFeature = value;  }
		}

		/// <summary>
		/// The currently active bug submission message.
		/// By default <see cref="SubmitBugDefault"/>.
		/// </summary>
		public static string SubmitBug
		{
			get { return (staticSubmitBug); }
			set { staticSubmitBug = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
