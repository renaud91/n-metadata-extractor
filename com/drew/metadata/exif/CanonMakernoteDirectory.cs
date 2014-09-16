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
	/// This class represents CANON marker note.
	/// </summary>
	public class CanonMakernoteDirectory : Directory 
	{
		// CANON cameras have some funny bespoke fields that need further processing...
		public const int TAG_CANON_CAMERA_STATE_1 = 0x0001;
		public const int TAG_CANON_CAMERA_STATE_2 = 0x0004;

		public const int TAG_CANON_IMAGE_TYPE = 0x0006;
		public const int TAG_CANON_FIRMWARE_VERSION = 0x0007;
		public const int TAG_CANON_IMAGE_NUMBER = 0x0008;
		public const int TAG_CANON_OWNER_NAME = 0x0009;
		public const int TAG_CANON_SERIAL_NUMBER = 0x000C;
		public const int TAG_CANON_UNKNOWN_1 = 0x000D;
		public const int TAG_CANON_CUSTOM_FUNCTIONS = 0x000F;

		// These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
		public const int TAG_CANON_STATE1_MACRO_MODE = 0xC101;
		public const int TAG_CANON_STATE1_SELF_TIMER_DELAY = 0xC102;
		public const int TAG_CANON_STATE1_UNKNOWN_1 = 0xC103;
		public const int TAG_CANON_STATE1_FLASH_MODE = 0xC104;
		public const int TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE = 0xC105;
		public const int TAG_CANON_STATE1_UNKNOWN_2 = 0xC106;
		public const int TAG_CANON_STATE1_FOCUS_MODE_1 = 0xC107;
		public const int TAG_CANON_STATE1_UNKNOWN_3 = 0xC108;
		public const int TAG_CANON_STATE1_UNKNOWN_4 = 0xC109;
		public const int TAG_CANON_STATE1_IMAGE_SIZE = 0xC10A;
		public const int TAG_CANON_STATE1_EASY_SHOOTING_MODE = 0xC10B;
		public const int TAG_CANON_STATE1_UNKNOWN_5 = 0xC10C;
		public const int TAG_CANON_STATE1_CONTRAST = 0xC10D;
		public const int TAG_CANON_STATE1_SATURATION = 0xC10E;
		public const int TAG_CANON_STATE1_SHARPNESS = 0xC10F;
		public const int TAG_CANON_STATE1_ISO = 0xC110;
		public const int TAG_CANON_STATE1_METERING_MODE = 0xC111;
		public const int TAG_CANON_STATE1_UNKNOWN_6 = 0xC112;
		public const int TAG_CANON_STATE1_AF_POINT_SELECTED = 0xC113;
		public const int TAG_CANON_STATE1_EXPOSURE_MODE = 0xC114;
		public const int TAG_CANON_STATE1_UNKNOWN_7 = 0xC115;
		public const int TAG_CANON_STATE1_UNKNOWN_8 = 0xC116;
		public const int TAG_CANON_STATE1_LONG_FOCAL_LENGTH = 0xC117;
		public const int TAG_CANON_STATE1_SHORT_FOCAL_LENGTH = 0xC118;
		public const int TAG_CANON_STATE1_FOCAL_UNITS_PER_MM = 0xC119;
		public const int TAG_CANON_STATE1_UNKNOWN_9 = 0xC11A;
		public const int TAG_CANON_STATE1_UNKNOWN_10 = 0xC11B;
		public const int TAG_CANON_STATE1_UNKNOWN_11 = 0xC11C;
		public const int TAG_CANON_STATE1_FLASH_DETAILS = 0xC11D;
		public const int TAG_CANON_STATE1_UNKNOWN_12 = 0xC11E;
		public const int TAG_CANON_STATE1_UNKNOWN_13 = 0xC11F;
		public const int TAG_CANON_STATE1_FOCUS_MODE_2 = 0xC120;

		public const int TAG_CANON_STATE2_WHITE_BALANCE = 0xC207;
		public const int TAG_CANON_STATE2_SEQUENCE_NUMBER = 0xC209;
		public const int TAG_CANON_STATE2_AF_POINT_USED = 0xC20E;
		public const int TAG_CANON_STATE2_FLASH_BIAS = 0xC20F;
		public const int TAG_CANON_STATE2_SUBJECT_DISTANCE = 0xC213;

		protected static readonly ResourceBundle BUNDLE = new ResourceBundle("CanonMarkernote");

		// 9  A  B  C  D  E  F  10 11 12 13
		// 9  10 11 12 13 14 15 16 17 18 19
		protected static readonly IDictionary tagNameMap = CanonMakernoteDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();

			resu.Add(TAG_CANON_FIRMWARE_VERSION, BUNDLE["TAG_CANON_FIRMWARE_VERSION"]);
			resu.Add(TAG_CANON_IMAGE_NUMBER, BUNDLE["TAG_CANON_IMAGE_NUMBER"]);
			resu.Add(TAG_CANON_IMAGE_TYPE, BUNDLE["TAG_CANON_IMAGE_TYPE"]);
			resu.Add(TAG_CANON_OWNER_NAME, BUNDLE["TAG_CANON_OWNER_NAME"]);
			resu.Add(TAG_CANON_UNKNOWN_1, BUNDLE["TAG_CANON_UNKNOWN_1"]);
			resu.Add(TAG_CANON_CUSTOM_FUNCTIONS, BUNDLE["TAG_CANON_CUSTOM_FUNCTIONS"]);
			resu.Add(TAG_CANON_SERIAL_NUMBER, BUNDLE["TAG_CANON_SERIAL_NUMBER"]);
			resu.Add(TAG_CANON_STATE1_AF_POINT_SELECTED, BUNDLE["TAG_CANON_STATE1_AF_POINT_SELECTED"]);
			resu.Add(TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE, BUNDLE["TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE"]);
			resu.Add(TAG_CANON_STATE1_CONTRAST, BUNDLE["TAG_CANON_STATE1_CONTRAST"]);
			resu.Add(TAG_CANON_STATE1_EASY_SHOOTING_MODE, BUNDLE["TAG_CANON_STATE1_EASY_SHOOTING_MODE"]);
			resu.Add(TAG_CANON_STATE1_EXPOSURE_MODE, BUNDLE["TAG_CANON_STATE1_EXPOSURE_MODE"]);
			resu.Add(TAG_CANON_STATE1_FLASH_DETAILS, BUNDLE["TAG_CANON_STATE1_FLASH_DETAILS"]);
			resu.Add(TAG_CANON_STATE1_FLASH_MODE, BUNDLE["TAG_CANON_STATE1_FLASH_MODE"]);
			resu.Add(TAG_CANON_STATE1_FOCAL_UNITS_PER_MM, BUNDLE["TAG_CANON_STATE1_FOCAL_UNITS_PER_MM"]);
			resu.Add(TAG_CANON_STATE1_FOCUS_MODE_1,	BUNDLE["TAG_CANON_STATE1_FOCUS_MODE_1"]);
			resu.Add(TAG_CANON_STATE1_FOCUS_MODE_2,	BUNDLE["TAG_CANON_STATE1_FOCUS_MODE_2"]);
			resu.Add(TAG_CANON_STATE1_IMAGE_SIZE, BUNDLE["TAG_CANON_STATE1_IMAGE_SIZE"]);
			resu.Add(TAG_CANON_STATE1_ISO, BUNDLE["TAG_CANON_STATE1_ISO"]);
			resu.Add(TAG_CANON_STATE1_LONG_FOCAL_LENGTH, BUNDLE["TAG_CANON_STATE1_LONG_FOCAL_LENGTH"]);
			resu.Add(TAG_CANON_STATE1_MACRO_MODE, BUNDLE["TAG_CANON_STATE1_MACRO_MODE"]);
			resu.Add(TAG_CANON_STATE1_METERING_MODE, BUNDLE["TAG_CANON_STATE1_METERING_MODE"]);
			resu.Add(TAG_CANON_STATE1_SATURATION, BUNDLE["TAG_CANON_STATE1_SATURATION"]);
			resu.Add(TAG_CANON_STATE1_SELF_TIMER_DELAY,	BUNDLE["TAG_CANON_STATE1_SELF_TIMER_DELAY"]);
			resu.Add(TAG_CANON_STATE1_SHARPNESS, BUNDLE["TAG_CANON_STATE1_SHARPNESS"]);
			resu.Add(TAG_CANON_STATE1_SHORT_FOCAL_LENGTH, BUNDLE["TAG_CANON_STATE1_SHORT_FOCAL_LENGTH"]);
			
			resu.Add(TAG_CANON_STATE1_UNKNOWN_1, BUNDLE["TAG_CANON_STATE1_UNKNOWN_1"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_2, BUNDLE["TAG_CANON_STATE1_UNKNOWN_2"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_3, BUNDLE["TAG_CANON_STATE1_UNKNOWN_3"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_4, BUNDLE["TAG_CANON_STATE1_UNKNOWN_4"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_5, BUNDLE["TAG_CANON_STATE1_UNKNOWN_5"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_6, BUNDLE["TAG_CANON_STATE1_UNKNOWN_6"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_7, BUNDLE["TAG_CANON_STATE1_UNKNOWN_7"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_8, BUNDLE["TAG_CANON_STATE1_UNKNOWN_8"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_9, BUNDLE["TAG_CANON_STATE1_UNKNOWN_9"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_10, BUNDLE["TAG_CANON_STATE1_UNKNOWN_10"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_11, BUNDLE["TAG_CANON_STATE1_UNKNOWN_11"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_12, BUNDLE["TAG_CANON_STATE1_UNKNOWN_12"]);
			resu.Add(TAG_CANON_STATE1_UNKNOWN_13, BUNDLE["TAG_CANON_STATE1_UNKNOWN_13"]);
			
			resu.Add(TAG_CANON_STATE2_WHITE_BALANCE, BUNDLE["TAG_CANON_STATE2_WHITE_BALANCE"]);
			resu.Add(TAG_CANON_STATE2_SEQUENCE_NUMBER, BUNDLE["TAG_CANON_STATE2_SEQUENCE_NUMBER"]);
			resu.Add(TAG_CANON_STATE2_AF_POINT_USED, BUNDLE["TAG_CANON_STATE2_AF_POINT_USED"]);
			resu.Add(TAG_CANON_STATE2_FLASH_BIAS, BUNDLE["TAG_CANON_STATE2_FLASH_BIAS"]);
			resu.Add(TAG_CANON_STATE2_SUBJECT_DISTANCE,	BUNDLE["TAG_CANON_STATE2_SUBJECT_DISTANCE"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public CanonMakernoteDirectory() : base()
		{
			this.SetDescriptor(new CanonMakernoteDescriptor(this));
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

		/// <summary>
		/// We need special handling for selected tags.
		/// </summary>
		/// <param name="tagType">the tag type</param>
		/// <param name="ints">what to set</param>
		public override void SetIntArray(int tagType, int[] ints) 
		{
			if (tagType == TAG_CANON_CAMERA_STATE_1) 
			{
				// this single tag has multiple values within
				int subTagTypeBase = 0xC100;
				// we intentionally skip the first array member
				for (int i = 1; i < ints.Length; i++) 
				{
					SetObject(subTagTypeBase + i, ints[i]);
				}
			} 
			else if (tagType == TAG_CANON_CAMERA_STATE_2) 
			{
				// this single tag has multiple values within
				int subTagTypeBase = 0xC200;
				// we intentionally skip the first array member
				for (int i = 1; i < ints.Length; i++) 
				{
					SetObject(subTagTypeBase + i, ints[i]);
				}
			} 
			else 
			{
				// no special handling...
				base.SetIntArray(tagType, ints);
			}
		}
	}
}