using System;
using System.Text;

/// <summary>
/// This class was first written by Drew Noakes in Java.
///
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this header in tact.
///
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.utils
{
	/// <summary>
	/// Class that try to recreate some Java functionnalities.
	/// </summary>
	public sealed class Utils
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <exception cref="UnauthorizedAccessException">always</exception>
		private Utils()
		{
			throw new UnauthorizedAccessException("Do not use");
		}

		/// <summary>
		/// Builds a string from a byte array
		/// </summary>
		/// <param name="anArray">the array of byte</param>
		/// <param name="offset">where to start</param>
		/// <param name="length">the length to transform in string</param>
		/// <param name="removeSpace">if true, spaces will be avoid</param>
		/// <returns>a string representing the array of byte</returns>
		public static string Decode(byte[] anArray, int offset, int length, bool removeSpace) 
		{
			StringBuilder sb = new StringBuilder(length);
			for(int i=offset; i<length+offset; i++) 
			{
				char aChar = (char)anArray[i];
				if (removeSpace && (anArray[i] == 0)) 
				{
					continue;
				}
				sb.Append(aChar);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Builds a string from a byte array
		/// </summary>
		/// <param name="anArray">the array of byte</param>
		/// <param name="removeSpace">if true, spaces will be avoid</param>
		/// <returns>a string representing the array of byte</returns>
		public static string Decode(byte[] anArray, bool removeSpace) 
		{
			return Decode(anArray, 0, anArray.Length, removeSpace);
		}

	}
}
