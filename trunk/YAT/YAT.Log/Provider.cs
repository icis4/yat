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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
	public class Provider : DisposableBase
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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "controlLog", Justification = "Log is actually disposed via 'this.logs' in the below loop.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "portLog", Justification = "Log is actually disposed via 'this.logs' in the below loop.")]
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				// In the 'normal' case, SwitchOff() has already been called.
				if (this.logs != null)
				{
					foreach (var l in this.logs)
						l.Dispose();
				}
			}
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
		public virtual bool ControlIsOn
		{
			get
			{
				if (this.controlLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AnyControlIsOn
		{
			get { return (ControlIsOn); } // Just a single control channel.
		}

		/// <summary></summary>
		public virtual bool RawTxIsOn
		{
			get
			{
				if (this.rawTxLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool RawBidirIsOn
		{
			get
			{
				if (this.rawBidirLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool RawRxIsOn
		{
			get
			{
				if (this.rawRxLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
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
						Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
						return (true);
					}
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool NeatTxIsOn
		{
			get
			{
				if (this.neatTxLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool NeatBidirIsOn
		{
			get
			{
				if (this.neatBidirLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool NeatRxIsOn
		{
			get
			{
				if (this.neatRxLog.IsOn)
				{
					Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
					return (true);
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
						Debug.Assert(this.isOn, "Provider state must be 'on' if any is 'on'!");
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
