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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class ErrorHelper
	{
		/// <summary></summary>
		public static string ComposeMessage(string lead, Exception ex = null, string additionalLead = null, string additionalText = null)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(lead);

			if (ex != null)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("System error message:");
				sb.Append(ex.Message);

				if (ex.InnerException != null)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine("Additional error message:");
					sb.Append(ex.InnerException.Message);
				}
			}

			if (!string.IsNullOrEmpty(additionalLead) && !string.IsNullOrEmpty(additionalText))
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(additionalLead);
				sb.Append(additionalText);
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		public static string ComposeMessage(string lead, string text, Exception ex = null, string additionalLead = null, string additionalText = null)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(lead);
			sb.Append(text);

			if (ex is System.Xml.XmlException)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("XML error message:");
				sb.Append(ex.Message);

				if (ex.InnerException != null)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine("File error message:");
					sb.Append(ex.InnerException.Message);
				}
			}
			else if (ex != null)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("System error message:");
				sb.Append(ex.Message);

				if (ex.InnerException != null)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine("Additional error message:");
					sb.Append(ex.InnerException.Message);
				}
			}

			if (!string.IsNullOrEmpty(additionalLead) && !string.IsNullOrEmpty(additionalText))
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(additionalLead);
				sb.Append(additionalText);
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
