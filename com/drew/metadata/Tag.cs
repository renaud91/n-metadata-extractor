using System;
using System.IO;

/// <summary>
/// This class was first written by Drew Noakes in Java.
///
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this header in tact.
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
namespace com.drew.metadata
{
	/// <summary>
	/// This class represent a basic tag
	/// </summary>
	[Serializable]
	public class Tag 
	{
		private int tagType;
		private Directory directory;

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aTagType">the type of this tag</param>
		/// <param name="aDirectory">the directory of this tag</param>
		public Tag(int aTagType, Directory aDirectory) : base()
		{
			tagType = aTagType;
			directory = aDirectory;
		}

		/// <summary>
		/// Gets the tag type as an int 
		/// </summary>
		/// <returns>the tag type as an int</returns>
		public int GetTagType() 
		{
			return tagType;
		}

		/// <summary>
		/// Gets the tag type in hex notation as a string with padded leading zeroes if necessary (i.e. 0x100E). 
		/// </summary>
		/// <returns>the tag type as a string in hexadecimal notation</returns>
		public string GetTagTypeHex() 
		{
			string hex = tagType.ToString("X");
			while (hex.Length < 4)
				hex = "0" + hex;
			return "0x" + hex;
		}

		/// <summary>
		/// Get a description of the tag's value, considering enumerated values and units. 
		/// </summary>
		/// <returns>a description of the tag's value</returns>
		public string GetDescription() 
		{
			return directory.GetDescription(tagType);
		}

		/// <summary>
		/// Get the name of the tag, such as Aperture, or InteropVersion.
		/// </summary>
		/// <returns>the tag's name</returns>
		public string GetTagName() 
		{
			return directory.GetTagName(tagType);
		}

		/// <summary>
		/// Get the name of the directory in which the tag exists, such as Exif, GPS or Interoperability. 
		/// </summary>
		/// <returns>name of the directory in which this tag exists</returns>
		public string GetDirectoryName() 
		{
			return directory.GetName();
		}

		/// <summary>
		/// A basic representation of the tag's type and value in format: FNumber - F2.8. 
		/// </summary>
		/// <returns>the tag's type and value</returns>
		public override string ToString() 
		{
			string description;
			try 
			{
				description = GetDescription();
			} 
			catch (MetadataException ) 
			{
				description =
					directory.GetString(GetTagType())
					+ " (unable to formulate description)";
			}
			return "["
				+ directory.GetName()
				+ "] "
				+ GetTagName()
				+ " - "
				+ description;
		}
	}
}
