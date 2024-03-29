﻿using System;
using System.IO;
using System.Text;

namespace ServiceStack.Common.Utils
{
	public static class PathUtils
	{
		public static string MapAbsolutePath(string relativePath, string appendPartialPathModifier)
		{
			if (relativePath.StartsWith("~"))
			{
				string assemblyDirectoryPath = Path.GetDirectoryName(new Uri(typeof(PathUtils).Assembly.EscapedCodeBase).LocalPath);

				// Escape the assembly bin directory to the hostname directory
				string hostDirectoryPath = assemblyDirectoryPath + appendPartialPathModifier;

				return Path.GetFullPath(relativePath.Replace("~", hostDirectoryPath));
			}

			return relativePath;
		}

		public static string MapAbsolutePath(this string relativePath)
		{
			var mapPath = MapAbsolutePath(relativePath, string.Format("{0}..{0}..", Path.DirectorySeparatorChar));
			return mapPath;
		}

		public static string MapHostAbsolutePath(this string relativePath)
		{
			var mapPath = MapAbsolutePath(relativePath, string.Format("{0}..", Path.DirectorySeparatorChar));
			return mapPath;
		}

		public static string CombinePaths(params string[] paths)
		{
			var sb = new StringBuilder();
			foreach (var path in paths)
			{
				if (sb.Length > 0)
					sb.Append("/");
				
				sb.Append(path.TrimStart('/','\\'));
			}

			return sb.ToString();
		}
	}


}
