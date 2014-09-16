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
	/// There are 3 formats of Nikon's MakerNote. MakerNote of E700/E800/E900/E900S/E910/E950 
	/// starts from ASCII string "Nikon". 
	/// Data format is the same as IFD, but it starts from offSet 0x08. T
	/// his is the same as Olympus except start string. 
	/// Example of actual data structure is shown below.
	/// 
	/// :0000: 4E 69 6B 6F 6E 00 02 00-00 00 4D 4D 00 2A 00 00 Nikon....MM.*...
	/// :0010: 00 08 00 1E 00 01 00 07-00 00 00 04 30 32 30 30 ............0200
	/// </summary>
	public class NikonType3MakernoteDescriptor : TagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="directory">a directory</param>
		public NikonType3MakernoteDescriptor(Directory directory) : base(directory)
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
				case NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_LENS :
					return GetLensDescription();
				case NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT :
					return GetHueAdjustmentDescription();
				case NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_CAMERA_COLOR_MODE :
					return GetColorModeDescription();
				default :
					return _directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Lens Description. 
		/// </summary>
		/// <returns>the Lens Description.</returns>
		public string GetLensDescription()  
		{
			if (!_directory
				.ContainsTag(NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_LENS))
				return null;

			Rational[] lensValues =
				_directory.GetRationalArray(
				NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_LENS);

			if (lensValues.Length != 4)
				return _directory.GetString(
					NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_LENS);

			string[] tab = new string[] {lensValues[0].IntValue().ToString(),			
											lensValues[1].IntValue().ToString(),
											lensValues[2].FloatValue().ToString(),
											lensValues[3].FloatValue().ToString()};

			return BUNDLE["LENS", tab];
		}

		/// <summary>
		/// Returns the Hue Adjustment Description. 
		/// </summary>
		/// <returns>the Hue Adjustment Description.</returns>
		public string GetHueAdjustmentDescription() 
		{
			if (!_directory
				.ContainsTag(
				NikonType3MakernoteDirectory
				.TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT))
				return null;

			return BUNDLE["DEGREES", _directory.GetString(NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT)];
		}

		/// <summary>
		/// Returns the Color Mode Description. 
		/// </summary>
		/// <returns>the Color Mode Description.</returns>
		public string GetColorModeDescription() 
		{
			if (!_directory
				.ContainsTag(
				NikonType3MakernoteDirectory
				.TAG_NIKON_TYPE3_CAMERA_COLOR_MODE))
				return null;

			string raw =
				_directory.GetString(
				NikonType3MakernoteDirectory.TAG_NIKON_TYPE3_CAMERA_COLOR_MODE);
			if (raw.StartsWith("MODE1")) 
			{
				return BUNDLE["MODE_I_SRGB"];
			}

			return raw;
		}
	}
}