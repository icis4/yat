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
// YAT Version 2.1.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY;

using YAT.Format.Settings;

#endregion

namespace YAT.Log
{
	/// <summary></summary>
	public class Provider : IDisposable, IDisposableEx
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Settings.LogSettings settings;
		private Encoding textTerminalEncoding;
		private FormatSettings neatFormat;

		private List<Log> logs;

		private TextLog controlLog;

		private List<Log> rawLogs;
		private RawLog rawTxLog;
		private RawLog rawBidirLog;
		private RawLog rawRxLog;

		private List<Log> neatLogs;
		private TextLog neatTxLog;
		private TextLog neatBidirLog;
		private TextLog neatRxLog;

		private bool isOn; // = false;

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

			var logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

			this.controlLog                     = new TextLog(this.settings.ControlLog,   new Func<string>(this.settings.MakeControlFilePath),   this.settings.WriteMode, logEncoding, this.neatFormat);

			this.rawLogs  = new List<Log>(3); // Preset the required capacity to improve memory management.
			this.rawLogs.Add(this.rawTxLog      = new RawLog( this.settings.RawLogTx,     new Func<string>(this.settings.MakeRawTxFilePath),     this.settings.WriteMode, logEncoding));
			this.rawLogs.Add(this.rawBidirLog   = new RawLog( this.settings.RawLogBidir,  new Func<string>(this.settings.MakeRawBidirFilePath),  this.settings.WriteMode, logEncoding));
			this.rawLogs.Add(this.rawRxLog      = new RawLog( this.settings.RawLogRx,     new Func<string>(this.settings.MakeRawRxFilePath),     this.settings.WriteMode, logEncoding));

			this.neatLogs = new List<Log>(3); // Preset the required capacity to improve memory management.
			this.neatLogs.Add(this.neatTxLog    = new TextLog(this.settings.NeatLogTx,    new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat));
			this.neatLogs.Add(this.neatBidirLog = new TextLog(this.settings.NeatLogBidir, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat));
			this.neatLogs.Add(this.neatRxLog    = new TextLog(this.settings.NeatLogRx,    new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat));

			this.logs = new List<Log>(7); // Preset the required capacity to improve memory management.
			this.logs.Add     (this.controlLog); // Attention: The sequence must correspond to the 'LogChannel' values!
			this.logs.AddRange(this.rawLogs);    // Attention: The sequence must correspond to the 'LogChannel' values!
			this.logs.AddRange(this.neatLogs);   // Attention: The sequence must correspond to the 'LogChannel' values!
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
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "portLog", Justification = "Logs are actually disposed via this.logs in the below foreach loop.")]
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

					this.neatTxLog   .ApplySettings(this.settings.NeatLogTx,    this.isOn, new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.ApplySettings(this.settings.NeatLogBidir, this.isOn, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog   .ApplySettings(this.settings.NeatLogRx,    this.isOn, new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
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

					this.neatTxLog   .ApplySettings(this.settings.NeatLogTx,    this.isOn, new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.ApplySettings(this.settings.NeatLogBidir, this.isOn, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog   .ApplySettings(this.settings.NeatLogRx,    this.isOn, new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
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

					var logEncoding = this.settings.ToTextEncoding(this.textTerminalEncoding);

					this.controlLog  .ApplySettings(this.settings.ControlLog,   this.isOn, new Func<string>(this.settings.MakeControlFilePath),   this.settings.WriteMode, logEncoding, this.neatFormat);

					this.rawTxLog    .ApplySettings(this.settings.RawLogTx,     this.isOn, new Func<string>(this.settings.MakeRawTxFilePath),     this.settings.WriteMode, logEncoding);
					this.rawBidirLog .ApplySettings(this.settings.RawLogBidir,  this.isOn, new Func<string>(this.settings.MakeRawBidirFilePath),  this.settings.WriteMode, logEncoding);
					this.rawRxLog    .ApplySettings(this.settings.RawLogRx,     this.isOn, new Func<string>(this.settings.MakeRawRxFilePath),     this.settings.WriteMode, logEncoding);

					this.neatTxLog   .ApplySettings(this.settings.NeatLogTx,    this.isOn, new Func<string>(this.settings.MakeNeatTxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatBidirLog.ApplySettings(this.settings.NeatLogBidir, this.isOn, new Func<string>(this.settings.MakeNeatBidirFilePath), this.settings.WriteMode, logEncoding, this.neatFormat);
					this.neatRxLog   .ApplySettings(this.settings.NeatLogRx,    this.isOn, new Func<string>(this.settings.MakeNeatRxFilePath),    this.settings.WriteMode, logEncoding, this.neatFormat);
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

				if (count == 0)
					Debug.Assert(!this.isOn, "Provider state must be 'off' if none is 'on'!");
				else
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");

				return (count);
			}
		}

		/// <summary></summary>
		public virtual bool AnyIsOn
		{
			get
			{
				foreach (var l in this.logs)
				{
					if (l.IsOn)
					{
						Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
						return (true);
					}
				}

				Debug.Assert(!this.isOn, "Provider state must be 'off' if none is 'on'!");
				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AnyControlIsOn
		{
			get
			{
				if (this.controlLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any control is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AnyRawIsOn
		{
			get
			{
				foreach (var l in this.rawLogs)
				{
					if (l.IsOn)
					{
						Debug.Assert(this.isOn, "Provider state must be 'on' if any raw is 'on'!");
						return (true);
					}
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AnyNeatIsOn
		{
			get
			{
				foreach (var l in this.neatLogs)
				{
					if (l.IsOn)
					{
						Debug.Assert(this.isOn, "Provider state must be 'on' if any neat is 'on'!");
						return (true);
					}
				}

				return (false);
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
					{
						return (false);
					}
				}

				Debug.Assert(this.isOn, "Provider state must be 'on' if all logs are 'on'!");
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

			this.isOn = true;
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

			this.isOn = false;
		}

		/// <summary></summary>
		public virtual void Write(Domain.RawChunk chunk, LogChannel writeChannel)
		{
			if (this.isOn)
			{
				var log = GetLog(writeChannel) as RawLog;
				if (log != null)
					log.Write(chunk);
			}
		}

		/// <summary></summary>
		public virtual void WriteLine(Domain.DisplayLine line, LogChannel writeChannel)
		{
			if (this.isOn)
			{
				var log = GetLog(writeChannel) as TextLog;
				if (log != null)
					log.WriteLine(line);
			}
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
				var result = new List<string>(this.logs.Count); // Preset the required capacity to improve memory management.

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
