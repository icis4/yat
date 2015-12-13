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
// YAT 2.0 Gamma 2 Development Version 1.99.35
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YAT.Log
{
	/// <summary></summary>
	public class Provider : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.LogSettings settings;

		private List<Log> logs;
		private List<Log> rawLogs;
		private List<Log> neatLogs;

		private BinaryLog rawTxLog;
		private BinaryLog rawBidirLog;
		private BinaryLog rawRxLog;

		private TextLog neatTxLog;
		private TextLog neatBidirLog;
		private TextLog neatRxLog;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Provider(Settings.LogSettings settings)
		{
			this.settings = settings;

			this.logs     = new List<Log>();
			this.rawLogs  = new List<Log>();
			this.neatLogs = new List<Log>();

			this.rawLogs.Add(this.rawTxLog    = new BinaryLog(this.settings.RawLogTx,    this.settings.RawTxFilePath,    this.settings.WriteMode, this.settings.NameSeparator));
			this.rawLogs.Add(this.rawBidirLog = new BinaryLog(this.settings.RawLogBidir, this.settings.RawBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.rawLogs.Add(this.rawRxLog    = new BinaryLog(this.settings.RawLogRx,    this.settings.RawRxFilePath,    this.settings.WriteMode, this.settings.NameSeparator));

			this.neatLogs.Add(this.neatTxLog    = new TextLog(this.settings.NeatLogTx,    this.settings.NeatTxFilePath,    this.settings.WriteMode, this.settings.NameSeparator));
			this.neatLogs.Add(this.neatBidirLog = new TextLog(this.settings.NeatLogBidir, this.settings.NeatBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.neatLogs.Add(this.neatRxLog    = new TextLog(this.settings.NeatLogRx,    this.settings.NeatRxFilePath,    this.settings.WriteMode, this.settings.NameSeparator));

			this.logs.AddRange(this.rawLogs);
			this.logs.AddRange(this.neatLogs);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, End() has already been called.
					SwitchOff();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Provider()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Settings.LogSettings Settings
		{
			get { return (this.settings); }
			set
			{
				this.settings = value;

				this.rawTxLog.SetSettings   (this.settings.RawLogTx,    this.settings.RawTxFilePath,    this.settings.WriteMode, this.settings.NameSeparator);
				this.rawBidirLog.SetSettings(this.settings.RawLogBidir, this.settings.RawBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.rawRxLog.SetSettings   (this.settings.RawLogRx,    this.settings.RawRxFilePath,    this.settings.WriteMode, this.settings.NameSeparator);

				this.neatTxLog.SetSettings   (this.settings.NeatLogTx,    this.settings.NeatTxFilePath,    this.settings.WriteMode, this.settings.NameSeparator);
				this.neatBidirLog.SetSettings(this.settings.NeatLogBidir, this.settings.NeatBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.neatRxLog.SetSettings   (this.settings.NeatLogRx,    this.settings.NeatRxFilePath,    this.settings.WriteMode, this.settings.NameSeparator);
			}
		}

		/// <summary></summary>
		public virtual int EnabledCount
		{
			get
			{
				int count = 0;
				foreach (Log l in this.logs)
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
				foreach (Log l in this.logs)
				{
					isOn |= l.IsOn;
				}
				return (isOn);
			}
		}

		/// <summary></summary>
		public virtual bool FileExists
		{
			get
			{
				bool fileExsists = false;
				foreach (Log l in this.logs)
				{
					fileExsists |= l.FileExists;
				}
				return (fileExsists);
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
			foreach (Log l in this.logs)
				l.Open();
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			foreach (Log l in this.logs)
				l.Truncate();
		}

		/// <summary></summary>
		public virtual void Flush()
		{
			foreach (Log l in this.logs)
				l.Flush();
		}

		/// <summary></summary>
		public virtual void SwitchOff()
		{
			foreach (Log l in this.logs)
				l.Close();
		}

		/// <summary></summary>
		public virtual void WriteByte(byte value, LogChannel writeChannel)
		{
			((BinaryLog)GetLog(writeChannel)).WriteByte(value);
		}

		/// <summary></summary>
		public virtual void WriteBytes(ReadOnlyCollection<byte> values, LogChannel writeChannel)
		{
			((BinaryLog)GetLog(writeChannel)).WriteBytes(values);
		}

		/// <summary></summary>
		public virtual void WriteString(string value, LogChannel writeChannel)
		{
			((TextLog)GetLog(writeChannel)).WriteString(value);
		}

		/// <summary></summary>
		public virtual void WriteEol(LogChannel writeChannel)
		{
			((TextLog)GetLog(writeChannel)).WriteEol();
		}

		/// <summary></summary>
		private Log GetFile(LogChannel channel)
		{
			return (this.logs[channel.GetHashCode()]);
		}

		/// <summary></summary>
		private Log GetLog(LogChannel channel)
		{
			return (this.logs[channel.GetHashCode()]);
		}

		/// <summary></summary>
		public virtual IList<string> GetFilePaths()
		{
			List<string> result = new List<string>();

			foreach (Log l in this.logs)
			{
				if (l.IsEnabled)
					result.Add(l.FilePath);
			}

			return (result);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
