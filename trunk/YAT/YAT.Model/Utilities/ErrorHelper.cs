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
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using MKY;
using MKY.IO;

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class ErrorHelper
	{
		/// <remarks>Provides [str] and [str, str] and [str, str, str] and [str, str, str, str].</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string ComposeMessage(string lead, string leadAddOn = null, string secondaryLead = null, string secondaryText = null)
		{
			return (ComposeMessage(lead, leadAddOn, null, secondaryLead, secondaryText));
		}

		/// <remarks>Provides [str, ex].</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string ComposeMessage(string lead, Exception ex, string secondaryLead = null, string secondaryText = null)
		{
			return (ComposeMessage(lead, null, ex, secondaryLead, secondaryText));
		}

		/// <remarks>Provides [str, str, ex].</remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string ComposeMessage(string lead, string leadAddOn, Exception ex, string secondaryLead = null, string secondaryText = null)
		{
			var sb = new StringBuilder(lead);

			if (!string.IsNullOrEmpty(leadAddOn))
			{
				sb.Append(" ");
				sb.Append(leadAddOn);
			}

			if (ex != null)
			{
				if (ex is System.Xml.XmlException)
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.AppendLine("XML error message:");
					sb.Append    (ex.Message);

					var inner = ex.InnerException;
					if (inner != null)
					{
						sb.AppendLine();
						sb.AppendLine();
						sb.Append    ("File error message (");
						sb.Append    (ex.InnerException.GetType());
						sb.AppendLine(                      "):");
						sb.Append    (ex.InnerException.Message);

						inner = inner.InnerException;
						while (inner != null)
						{
							sb.AppendLine();
							sb.AppendLine();
							sb.Append    ("Additional error message (");
							sb.Append    (inner.GetType());
							sb.AppendLine(                            "):");
							sb.Append    (inner.Message);

							inner = inner.InnerException;
						}
					}
				}
				else
				{
					sb.AppendLine();
					sb.AppendLine();
					sb.Append    ("System error message (");
					sb.Append    (ex.GetType());
					sb.AppendLine(                        "):");
					sb.Append    (ex.Message);

					var inner = ex.InnerException;
					while (inner != null)
					{
						sb.AppendLine();
						sb.AppendLine();
						sb.Append    ("Additional error message (");
						sb.Append    (inner.GetType());
						sb.AppendLine(                            "):");
						sb.Append    (inner.Message);

						inner = inner.InnerException;
					}
				}
			}

			if (!string.IsNullOrEmpty(secondaryLead))
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.Append    (secondaryLead);

				if (!string.IsNullOrEmpty(secondaryText))
				{
					sb.AppendLine(); // Line break after secondary lead above.
					sb.Append    (secondaryText);
				}
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, man!")]
		public static void MakeStartHint(Domain.IOType ioType, out string yatLead, out string yatText)
		{
			switch (ioType)
			{
				case Domain.IOType.SerialPort:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Check the communication settings and keep in mind that hardware and driver may limit the allowed settings.";
					break;
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.UdpClient:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Make sure the selected remote host is available.";
					break;
				}

				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Make sure the selected socket is not already in use.";
					break;
				}

				case Domain.IOType.UsbSerialHid:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Make sure the selected USB device is ready.";
					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + ioType + "' is an I/O type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, man!")]
		public static void MakeExceptionHint(Domain.IOType ioType, out string yatLead, out string yatText)
		{
			switch (ioType)
			{
				case Domain.IOType.SerialPort:
				{
					yatLead = ApplicationEx.CommonName + " hints:";
					yatText = "Make sure the selected serial COM port is available and not already in use. " +
					          "Also, check the communication settings and keep in mind that hardware and driver may limit the allowed settings.";
					break;
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.UdpClient:
				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Make sure the selected socket is not already in use.";
					break;
				}

				case Domain.IOType.UsbSerialHid:
				{
					yatLead = ApplicationEx.CommonName + " hint:";
					yatText = "Make sure the selected USB device is connected and not already in use.";
					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + ioType + "' is an I/O type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, man!")]
		public static void MakeLogHint(Log.Provider log, out string yatLead, out string yatText)
		{
			var hintText = new StringBuilder();
			int hintCount = 0;

			if (log != null)
			{
				var path = log.Settings.RootPath;

				if (!Directory.Exists(path)) // Exists() never throws.
				{
					if (hintText.Length > 0)
						hintText.AppendLine();

					hintText.Append("Log root path does not exist. Either restore the path, or change it in the log settings.");
					hintCount++;
				}
				else
				{
					if (!DirectoryEx.IsWritable(path)) // IsWritable() never throws.
					{
						if (hintText.Length > 0)
							hintText.AppendLine();

						hintText.Append("Log root path is not writable. Either make the path writeable, or change it in the log settings.");
						hintCount++;
					}
					else
					{
						try
						{
							if (log.FileExists)
							{
								if (hintText.Length > 0)
									hintText.AppendLine();

								hintText.Append("Log file(s) could already be in use. Ensure that no other application accesses the log file(s).");
								hintCount++;
							}
						}
						catch { }
					}
				}
			}

			if (hintText.Length == 0) // No dedicated hint found => Use generic hint:
			{
				hintText.Append("Check the log settings.");
				hintCount++;
			}

			yatLead = ApplicationEx.CommonName + (hintCount <= 1 ? " hint:" : " hints:");
			yatText = hintText.ToString();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
