using System;
using com.drew.metadata;

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
namespace com.drew.imaging.jpg
{
	/// <summary>
	/// A default tag descriptor class
	/// </summary>
	public class DefaultTagDescriptor : AbstractTagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
        /// <param name="aDirectory">its directory</param>
		public DefaultTagDescriptor(AbstractDirectory aDirectory) : base(aDirectory)
		{
		}

		/// <summary>
		/// Gets the tag name
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the tag name</returns>
		public string GetTagName(int aTagType) 
		{			
			string lcHex = aTagType.ToString("X");
            while (lcHex.Length < 4)
            {
                lcHex = "0" + lcHex;
            }
			return "Unknown tag 0x" + lcHex;
		}

		/// <summary>
		/// Returns a descriptive value of the the specified tag for this image. 
		/// Where possible, known values will be substituted here in place of the raw tokens actually 
		/// kept in the Exif segment. 
		/// If no substitution is available, the value provided by GetString(int) will be returned.
		/// This and GetString(int) are the only 'get' methods that won't throw an exception.
		/// </summary>
		/// <param name="aTagType">the tag to find a description for</param>
		/// <returns>a description of the image'str value for the specified tag, or null if the tag hasn't been defined.</returns>
		public override string GetDescription(int aTagType) 
		{
			return base.directory.GetString(aTagType);
		}
	}
}
