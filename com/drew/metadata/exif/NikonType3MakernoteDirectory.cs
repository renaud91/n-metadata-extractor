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
	/// The type-3 directory is for D-Series cameras such as the D1 and D100.
	/// Thanks to Fabrizio Giudici for publishing his reverse-engineering of the D1 makernote data.
	/// http://www.timelesswanderings.net/equipment/D100/NEF.html
	/// 
	/// Additional sample images have been observed, and their tag values recorded in doc 
	/// comments for each tag's field. New tags have subsequently been added since Fabrizio's observations.
	/// </summary>
	public class NikonType3MakernoteDirectory : NikonTypeMakernoteDirectory 
	{
		/// <summary>
		/// Values observed
		/// - 0200
		/// </summary>
		public const int TAG_NIKON_TYPE3_FIRMWARE_VERSION = 1;

		/// <summary>
		/// Values observed
		/// - 0 250
		/// - 0 400
		/// </summary>
		public const int TAG_NIKON_TYPE3_ISO_1 = 2;

		/// <summary>
		/// Values observed
		/// - FILE
		/// - RAW
		/// </summary>
		public const int TAG_NIKON_TYPE3_FILE_FORMAT = 4;

		/// <summary>
		/// Values observed
		/// - AUTO
		/// - SUNNY
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE = 5;

		/// <summary>
		/// Values observed
		/// - AUTO
		/// - NORMAL
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_SHARPENING = 6;

		/// <summary>
		/// Values observed
		/// - AF-S
		/// </summary>
		public const int TAG_NIKON_TYPE3_AF_TYPE = 7;

		/// <summary>
		/// Values observed
		/// - NORMAL
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_17 = 8;

		/// <summary>
		/// Values observed
		/// -
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_18 = 9;

		/// <summary>
		/// Values observed
		/// - 0
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_FINE = 11;

		/// <summary>
		/// Values observed
		/// - 2.25882352 1.76078431 0.0 0.0
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_RB_COEFF = 12;

		/// <summary>
		/// Values observed
		/// -
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_1 = 13;

		/// <summary>
		/// Values observed
		/// -
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_2 = 14;

		/// <summary>
		/// Values observed
		/// - 914
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_3 = 17;

		/// <summary>
		/// Values observed
		/// -
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_19 = 18;

		/// <summary>
		/// Values observed
		/// - 0 250
		/// </summary>
		public const int TAG_NIKON_TYPE3_ISO_2 = 19;

		/// <summary>
		/// Values observed
		/// - AUTO
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_TONE_COMPENSATION = 129;

		/// <summary>
		/// Values observed
		/// - 6
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_4 = 131;

		/// <summary>
		/// Values observed
		/// - 240/10 850/10 35/10 45/10
		/// </summary>
		public const int TAG_NIKON_TYPE3_LENS = 132;

		/// <summary>
		/// Values observed
		/// - 0
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_5 = 135;

		/// <summary>
		/// Values observed
		/// -
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_6 = 136;

		/// <summary>
		/// Values observed
		/// - 0
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_7 = 137;

		/// <summary>
		/// Values observed
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_8 = 139;

		/// <summary>
		/// Values observed
		/// - 0
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_20 = 138;

		/// <summary>
		/// Values observed
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_9 = 140;

		/// <summary>
		/// Values observed
		/// - MODE1
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_COLOR_MODE = 141;

		/// <summary>
		/// Values observed
		/// - NATURAL
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_10 = 144;

		public const int TAG_NIKON_TYPE3_UNKNOWN_11 = 145;

		/// <summary>
		/// Values observed
		/// - 0
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT = 146;

		/// <summary>
		/// Values observed
		/// - OFF
		/// </summary>
		public const int TAG_NIKON_TYPE3_NOISE_REDUCTION = 149;

		public const int TAG_NIKON_TYPE3_UNKNOWN_12 = 151;

		/// <summary>
		/// Values observed
		/// - 0100fht@7b,4x,D"Y
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_13 = 152;

		/// <summary>
		/// Values observed
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_14 = 153;

		/// <summary>
		/// Values observed
		/// - 78/10 78/10
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_15 = 154;

		/// <summary>
		/// Values observed
		/// </summary>
		public const int TAG_NIKON_TYPE3_CAPTURE_EDITOR_DATA = 3585;

		/// <summary>
		/// Values observed
		/// </summary>
		public const int TAG_NIKON_TYPE3_UNKNOWN_16 = 3600;

		protected static readonly IDictionary tagNameMap = NikonType3MakernoteDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();
			resu.Add(TAG_NIKON_TYPE3_FIRMWARE_VERSION, BUNDLE["TAG_NIKON_TYPE3_FIRMWARE_VERSION"]);
			resu.Add(TAG_NIKON_TYPE3_ISO_1, BUNDLE["TAG_NIKON_TYPE3_ISO_1"]);
			resu.Add(TAG_NIKON_TYPE3_FILE_FORMAT, BUNDLE["TAG_NIKON_TYPE3_FILE_FORMAT"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE, BUNDLE["TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_SHARPENING, BUNDLE["TAG_NIKON_TYPE3_CAMERA_SHARPENING"]);
			resu.Add(TAG_NIKON_TYPE3_AF_TYPE, BUNDLE["TAG_NIKON_TYPE3_AF_TYPE"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_FINE, BUNDLE["TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_FINE"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_RB_COEFF, BUNDLE["TAG_NIKON_TYPE3_CAMERA_WHITE_BALANCE_RB_COEFF"]);
			resu.Add(TAG_NIKON_TYPE3_ISO_2, BUNDLE["TAG_NIKON_TYPE3_ISO_2"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_TONE_COMPENSATION, BUNDLE["TAG_NIKON_TYPE3_CAMERA_TONE_COMPENSATION"]);
			resu.Add(TAG_NIKON_TYPE3_LENS, BUNDLE["TAG_NIKON_TYPE3_LENS"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_COLOR_MODE,	BUNDLE["TAG_NIKON_TYPE3_CAMERA_COLOR_MODE"]);
			resu.Add(TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT,	BUNDLE["TAG_NIKON_TYPE3_CAMERA_HUE_ADJUSTMENT"]);
			resu.Add(TAG_NIKON_TYPE3_NOISE_REDUCTION, BUNDLE["TAG_NIKON_TYPE3_NOISE_REDUCTION"]);
			resu.Add(TAG_NIKON_TYPE3_CAPTURE_EDITOR_DATA, BUNDLE["TAG_NIKON_TYPE3_CAPTURE_EDITOR_DATA"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_1, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_1"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_2, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_2"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_3, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_3"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_4, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_4"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_5, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_5"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_6, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_6"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_7, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_7"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_8, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_8"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_9, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_9"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_10, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_10"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_11, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_11"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_12, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_12"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_13, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_13"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_14, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_14"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_15, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_15"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_16, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_16"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_17, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_17"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_18, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_18"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_19, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_19"]);
			resu.Add(TAG_NIKON_TYPE3_UNKNOWN_20, BUNDLE["TAG_NIKON_TYPE3_UNKNOWN_20"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public NikonType3MakernoteDirectory() : base()
		{
			this.SetDescriptor(new NikonType3MakernoteDescriptor(this));
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