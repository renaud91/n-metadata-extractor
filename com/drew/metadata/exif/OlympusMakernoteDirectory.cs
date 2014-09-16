using System;
using System.Collections;
using System.Text;
using System.IO;
using com.drew.metadata;
using com.drew.lang;
using com.utils;

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
	public class OlympusMakernoteDirectory : Directory 
	{
		public const int TAG_OLYMPUS_SPECIAL_MODE = 0x0200;
		public const int TAG_OLYMPUS_JPEG_QUALITY = 0x0201;
		public const int TAG_OLYMPUS_MACRO_MODE = 0x0202;
		public const int TAG_OLYMPUS_UNKNOWN_1 = 0x0203;
		public const int TAG_OLYMPUS_DIGI_ZOOM_RATIO = 0x0204;
		public const int TAG_OLYMPUS_UNKNOWN_2 = 0x0205;
		public const int TAG_OLYMPUS_UNKNOWN_3 = 0x0206;
		public const int TAG_OLYMPUS_FIRMWARE_VERSION = 0x0207;
		public const int TAG_OLYMPUS_PICT_INFO = 0x0208;
		public const int TAG_OLYMPUS_CAMERA_ID = 0x0209;
		public const int TAG_OLYMPUS_DATA_DUMP = 0x0F00;

		protected static readonly ResourceBundle BUNDLE = new ResourceBundle("OlympusMarkernote");
		protected static readonly IDictionary tagNameMap = OlympusMakernoteDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();
			resu.Add(TAG_OLYMPUS_SPECIAL_MODE, BUNDLE["TAG_OLYMPUS_SPECIAL_MODE"]);
			resu.Add(TAG_OLYMPUS_JPEG_QUALITY, BUNDLE["TAG_OLYMPUS_JPEG_QUALITY"]);
			resu.Add(TAG_OLYMPUS_MACRO_MODE, BUNDLE["TAG_OLYMPUS_MACRO_MODE"]);
			resu.Add(TAG_OLYMPUS_UNKNOWN_1, BUNDLE["TAG_OLYMPUS_UNKNOWN_1"]);
			resu.Add(TAG_OLYMPUS_DIGI_ZOOM_RATIO, BUNDLE["TAG_OLYMPUS_DIGI_ZOOM_RATIO"]);
			resu.Add(TAG_OLYMPUS_UNKNOWN_2, BUNDLE["TAG_OLYMPUS_UNKNOWN_2"]);
			resu.Add(TAG_OLYMPUS_UNKNOWN_3, BUNDLE["TAG_OLYMPUS_UNKNOWN_3"]);
			resu.Add(TAG_OLYMPUS_FIRMWARE_VERSION, BUNDLE["TAG_OLYMPUS_FIRMWARE_VERSION"]);
			resu.Add(TAG_OLYMPUS_PICT_INFO, BUNDLE["TAG_OLYMPUS_PICT_INFO"]);
			resu.Add(TAG_OLYMPUS_CAMERA_ID, BUNDLE["TAG_OLYMPUS_CAMERA_ID"]);
			resu.Add(TAG_OLYMPUS_DATA_DUMP, BUNDLE["TAG_OLYMPUS_DATA_DUMP"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public OlympusMakernoteDirectory() : base()
		{
			this.SetDescriptor(new OlympusMakernoteDescriptor(this));
		}

		/// <summary>
		/// Provides the name of the directory, for display purposes.  E.g. Exif 
		/// </summary>
		/// <returns>the name of the directory</returns>
		public override string GetName() 
		{
			return BUNDLE["MARKER_NOTE_NAME"];
		}

		/// <summary>
		/// Provides the map of tag names, hashed by tag type identifier. 
		/// </summary>
		/// <returns>the map of tag names</returns>
		protected override IDictionary GetTagNameMap() 
		{
			return tagNameMap;
		}
	}
}
