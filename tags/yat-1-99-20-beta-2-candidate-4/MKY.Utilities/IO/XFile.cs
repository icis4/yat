using System;
using System.IO;

namespace MKY.Utilities.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.File"/>.
	/// </summary>
	public static class XFile
	{
		/// <summary>
		/// Returns a unique file name for a file specified by path.
		/// </summary>
		public static string MakeUniqueFileName(string path)
		{
			return (MakeUniqueFileName(path, ""));
		}

		/// <summary>
		/// Returns a unique file name for a file specified by path, unique part is separated by separator string.
		/// </summary>
		public static string MakeUniqueFileName(string path, string separator)
		{
			string dir = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
			string name = Path.GetFileNameWithoutExtension(path);
			string ext = Path.GetExtension(path);

			if (File.Exists(dir + name + ext))
			{
				int index = -1;
				string unique = "";
				do
				{
					index++;
					unique = index.ToString();
				}
				while (File.Exists(dir + name + separator + unique + ext));
				return (dir + name + separator + unique + ext);
			}
			return (dir + name + ext);
		}
	}
}
