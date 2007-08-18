using System;

namespace MKY.Utilities.Net
{
	/// <summary>
	/// Browser utility methods.
	/// </summary>
	public static class Browser
	{
		/// <summary>
		/// Opens the system default browser and browses url.
		/// </summary>
		/// <param name="url">URL to browse</param>
		public static void BrowseUrl(string url)
		{
			System.Diagnostics.Process.Start(url);
		}
	}
}
