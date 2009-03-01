using System;

namespace MKY.Utilities.Settings
{
	/// <summary></summary>
	public class SettingsEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly Settings Source;

		/// <summary></summary>
		public readonly SettingsEventArgs Inner;

		/// <summary></summary>
		public SettingsEventArgs(Settings source)
		{
			Source = source;
		}

		/// <summary></summary>
		public SettingsEventArgs(Settings source, SettingsEventArgs inner)
		{
			Source = source;
			Inner = inner;
		}
	}
}
