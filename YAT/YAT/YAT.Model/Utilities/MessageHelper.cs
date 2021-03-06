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
// YAT Version 2.4.1
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Collections;
using MKY.IO;

using YAT.Format.Types;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class MessageHelper
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
			var text = new StringBuilder(lead);

			if (!string.IsNullOrEmpty(leadAddOn))
			{
				text.Append(" ");
				text.Append(leadAddOn);
			}

			if (ex != null)
			{
				if (ex is System.Xml.XmlException)
				{
					text.AppendLine();
					text.AppendLine();
					text.AppendLine("XML error message:");
					text.Append    (ex.Message);

					var inner = ex.InnerException;
					if (inner != null)
					{
						text.AppendLine();
						text.AppendLine();
						text.Append    ("File error message (");
						text.Append    (ex.InnerException.GetType());
						text.AppendLine(                      "):");
						text.Append    (ex.InnerException.Message);

						inner = inner.InnerException;
						while (inner != null)
						{
							text.AppendLine();
							text.AppendLine();
							text.Append    ("Additional error message (");
							text.Append    (inner.GetType());
							text.AppendLine(                            "):");
							text.Append    (inner.Message);

							inner = inner.InnerException;
						}
					}
				}
				else
				{
					text.AppendLine();
					text.AppendLine();
					text.Append    ("System error message (");
					text.Append    (ex.GetType());
					text.AppendLine(                        "):");
					text.Append    (ex.Message);

					var inner = ex.InnerException;
					while (inner != null)
					{
						text.AppendLine();
						text.AppendLine();
						text.Append    ("Additional error message (");
						text.Append    (inner.GetType());
						text.AppendLine(                            "):");
						text.Append    (inner.Message);

						inner = inner.InnerException;
					}
				}
			}

			if (!string.IsNullOrEmpty(secondaryLead))
			{
				text.AppendLine();
				text.AppendLine();
				text.Append    (secondaryLead);

				if (!string.IsNullOrEmpty(secondaryText))
				{
					text.AppendLine(); // Line break after secondary lead above.
					text.Append    (secondaryText);
				}
			}

			return (text.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void MakeCommandLineErrorMessage(ICollection<string> invalidArgs, IReadOnlyList<string> invalidationMessages, out StringBuilder text)
		{
			text = new StringBuilder();

			text.Append(ApplicationEx.ProductName); // "YAT" or "YATConsole", as indicated in main title bar.
			text.Append(" could not be launched because the command line is invalid!");

			var hasInvalidArgs = !ICollectionEx.IsNullOrEmpty((ICollection)invalidArgs);
			if (hasInvalidArgs)
			{
				text.AppendLine(); // Add separating empty line as the following message is not a proper sentence.
				text.AppendLine();
				text.Append(((invalidArgs.Count == 1) ? "Invalid argument: " : "Invalid arguments: "));
				text.Append(IEnumerableEx.ItemsToString(invalidArgs, '"'));
			}

			if (!ICollectionEx.IsNullOrEmpty((ICollection)invalidationMessages))
			{
				// Prepending something like "Additional information:" makes little sense, the messages are good ennough.
				if (invalidationMessages.Count == 1)
				{
					if (hasInvalidArgs) // Add separating empty line only if invalid args are already given:
					{
						text.AppendLine();
						text.AppendLine();
					}

					text.Append(" ");
					text.Append(invalidationMessages[0]);
				}
				else
				{
					text.AppendLine(); // Add separating empty line in any case:
					text.AppendLine();

					for (int i = 0; i < invalidationMessages.Count; i++)
					{
						if (i < (invalidationMessages.Count - 1))
							text.AppendLine(invalidationMessages[i]);
						else
							text.Append(invalidationMessages[i]);
					}
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static void MakeMissingFontMessage(string defaultFontName, Exception exceptionOnFailure, out StringBuilder text, out List<LinkLabel.Link> links)
		{
			text = new StringBuilder();
			links = new List<LinkLabel.Link>();

			var lead = "The " + ApplicationEx.CommonName + " default font '" + defaultFontName + "' is not available!";
			if (exceptionOnFailure is NotSupportedException) // Makes little sense to replicate this information.
				text.Append(lead);
			else
				text.Append(ComposeMessage(lead, exceptionOnFailure));

			text.AppendLine();
			text.AppendLine();

			text.Append("The font gets installed along with " + ApplicationEx.CommonName + " when using one of the installer packages available at ");
			var linkStart = text.Length;
			var textLink = "SourceForge.net";
			links.Add(new LinkLabel.Link(linkStart, textLink.Length, "https://sourceforge.net/projects/y-a-terminal/"));
			text.Append(textLink);
			text.Append(".");

			text.AppendLine();
			text.AppendLine();

			string[] fontFilePaths;
			try
			{
				fontFilePaths = Directory.GetFiles(ApplicationEx.ExecutableDirectoryPath, FontFormat.FileNameDefault, SearchOption.AllDirectories);
			}
			catch
			{
				fontFilePaths = null;
			}

			if (!ArrayEx.IsNullOrEmpty(fontFilePaths))
			{
				var fontSubdirectoryPath = PathEx.GetDirectoryPath(fontFilePaths[0]);

				text.Append("The font is also available in the ");
				linkStart = text.Length;
				textLink = PathEx.GetDirectoryNameOnly(fontSubdirectoryPath);
				links.Add(new LinkLabel.Link(linkStart, textLink.Length, fontSubdirectoryPath));
				text.Append(textLink);
				text.Append(" subdirectory of the running application.");
			}
			else
			{
				text.Append("The font is also available in any of the YAT binary packages at the link above or can be downloaded from the ");
				linkStart = text.Length;
				textLink = "DejaVu Github page";
				links.Add(new LinkLabel.Link(linkStart, textLink.Length, "https://dejavu-fonts.github.io/"));
				text.Append(textLink);
				text.Append(".");
			}

			text.Append(" Installing the font requires administrator privileges.");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Monospaced", Justification = "'Monospaced' is a correct English term.")]
		public static string MakeFontMonospacedRecommendation()
		{
			return ("For best readability, it is highly recommended to use a monospaced font.");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Monospaced", Justification = "'Monospaced' is a correct English term.")]
		public static void MakeFontNotMonospacedMessage(string terminalName, string currentFontName, out StringBuilder text)
		{
			text = new StringBuilder();

			text.Append("The font of '");
			text.Append(terminalName);
			text.Append("' is currently configured to '");
			text.Append(currentFontName);
			text.Append("' which is not monospaced. ");
			text.Append(MakeFontMonospacedRecommendation());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeSerialPortStartHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hint:";
			yatText = "Check the communication settings and keep in mind that hardware and driver may limit the allowed settings.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeSerialPortExceptionHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hints:";
			yatText = "Make sure the selected serial COM port is available and not already in use. " +
			          "Also, check the communication settings and keep in mind that hardware and driver may limit the allowed settings.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeIPClientStartHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hint:";
			yatText = "Make sure the selected remote host is available.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeIPClientExceptionHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hint:";
			yatText = "Check the network settings and state.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeTcpListenerHint(int port, out string yatLead, out string yatText)
		{
			var portIsInUse = false;
			var globalProperties = IPGlobalProperties.GetIPGlobalProperties();
			var listenerInfos = globalProperties.GetActiveTcpListeners();
			foreach (var li in listenerInfos)
			{
				if (li.Port == port)
				{
					portIsInUse = true;
					break;
				}
			}

			if (portIsInUse)
			{
				yatLead = ApplicationEx.CommonName + " hint:";
				yatText = "The selected TCP port " + port + " is already in use. " +
				          "Either select a different port or close the process that currently uses it. " +
				          "Use e.g. https://www.nirsoft.net/utils/cports.html to identify the process that uses the port.";
			}
			else
			{
				yatLead = ApplicationEx.CommonName + " hint:";
				yatText = "Make sure the selected TCP/IP endpoint is not already in use.";
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeUdpListenerHint(int port, out string yatLead, out string yatText)
		{
			var portIsInUse = false;
			var globalProperties = IPGlobalProperties.GetIPGlobalProperties();
			var listenerInfos = globalProperties.GetActiveUdpListeners();
			foreach (var li in listenerInfos)
			{
				if (li.Port == port)
				{
					portIsInUse = true;
					break;
				}
			}

			if (portIsInUse)
			{
				yatLead = ApplicationEx.CommonName + " hint:";
				yatText = "The selected UDP port " + port + " is already in use. " +
				          "Either select a different port or close the process that currently uses it. " +
				          "Use e.g. https://www.nirsoft.net/utils/cports.html to identify the process that uses the port.";
			}
			else
			{
				yatLead = ApplicationEx.CommonName + " hint:";
				yatText = "Make sure the selected UDP/IP endpoint is not already in use.";
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeUsbSerialHidStartHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hint:";
			yatText = "Make sure the selected USB device is ready.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeUsbSerialHidExceptionHint(out string yatLead, out string yatText)
		{
			yatLead = ApplicationEx.CommonName + " hint:";
			yatText = "Check the selected USB device and its corresponding driver.";
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		public static void MakeLogHint(Log.Provider log, out string yatLead, out string yatText)
		{
			var hintText = new StringBuilder();
			int hintCount = 0;

			if (log != null)
			{
				var directoryPath = log.Settings.RootDirectoryPath;

				if (!Directory.Exists(directoryPath)) // Exists() never throws.
				{
					if (hintText.Length > 0)
						hintText.AppendLine();

					hintText.Append("Log root path does not exist. Either restore the path, or change it in the log settings.");
					hintCount++;
				}
				else
				{
					if (!DirectoryEx.IsWritable(directoryPath)) // IsWritable() never throws.
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
