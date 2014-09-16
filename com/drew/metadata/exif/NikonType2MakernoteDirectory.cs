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
	public class NikonType2MakernoteDirectory : NikonTypeMakernoteDirectory 
	{
		// TYPE2 is for E990, D1 and later
		public const int TAG_NIKON_TYPE2_UNKNOWN_1 = 0x0001;
		public const int TAG_NIKON_TYPE2_ISO_SETTING = 0x0002;
		public const int TAG_NIKON_TYPE2_COLOR_MODE = 0x0003;
		public const int TAG_NIKON_TYPE2_QUALITY = 0x0004;
		public const int TAG_NIKON_TYPE2_WHITE_BALANCE = 0x0005;
		public const int TAG_NIKON_TYPE2_IMAGE_SHARPENING = 0x0006;
		public const int TAG_NIKON_TYPE2_FOCUS_MODE = 0x0007;
		public const int TAG_NIKON_TYPE2_FLASH_SETTING = 0x0008;
		public const int TAG_NIKON_TYPE2_UNKNOWN_2 = 0x000A;
		public const int TAG_NIKON_TYPE2_ISO_SELECTION = 0x000F;
		public const int TAG_NIKON_TYPE2_IMAGE_ADJUSTMENT = 0x0080;
		public const int TAG_NIKON_TYPE2_ADAPTER = 0x0082;
		public const int TAG_NIKON_TYPE2_MANUAL_FOCUS_DISTANCE = 0x0085;
		public const int TAG_NIKON_TYPE2_DIGITAL_ZOOM = 0x0086;
		public const int TAG_NIKON_TYPE2_AF_FOCUS_POSITION = 0x0088;
		public const int TAG_NIKON_TYPE2_DATA_DUMP = 0x0010;

		protected static readonly IDictionary tagNameMap = NikonType2MakernoteDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();
			resu.Add(TAG_NIKON_TYPE2_ADAPTER, BUNDLE["TAG_NIKON_TYPE2_ADAPTER"]);
			resu.Add(TAG_NIKON_TYPE2_AF_FOCUS_POSITION, BUNDLE["TAG_NIKON_TYPE2_AF_FOCUS_POSITION"]);
			resu.Add(TAG_NIKON_TYPE2_COLOR_MODE, BUNDLE["TAG_NIKON_TYPE2_COLOR_MODE"]);
			resu.Add(TAG_NIKON_TYPE2_DATA_DUMP, BUNDLE["TAG_NIKON_TYPE2_DATA_DUMP"]);
			resu.Add(TAG_NIKON_TYPE2_DIGITAL_ZOOM, BUNDLE["TAG_NIKON_TYPE2_DIGITAL_ZOOM"]);
			resu.Add(TAG_NIKON_TYPE2_FLASH_SETTING, BUNDLE["TAG_NIKON_TYPE2_FLASH_SETTING"]);
			resu.Add(TAG_NIKON_TYPE2_FOCUS_MODE, BUNDLE["TAG_NIKON_TYPE2_FOCUS_MODE"]);
			resu.Add(TAG_NIKON_TYPE2_IMAGE_ADJUSTMENT, BUNDLE["TAG_NIKON_TYPE2_IMAGE_ADJUSTMENT"]);
			resu.Add(TAG_NIKON_TYPE2_IMAGE_SHARPENING, BUNDLE["TAG_NIKON_TYPE2_IMAGE_SHARPENING"]);
			resu.Add(TAG_NIKON_TYPE2_ISO_SELECTION, BUNDLE["TAG_NIKON_TYPE2_ISO_SELECTION"]);
			resu.Add(TAG_NIKON_TYPE2_ISO_SETTING, BUNDLE["TAG_NIKON_TYPE2_ISO_SETTING"]);
			resu.Add(TAG_NIKON_TYPE2_MANUAL_FOCUS_DISTANCE, BUNDLE["TAG_NIKON_TYPE2_MANUAL_FOCUS_DISTANCE"]);
			resu.Add(TAG_NIKON_TYPE2_QUALITY, BUNDLE["TAG_NIKON_TYPE2_QUALITY"]);
			resu.Add(TAG_NIKON_TYPE2_UNKNOWN_1, BUNDLE["TAG_NIKON_TYPE2_UNKNOWN_1"]);
			resu.Add(TAG_NIKON_TYPE2_UNKNOWN_2, BUNDLE["TAG_NIKON_TYPE2_UNKNOWN_2"]);
			resu.Add(TAG_NIKON_TYPE2_WHITE_BALANCE, BUNDLE["TAG_NIKON_TYPE2_WHITE_BALANCE"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public NikonType2MakernoteDirectory() : base()
		{
			this.SetDescriptor(new NikonType2MakernoteDescriptor(this));
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
