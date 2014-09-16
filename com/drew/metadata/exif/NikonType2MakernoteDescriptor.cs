using System;
using System.Collections;
using System.Text;
using System.IO;
using com.drew.metadata;
using com.drew.lang;

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
namespace com.drew.metadata.exif
{
	/// <summary>
	/// Tag descriptor for Nikon
	/// </summary>
	public class NikonType2MakernoteDescriptor : TagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="directory">a directory</param>
		public NikonType2MakernoteDescriptor(Directory directory) : base(directory)
		{		
		}

		/// <summary>
		/// Returns a descriptive value of the the specified tag for this image. 
		/// Where possible, known values will be substituted here in place of the raw tokens actually 
		/// kept in the Exif segment. 
		/// If no substitution is available, the value provided by GetString(int) will be returned.
		/// This and GetString(int) are the only 'get' methods that won't throw an exception.
		/// </summary>
		/// <param name="tagType">the tag to find a description for</param>
		/// <returns>a description of the image's value for the specified tag, or null if the tag hasn't been defined.</returns>
		public override string GetDescription(int tagType)  
		{
			switch(tagType) 
			{
				case NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_ISO_SETTING :
					return GetIsoSettingDescription();
				case NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_DIGITAL_ZOOM :
					return GetDigitalZoomDescription();
				case NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_AF_FOCUS_POSITION :
					return GetAutoFocusPositionDescription();
				default :
					return _directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Auto Focus Position Description. 
		/// </summary>
		/// <returns>the Auto Focus Position Description.</returns>
		private string GetAutoFocusPositionDescription()  
		{
			if (!_directory
				.ContainsTag(
				NikonType2MakernoteDirectory
				.TAG_NIKON_TYPE2_AF_FOCUS_POSITION))
				return null;
			int[] values =
				_directory.GetIntArray(
				NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_AF_FOCUS_POSITION);
			if (values.Length != 4
				|| values[0] != 0
				|| values[2] != 0
				|| values[3] != 0) 
			{
				return BUNDLE["UNKNOWN", _directory.GetString(NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_AF_FOCUS_POSITION)];
			}
			switch (values[1]) 
			{
				case 0 :
					return BUNDLE["CENTER"];
				case 1 :
					return BUNDLE["TOP"];
				case 2 :
					return BUNDLE["BOTTOM"];
				case 3 :
					return BUNDLE["LEFT"];
				case 4 :
					return BUNDLE["RIGHT"];
				default :
					return BUNDLE["UNKNOWN", values[1].ToString()];
			}
		}

		/// <summary>
		/// Returns the Digital Zoom Description. 
		/// </summary>
		/// <returns>the Digital Zoom Description.</returns>
		private string GetDigitalZoomDescription()  
		{
			if (!_directory
				.ContainsTag(
				NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_DIGITAL_ZOOM))
				return null;
			Rational rational =
				_directory.GetRational(
				NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_DIGITAL_ZOOM);
			if (rational.IntValue() == 1) 
			{
				return BUNDLE["NO_DIGITAL_ZOOM"];
			}
			return BUNDLE["DIGITAL_ZOOM", rational.ToSimpleString(true)];
		}

		/// <summary>
		/// Returns the Iso Setting Description. 
		/// </summary>
		/// <returns>the Iso Setting Description.</returns>
		private string GetIsoSettingDescription()  
		{
			if (!_directory
				.ContainsTag(
				NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_ISO_SETTING))
				return null;
			int[] values =
				_directory.GetIntArray(
				NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_ISO_SETTING);
			if (values[0] != 0 || values[1] == 0) 
			{
				return BUNDLE["UNKNOWN", _directory.GetString(NikonType2MakernoteDirectory.TAG_NIKON_TYPE2_ISO_SETTING)];
			}
			return BUNDLE["ISO", values[1].ToString()];
		}
	}
}
