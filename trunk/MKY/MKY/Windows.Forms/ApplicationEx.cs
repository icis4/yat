﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ApplicationEx
	{
		/// <summary>
		/// The default support request message is <![CDATA["Support may be requested at the origin of this software."]]>.
		/// </summary>
		public const string RequestSupportMessageDefault = "Support may be requested at the origin of this software.";

		/// <summary>
		/// The default feature request message is <![CDATA["New features can be requested at the origin of this software."]]>.
		/// </summary>
		public const string RequestFeatureMessageDefault = "New features can be requested at the origin of this software.";

		/// <summary>
		/// The default bug submission message is <![CDATA["Please report this issue at the origin of this software."]]>.
		/// </summary>
		/// <remarks>
		/// Note the preceeding space.
		/// </remarks>
		public const string SubmitBugMessageDefault = "Please report this issue at the origin of this software.";

		private static string staticRequestSupportMessage = RequestSupportMessageDefault;
		private static string staticRequestFeatureMessage = RequestFeatureMessageDefault;
		private static string staticSubmitBugMessage      = SubmitBugMessageDefault;

		private static bool staticVisualStylesAndTextRenderingIsAlreadyDoneSo;

		/// <summary>
		/// The currently active support request message.
		/// By default <see cref="RequestSupportMessageDefault"/>.
		/// </summary>
		public static string RequestSupportMessage
		{
			get { return (staticRequestSupportMessage); }
			set { staticRequestSupportMessage = value;  }
		}

		/// <summary>
		/// The currently active feature request message.
		/// By default <see cref="RequestFeatureMessageDefault"/>.
		/// </summary>
		public static string RequestFeatureMessage
		{
			get { return (staticRequestFeatureMessage); }
			set { staticRequestFeatureMessage = value;  }
		}

		/// <summary>
		/// The currently active bug submission message.
		/// By default <see cref="SubmitBugMessageDefault"/>.
		/// </summary>
		public static string SubmitBugMessage
		{
			get { return (staticSubmitBugMessage); }
			set { staticSubmitBugMessage = value;  }
		}

		/// <summary>
		/// Helper method to deal with application startup requirements of .NET 2.0 and later.
		/// </summary>
		/// <remarks>
		/// <see cref="Application.EnableVisualStyles"/> must be called before creating any
		/// controls in the application. Typically, it is the first line in the Main function.
		/// <see cref="Application.SetCompatibleTextRenderingDefault"/> has to be set to false
		/// for all controls included with Windows Forms 2.0 or later. However, this method can
		/// only be called before the first window is created by the application. This helper
		/// method ensures that this only happens once. This is particularly useful when
		/// calling application startup from a test environment such as NUnit.
		/// </remarks>
		public static bool EnableVisualStylesAndSetTextRenderingIfNotAlreadyDoneSo()
		{
			if (!staticVisualStylesAndTextRenderingIsAlreadyDoneSo)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				staticVisualStylesAndTextRenderingIsAlreadyDoneSo = true;

				return (true);
			}
			else
			{
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
