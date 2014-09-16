using System;
using System.Collections;

/// <summary>
/// This class was first written by Drew Noakes in Java.
///
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// If you make modifications to this code that you think would benefit the
/// wider community, please send me a copy and I'll post it on my site.
///
/// If you make use of this code, Drew Noakes will appreciate hearing 
/// about it: <a href="mailto:drew@drewnoakes.com">drew@drewnoakes.com</a>
///
/// Latest Java version of this software kept at 
/// <a href="http://drewnoakes.com">http://drewnoakes.com/</a>
///
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.drew.metadata.exif
{
	/// <summary>
	/// Data format class. Will be used one day ;-)
	/// </summary>
	[Serializable]
	public class DataFormat 
	{
		public static readonly DataFormat BYTE = new DataFormat("BYTE", 1);
		public static readonly DataFormat STRING = new DataFormat("STRING", 2);
		public static readonly DataFormat USHORT = new DataFormat("USHORT", 3);
		public static readonly DataFormat ULONG = new DataFormat("ULONG", 4);
		public static readonly DataFormat URATIONAL = new DataFormat("URATIONAL", 5);
		public static readonly DataFormat SBYTE = new DataFormat("SBYTE", 6);
		public static readonly DataFormat UNDEFINED = new DataFormat("UNDEFINED", 7);
		public static readonly DataFormat SSHORT = new DataFormat("SSHORT", 8);
		public static readonly DataFormat SLONG = new DataFormat("SLONG", 9);
		public static readonly DataFormat SRATIONAL = new DataFormat("SRATIONAL", 10);
		public static readonly DataFormat SINGLE = new DataFormat("SINGLE", 11);
		public static readonly DataFormat DOUBLE = new DataFormat("DOUBLE", 12);

		private string myName;
		private int aValue;

		/// <summary>
		/// Builds a data format
		/// </summary>
		/// <param name="aValue">the value you want (BYTE, SINGLE, ...)</param>
		/// <returns>the data format</returns>
		public static DataFormat FromValue(int aValue) 
		{
			switch (aValue) 
			{
				case 1 :
					return BYTE;
				case 2 :
					return STRING;
				case 3 :
					return USHORT;
				case 4 :
					return ULONG;
				case 5 :
					return URATIONAL;
				case 6 :
					return SBYTE;
				case 7 :
					return UNDEFINED;
				case 8 :
					return SSHORT;
				case 9 :
					return SLONG;
				case 10 :
					return SRATIONAL;
				case 11 :
					return SINGLE;
				case 12 :
					return DOUBLE;
			}

			throw new MetadataException(
				"Value '" + aValue + "' does not represent a known data format.");
		}

		/// <summary>
		/// Builds a data
		/// </summary>
		/// <param name="name">the name of the data</param>
		/// <param name="aValue">the value of the data</param>
		private DataFormat(string name, int aValue) 
		{
			myName = name;
			this.aValue = aValue;
		}

		/// <summary>
		/// Gets the value of the data (SINGLE, BYTE, ...)
		/// </summary>
		/// <returns>the value of the object (SINGLE, BYTE, ...)</returns>
		public int GetValue() 
		{
			return aValue;
		}

		/// <summary>
		/// Gets the name of the data
		/// </summary>
		/// <returns>the name of the data</returns>
		public override string ToString() 
		{
			return myName;
		}
	}
}
