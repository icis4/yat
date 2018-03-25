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
// YAT Version 2.0.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
using System.Text;

using YAT.Format.Settings;

#endregion

namespace YAT.Log
{
	/// <summary></summary>
	public class Provider : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Settings.LogSettings settings;
		private Encoding textTerminalEncoding;
		private FormatSettings neatFormat;

		private List<Log> logs;
		private List<Log> rawLogs;
		private List<Log> neatLogs;

		private RawLog rawTxLog;
		private RawLog rawBidirLog;
		private RawLog rawRxLog;

		private NeatLog neatTxLog;
		private NeatLog neatBidirLog;
		private NeatLog neatRxLog;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Provider(Settings.LogSettings settings, Encoding textTerminalEncoding, FormatSettings neatFormat)
		{
			this.settings             = settings;
			this.textTerminalEncoding = textTerminalEncoding;
			this.neatFormat           = neatFormat;

			this.rawLogs  = new List<Log>(3); // Preset the required capacity to improve memory management.
			this.rawLogs.Add(this.rawTxLog    = new RawLog(this.settings.RawLogTx,    new Func<string>(this.settings.MakeRawTxFilePath),    this.settings.WriteMode));
			this.rawLogs.Add(this.rawBidirLog = new RawLog(this.settings.RawLogBidir, new Func<string>(this.settings.MakeRawBidirFilePath), this.settings.WriteMode));
			this.rawLogs.Add(this.rawRxLog    = new RawLog(this.settings.RawLogRx,    new Func<string>(this.settings.MakeRawRxFilePath),    this.settings.WriteMode));

			Encoding logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

			this.neatLogs = new List<Log>(3); // Preset the required capacity to improve memory management.
			this.neatLogs.Add(this.neatTxLog    = new NeatLog(this.settings.NeatLogTx,    new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat));
			this.neatLogs.Add(this.neatBidirLog = new NeatLog(this.settings.NeatLogBidir, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat));
			this.neatLogs.Add(this.neatRxLog    = new NeatLog(this.settings.NeatLogRx,    new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat));

			this.logs = new List<Log>(2); // Preset the required capacity to improve memory management.
			this.logs.AddRange(this.rawLogs);
			this.logs.AddRange(this.neatLogs);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, SwitchOff() has already been called.
					if (this.logs != null)
					{
						foreach (var l in this.logs)
							l.Dispose();
					}
				}

				// Set state to disposed:
				IsDisposed = true;
			}
		}

	#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Provider()
		{
			Dispose(false);

			MKY.Diagnostics.DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Encoding TextTerminalEncoding
		{
			get { return (this.textTerminalEncoding); }
			set
			{
				if (this.textTerminalEncoding != value)
				{
					this.textTerminalEncoding = value;

					Encoding logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

					this.neatTxLog.SetSettings   (this.settings.NeatLogTx,    new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.SetSettings(this.settings.NeatLogBidir, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog.SetSettings   (this.settings.NeatLogRx,    new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
				}
			}
		}

		/// <summary></summary>
		public virtual FormatSettings NeatFormat
		{
			get { return (this.neatFormat); }
			set
			{
				if (this.neatFormat != value)
				{
					this.neatFormat = value;

					Encoding logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

					this.neatTxLog.SetSettings   (this.settings.NeatLogTx,    new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.SetSettings(this.settings.NeatLogBidir, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog.SetSettings   (this.settings.NeatLogRx,    new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
				}
			}
		}

		/// <summary></summary>
		public virtual Settings.LogSettings Settings
		{
			get { return (this.settings); }
			set
			{
				if (this.settings != value)
				{
					this.settings = value;

					this.rawTxLog.SetSettings   (this.settings.RawLogTx,    new Func<string>(this.settings.MakeRawTxFilePath),    this.settings.WriteMode);
					this.rawBidirLog.SetSettings(this.settings.RawLogBidir, new Func<string>(this.settings.MakeRawBidirFilePath), this.settings.WriteMode);
					this.rawRxLog.SetSettings   (this.settings.RawLogRx,    new Func<string>(this.settings.MakeRawRxFilePath),    this.settings.WriteMode);

					var logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

					this.neatTxLog.SetSettings   (this.settings.NeatLogTx,    new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.SetSettings(this.settings.NeatLogBidir, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog.SetSettings   (this.settings.NeatLogRx,    new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
				}
			}
		}

		/// <summary></summary>
		public virtual int EnabledCount
		{
			get
			{
				int count = 0;
				foreach (var l in this.logs)
				{
					if (l.IsEnabled)
						count++;
				}
				return (count);
			}
		}

		/// <summary></summary>
		public virtual bool IsOn
		{
			get
			{
				bool isOn = false;
				foreach (var l in this.logs)
				{
					isOn |= l.IsOn;
				}
				return (isOn);
			}
		}

		/// <summary></summary>
		public virtual bool AllAreOn
		{
			get
			{
				foreach (var l in this.logs)
				{
					if (!l.IsOn)
						return (false);
				}
				return (true);
			}
		}

		/// <summary></summary>
		public virtual bool FileExists
		{
			get
			{
				bool fileExists = false;
				foreach (var l in this.logs)
				{
					fileExists |= l.FileExists;
				}
				return (fileExists);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SwitchOn()
		{
			foreach (var l in this.logs)
				l.Open();
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			foreach (var l in this.logs)
				l.Clear();
		}

		/// <summary></summary>
		public virtual void Flush()
		{
			foreach (var l in this.logs)
				l.Flush();
		}

		/// <summary></summary>
		public virtual void SwitchOff()
		{
			foreach (var l in this.logs)
				l.Close();
		}

		/// <summary></summary>
		public virtual void Write(Domain.RawChunk chunk, LogChannel writeChannel)
		{
			((RawLog)GetLog(writeChannel)).Write(chunk);
		}

		/// <summary></summary>
		public virtual void WriteLine(Domain.DisplayLine line, LogChannel writeChannel)
		{
			((NeatLog)GetLog(writeChannel)).WriteLine(line);
		}

		/// <summary></summary>
		private Log GetLog(LogChannel channel)
		{
			return (this.logs[channel.GetHashCode()]);
		}

		/// <summary></summary>
		public virtual IList<string> FilePaths
		{
			get
			{
				var result = new List<string>(this.logs.Count); // Preset the initial capacity to improve memory management.

				foreach (var l in this.logs)
				{
					if (l.IsEnabled)
						result.Add(l.FilePath);
				}

				return (result);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
