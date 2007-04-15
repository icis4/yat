using System;
using System.IO;

namespace HSR.Utilities.IO
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public static class XPath
	{
		/// <summary>
		/// Limits a folder or file path to the specified max length.
		/// </summary>
		public static string LimitPath(string path, int length)
		{
			string limitedPath;

			if (path.Length <= length)                 // path string too long ?
				return (path);
			                                           // local drive ?
			if (path.IndexOf(Path.VolumeSeparatorChar) < 0)
			{
				limitedPath = Types.XString.Left(path, 3) + "..." +
							  Types.XString.Right(path, Math.Max(length - 6, 0));
			}
			else                                       // network drive !
			{
				int separatorPosition = path.Substring(4).IndexOf(Path.DirectorySeparatorChar);
				if ((separatorPosition >= 0) && (separatorPosition < length - 4))
				{
					limitedPath = Types.XString.Left(path, separatorPosition) + "..." +
								  Types.XString.Right(path, Math.Max(length - 4 - separatorPosition, 0));
				}
				else
				{
					limitedPath = Types.XString.Left(path, 5) + "..." +
								  Types.XString.Right(path, Math.Max(length - 8, 0));
				}
			}

			return (Types.XString.Right(limitedPath, length));
		}
	}
}
