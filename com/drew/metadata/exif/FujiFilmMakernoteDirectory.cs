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
	/// <summary>
	/// The Fuji Film Makernote Directory
	/// </summary>
	public class FujiFilmMakernoteDirectory : Directory 
	{
		public const int TAG_FUJIFILM_MAKERNOTE_VERSION = 0x0000;
		public const int TAG_FUJIFILM_QUALITY = 0x1000;
		public const int TAG_FUJIFILM_SHARPNESS = 0x1001;
		public const int TAG_FUJIFILM_WHITE_BALANCE = 0x1002;
		public const int TAG_FUJIFILM_COLOR = 0x1003;
		public const int TAG_FUJIFILM_TONE = 0x1004;
		public const int TAG_FUJIFILM_FLASH_MODE = 0x1010;
		public const int TAG_FUJIFILM_FLASH_STRENGTH = 0x1011;
		public const int TAG_FUJIFILM_MACRO = 0x1020;
		public const int TAG_FUJIFILM_FOCUS_MODE = 0x1021;
		public const int TAG_FUJIFILM_SLOW_SYNCHRO = 0x1030;
		public const int TAG_FUJIFILM_PICTURE_MODE = 0x1031;
		public const int TAG_FUJIFILM_UNKNOWN_1 = 0x1032;
		public const int TAG_FUJIFILM_CONTINUOUS_TAKING_OR_AUTO_BRACKETTING =	0x1100;
		public const int TAG_FUJIFILM_UNKNOWN_2 = 0x1200;
		public const int TAG_FUJIFILM_BLUR_WARNING = 0x1300;
		public const int TAG_FUJIFILM_FOCUS_WARNING = 0x1301;
		public const int TAG_FUJIFILM_AE_WARNING = 0x1302;

		protected static readonly ResourceBundle BUNDLE = new ResourceBundle("FujiFilmMarkernote");
		protected static readonly IDictionary tagNameMap = FujiFilmMakernoteDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();
			resu.Add(TAG_FUJIFILM_AE_WARNING, BUNDLE["TAG_FUJIFILM_AE_WARNING"]);
			resu.Add(TAG_FUJIFILM_BLUR_WARNING, BUNDLE["TAG_FUJIFILM_BLUR_WARNING"]);
			resu.Add(TAG_FUJIFILM_COLOR, BUNDLE["TAG_FUJIFILM_COLOR"]);
			resu.Add(TAG_FUJIFILM_CONTINUOUS_TAKING_OR_AUTO_BRACKETTING, BUNDLE["TAG_FUJIFILM_CONTINUOUS_TAKING_OR_AUTO_BRACKETTING"]);
			resu.Add(TAG_FUJIFILM_FLASH_MODE, BUNDLE["TAG_FUJIFILM_FLASH_MODE"]);
			resu.Add(TAG_FUJIFILM_FLASH_STRENGTH, BUNDLE["TAG_FUJIFILM_FLASH_STRENGTH"]);
			resu.Add(TAG_FUJIFILM_FOCUS_MODE, BUNDLE["TAG_FUJIFILM_FOCUS_MODE"]);
			resu.Add(TAG_FUJIFILM_FOCUS_WARNING, BUNDLE["TAG_FUJIFILM_FOCUS_WARNING"]);
			resu.Add(TAG_FUJIFILM_MACRO, BUNDLE["TAG_FUJIFILM_MACRO"]);
			resu.Add(TAG_FUJIFILM_MAKERNOTE_VERSION, BUNDLE["TAG_FUJIFILM_MAKERNOTE_VERSION"]);
			resu.Add(TAG_FUJIFILM_PICTURE_MODE, BUNDLE["TAG_FUJIFILM_PICTURE_MODE"]);
			resu.Add(TAG_FUJIFILM_QUALITY, BUNDLE["TAG_FUJIFILM_QUALITY"]);
			resu.Add(TAG_FUJIFILM_SHARPNESS, BUNDLE["TAG_FUJIFILM_SHARPNESS"]);
			resu.Add(TAG_FUJIFILM_SLOW_SYNCHRO, BUNDLE["TAG_FUJIFILM_SLOW_SYNCHRO"]);
			resu.Add(TAG_FUJIFILM_TONE, BUNDLE["TAG_FUJIFILM_TONE"]);
			resu.Add(TAG_FUJIFILM_UNKNOWN_1, BUNDLE["TAG_FUJIFILM_UNKNOWN_1"]);
			resu.Add(TAG_FUJIFILM_UNKNOWN_2, BUNDLE["TAG_FUJIFILM_UNKNOWN_2"]);
			resu.Add(TAG_FUJIFILM_WHITE_BALANCE, BUNDLE["TAG_FUJIFILM_WHITE_BALANCE"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public FujiFilmMakernoteDirectory() : base()
		{
			this.SetDescriptor(new FujifilmMakernoteDescriptor(this));
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