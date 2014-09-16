using System;
using System.Collections;
using com.drew.metadata;
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
	/// <summary>
	/// This class represents CASIO marker note.
	/// </summary>
	public class CasioMakernoteDirectory : Directory 
	{
		public const int TAG_CASIO_RECORDING_MODE = 0x0001;
		public const int TAG_CASIO_QUALITY = 0x0002;
		public const int TAG_CASIO_FOCUSING_MODE = 0x0003;
		public const int TAG_CASIO_FLASH_MODE = 0x0004;
		public const int TAG_CASIO_FLASH_INTENSITY = 0x0005;
		public const int TAG_CASIO_OBJECT_DISTANCE = 0x0006;
		public const int TAG_CASIO_WHITE_BALANCE = 0x0007;
		public const int TAG_CASIO_UNKNOWN_1 = 0x0008;
		public const int TAG_CASIO_UNKNOWN_2 = 0x0009;
		public const int TAG_CASIO_DIGITAL_ZOOM = 0x000A;
		public const int TAG_CASIO_SHARPNESS = 0x000B;
		public const int TAG_CASIO_CONTRAST = 0x000C;
		public const int TAG_CASIO_SATURATION = 0x000D;
		public const int TAG_CASIO_UNKNOWN_3 = 0x000E;
		public const int TAG_CASIO_UNKNOWN_4 = 0x000F;
		public const int TAG_CASIO_UNKNOWN_5 = 0x0010;
		public const int TAG_CASIO_UNKNOWN_6 = 0x0011;
		public const int TAG_CASIO_UNKNOWN_7 = 0x0012;
		public const int TAG_CASIO_UNKNOWN_8 = 0x0013;
		public const int TAG_CASIO_CCD_SENSITIVITY = 0x0014;

		protected static readonly ResourceBundle BUNDLE = new ResourceBundle("CasioMarkernote");

		protected static readonly IDictionary tagNameMap = CasioMakernoteDirectory.InitTagMap();
		
		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();

			resu.Add(TAG_CASIO_CCD_SENSITIVITY,	BUNDLE["TAG_CASIO_CCD_SENSITIVITY"]);
			resu.Add(TAG_CASIO_CONTRAST, BUNDLE["TAG_CASIO_CONTRAST"]);
			resu.Add(TAG_CASIO_DIGITAL_ZOOM, BUNDLE["TAG_CASIO_DIGITAL_ZOOM"]);
			resu.Add(TAG_CASIO_FLASH_INTENSITY,	BUNDLE["TAG_CASIO_FLASH_INTENSITY"]);
			resu.Add(TAG_CASIO_FLASH_MODE, BUNDLE["TAG_CASIO_FLASH_MODE"]);
			resu.Add(TAG_CASIO_FOCUSING_MODE, BUNDLE["TAG_CASIO_FOCUSING_MODE"]);
			resu.Add(TAG_CASIO_OBJECT_DISTANCE,	BUNDLE["TAG_CASIO_OBJECT_DISTANCE"]);
			resu.Add(TAG_CASIO_QUALITY, BUNDLE["TAG_CASIO_QUALITY"]);
			resu.Add(TAG_CASIO_RECORDING_MODE, BUNDLE["TAG_CASIO_RECORDING_MODE"]);
			resu.Add(TAG_CASIO_SATURATION, BUNDLE["TAG_CASIO_SATURATION"]);
			resu.Add(TAG_CASIO_SHARPNESS, BUNDLE["TAG_CASIO_SHARPNESS"]);
			resu.Add(TAG_CASIO_UNKNOWN_1, BUNDLE["TAG_CASIO_UNKNOWN_1"]);
			resu.Add(TAG_CASIO_UNKNOWN_2, BUNDLE["TAG_CASIO_UNKNOWN_2"]);
			resu.Add(TAG_CASIO_UNKNOWN_3, BUNDLE["TAG_CASIO_UNKNOWN_3"]);
			resu.Add(TAG_CASIO_UNKNOWN_4, BUNDLE["TAG_CASIO_UNKNOWN_4"]);
			resu.Add(TAG_CASIO_UNKNOWN_5, BUNDLE["TAG_CASIO_UNKNOWN_5"]);
			resu.Add(TAG_CASIO_UNKNOWN_6, BUNDLE["TAG_CASIO_UNKNOWN_6"]);
			resu.Add(TAG_CASIO_UNKNOWN_7, BUNDLE["TAG_CASIO_UNKNOWN_7"]);
			resu.Add(TAG_CASIO_UNKNOWN_8, BUNDLE["TAG_CASIO_UNKNOWN_8"]);
			resu.Add(TAG_CASIO_WHITE_BALANCE, BUNDLE["TAG_CASIO_WHITE_BALANCE"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public CasioMakernoteDirectory() : base()
		{
			this.SetDescriptor(new CasioMakernoteDescriptor(this));
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
