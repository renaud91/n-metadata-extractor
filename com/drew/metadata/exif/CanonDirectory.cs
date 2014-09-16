using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using com.drew.metadata;
using com.utils;

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
namespace com.drew.metadata.exif
{
	/// <summary>
	/// This class represents CANON marker note.
	/// </summary>
    public class CanonDirectory : AbstractDirectory
    {
        // CANON cameras have some funny bespoke fields that need further processing...
        public const int TAG_CANON_CAMERA_STATE_1 = 0x0001;
        public const int TAG_CANON_CAMERA_STATE_2 = 0x0004;

        public const int TAG_CANON_IMAGE_TYPE = 0x0006;
        public const int TAG_CANON_FIRMWARE_VERSION = 0x0007;
        public const int TAG_CANON_IMAGE_NUMBER = 0x0008;
        public const int TAG_CANON_OWNER_NAME = 0x0009;
        /// <summary>
        ///  To display serial number as on camera use: printf( "%04X%05d", highbyte, lowbyte )
        ///  TODO handle this in CanonMakernoteDescriptor
        /// </summary>
        public const int TAG_CANON_SERIAL_NUMBER = 0x000C;
        public const int TAG_CANON_UNKNOWN_1 = 0x000D;
        public const int TAG_CANON_CUSTOM_FUNCTIONS = 0x000F;

        // These 'sub'-tag values have been created for consistency -- they don't exist within the exif segment
        /// <summary>
        ///  1 = Macro
        ///  2 = Normal
        /// </summary>
        public const int TAG_CANON_STATE1_MACRO_MODE = 0xC101;
        public const int TAG_CANON_STATE1_SELF_TIMER_DELAY = 0xC102;
        /// <summary>
        ///  2 = Normal
        ///  3 = Fine
        ///  5 = Superfine
        /// </summary>
        public const int TAG_CANON_STATE1_QUALITY = 0xC103;
        /// <summary>
        ///  0 = Flash Not Fired
        ///  1 = Auto
        ///  2 = On
        ///  3 = Red Eye Reduction
        ///  4 = Slow Synchro
        ///  5 = Auto + Red Eye Reduction
        ///  6 = On + Red Eye Reduction
        ///  16 = External Flash
        /// </summary>
        public const int TAG_CANON_STATE1_FLASH_MODE = 0xC104;
        /// <summary>
        ///  0 = Single Frame or Timer Mode
        ///  1 = Continuous
        /// </summary>
        public const int TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE = 0xC105;
        public const int TAG_CANON_STATE1_UNKNOWN_2 = 0xC106;
        /// <summary>
        ///  0 = One-Shot
        ///  1 = AI Servo
        ///  2 = AI Focus
        ///  3 = Manual Focus
        ///  4 = Single
        ///  5 = Continuous
        ///  6 = Manual Focus
        /// </summary>
        public const int TAG_CANON_STATE1_FOCUS_MODE_1 = 0xC107;
        public const int TAG_CANON_STATE1_UNKNOWN_3 = 0xC108;
        public const int TAG_CANON_STATE1_UNKNOWN_4 = 0xC109;
        /// <summary>
        ///  0 = Large
        ///  1 = Medium
        ///  2 = Small
        /// </summary>
        public const int TAG_CANON_STATE1_IMAGE_SIZE = 0xC10A;
        /// <summary>
        ///  0 = Full Auto
        ///  1 = Manual
        ///  2 = Landscape
        ///  3 = Fast Shutter
        ///  4 = Slow Shutter
        ///  5 = Night
        ///  6 = Black & White
        ///  7 = Sepia
        ///  8 = Portrait
        ///  9 = Sports
        ///  10 = Macro / Close-Up
        ///  11 = Pan Focus
        /// </summary>
        public const int TAG_CANON_STATE1_EASY_SHOOTING_MODE = 0xC10B;
        /// <summary>
        ///  0 = No Digital Zoom
        ///  1 = 2x
        ///  2 = 4x
        /// </summary>
        public const int TAG_CANON_STATE1_DIGITAL_ZOOM = 0xC10C;
        /// <summary>
        ///  0 = Normal
        ///  1 = High
        ///  65535 = Low
        /// </summary>
        public const int TAG_CANON_STATE1_CONTRAST = 0xC10D;
        /// <summary>
        ///  0 = Normal
        ///  1 = High
        ///  65535 = Low
        /// </summary>
        public const int TAG_CANON_STATE1_SATURATION = 0xC10E;
        /// <summary>
        ///  0 = Normal
        ///  1 = High
        ///  65535 = Low
        /// </summary>
        public const int TAG_CANON_STATE1_SHARPNESS = 0xC10F;
        /// <summary>
        ///  0 = Check ISOSpeedRatings EXIF tag for ISO Speed
        ///  15 = Auto ISO
        ///  16 = ISO 50
        ///  17 = ISO 100
        ///  18 = ISO 200
        ///  19 = ISO 400
        /// </summary>
        public const int TAG_CANON_STATE1_ISO = 0xC110;
        /// <summary>
        ///  3 = Evaluative
        ///  4 = Partial
        ///  5 = Center Weighted
        /// </summary>
        public const int TAG_CANON_STATE1_METERING_MODE = 0xC111;
        /// <summary>
        ///  0 = Manual
        ///  1 = Auto
        ///  3 = Close-up (Macro)
        ///  8 = Locked (Pan Mode)
        /// </summary>
        public const int TAG_CANON_STATE1_FOCUS_TYPE = 0xC112;
        /// <summary>
        ///  12288 = None (Manual Focus)
        ///  12289 = Auto Selected
        ///  12290 = Right
        ///  12291 = Center
        ///  12292 = Left
        /// </summary>
        public const int TAG_CANON_STATE1_AF_POINT_SELECTED = 0xC113;
        /// <summary>
        ///  0 = Easy Shooting (See Easy Shooting Mode)
        ///  1 = Program
        ///  2 = Tv-Priority
        ///  3 = Av-Priority
        ///  4 = Manual
        ///  5 = A-DEP
        /// </summary>
        public const int TAG_CANON_STATE1_EXPOSURE_MODE = 0xC114;
        public const int TAG_CANON_STATE1_UNKNOWN_7 = 0xC115;
        public const int TAG_CANON_STATE1_UNKNOWN_8 = 0xC116;
        public const int TAG_CANON_STATE1_LONG_FOCAL_LENGTH = 0xC117;
        public const int TAG_CANON_STATE1_SHORT_FOCAL_LENGTH = 0xC118;
        public const int TAG_CANON_STATE1_FOCAL_UNITS_PER_MM = 0xC119;
        public const int TAG_CANON_STATE1_UNKNOWN_9 = 0xC11A;
        public const int TAG_CANON_STATE1_UNKNOWN_10 = 0xC11B;
        /// <summary>
        ///  0 = Flash Did Not Fire
        ///  1 = Flash Fired
        /// </summary>
        public const int TAG_CANON_STATE1_FLASH_ACTIVITY = 0xC11C;
        public const int TAG_CANON_STATE1_FLASH_DETAILS = 0xC11D;
        public const int TAG_CANON_STATE1_UNKNOWN_12 = 0xC11E;
        public const int TAG_CANON_STATE1_UNKNOWN_13 = 0xC11F;
        /// <summary>
        ///  0 = Focus Mode: Single
        ///  1 = Focus Mode: Continuous
        /// </summary>
        public const int TAG_CANON_STATE1_FOCUS_MODE_2 = 0xC120;

        /// <summary>
        ///  0 = Auto
        ///  1 = Sunny
        ///  2 = Cloudy
        ///  3 = Tungsten
        ///  4 = Fluorescent
        ///  5 = Flash
        ///  6 = Custom
        /// </summary>
        public const int TAG_CANON_STATE2_WHITE_BALANCE = 0xC207;
        public const int TAG_CANON_STATE2_SEQUENCE_NUMBER = 0xC209;
        public const int TAG_CANON_STATE2_AF_POINT_USED = 0xC20E;
        /// <summary>
        ///  The value of this tag may be translated into a flash bias value, in EV.
        /// 
        ///  0xffc0 = -2 EV
        ///  0xffcc = -1.67 EV
        ///  0xffd0 = -1.5 EV
        ///  0xffd4 = -1.33 EV
        ///  0xffe0 = -1 EV
        ///  0xffec = -0.67 EV
        ///  0xfff0 = -0.5 EV
        ///  0xfff4 = -0.33 EV
        ///  0x0000 = 0 EV
        ///  0x000c = 0.33 EV
        ///  0x0010 = 0.5 EV
        ///  0x0014 = 0.67 EV
        ///  0x0020 = 1 EV
        ///  0x002c = 1.33 EV
        ///  0x0030 = 1.5 EV
        ///  0x0034 = 1.67 EV
        ///  0x0040 = 2 EV 
        /// </summary>
        public const int TAG_CANON_STATE2_FLASH_BIAS = 0xC20F;
        public const int TAG_CANON_STATE2_AUTO_EXPOSURE_BRACKETING = 0xC210;
        public const int TAG_CANON_STATE2_AEB_BRACKET_VALUE = 0xC211;
        public const int TAG_CANON_STATE2_SUBJECT_DISTANCE = 0xC213;

        /// <summary>
        ///  Long Exposure Noise Reduction
        ///  0 = Off
        ///  1 = On
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_LONG_EXPOSURE_NOISE_REDUCTION = 0xC301;

        /// <summary>
        ///  Shutter/Auto Exposure-lock buttons
        ///  0 = AF/AE lock
        ///  1 = AE lock/AF
        ///  2 = AF/AF lock
        ///  3 = AE+release/AE+AF
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_AUTO_EXPOSURE_LOCK_BUTTONS = 0xC302;

        /// <summary>
        ///  Mirror lockup
        ///  0 = Disable
        ///  1 = Enable
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_MIRROR_LOCKUP = 0xC303;

        /// <summary>
        ///  Tv/Av and exposure level
        ///  0 = 1/2 stop
        ///  1 = 1/3 stop
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_TV_AV_AND_EXPOSURE_LEVEL = 0xC304;

        /// <summary>
        ///  AF-assist light
        ///  0 = On (Auto)
        ///  1 = Off
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_AF_ASSIST_LIGHT = 0xC305;

        /// <summary>
        ///  Shutter speed in Av mode
        ///  0 = Automatic
        ///  1 = 1/200 (fixed)
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_SPEED_IN_AV_MODE = 0xC306;

        /// <summary>
        ///  Auto-Exposure Bracketting sequence/auto cancellation
        ///  0 = 0,-,+ / Enabled
        ///  1 = 0,-,+ / Disabled
        ///  2 = -,0,+ / Enabled
        ///  3 = -,0,+ / Disabled
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_BRACKETTING = 0xC307;

        /// <summary>
        ///  Shutter Curtain Sync
        ///  0 = 1st Curtain Sync
        ///  1 = 2nd Curtain Sync
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_SHUTTER_CURTAIN_SYNC = 0xC308;

        /// <summary>
        ///  Lens Auto-Focus stop button Function Switch
        ///  0 = AF stop
        ///  1 = Operate AF
        ///  2 = Lock AE and start timer
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_AF_STOP = 0xC309;

        /// <summary>
        ///  Auto reduction of fill flash
        ///  0 = Enable
        ///  1 = Disable
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_FILL_FLASH_REDUCTION = 0xC30A;

        /// <summary>
        ///  Menu button return position
        ///  0 = Top
        ///  1 = Previous (volatile)
        ///  2 = Previous
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_MENU_BUTTON_RETURN = 0xC30B;

        /// <summary>
        ///  SET button function when shooting
        ///  0 = Not Assigned
        ///  1 = Change Quality
        ///  2 = Change ISO Speed
        ///  3 = Select Parameters
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_SET_BUTTON_FUNCTION = 0xC30C;

        /// <summary>
        ///  Sensor cleaning
        ///  0 = Disable
        ///  1 = Enable
        /// </summary>
        public const int TAG_CANON_CUSTOM_FUNCTION_SENSOR_CLEANING = 0xC30D;

        protected static readonly ResourceBundle BUNDLE = new ResourceBundle("CanonMarkernote");

        // 9  A  B  C  D  E  F  10 11 12 13
        // 9  10 11 12 13 14 15 16 17 18 19
        protected static readonly Dictionary<int, string> tagNameMap = FillTagMap(Type.GetType("com.drew.metadata.exif.CanonDirectory"), BUNDLE);

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        public CanonDirectory()
            : base()
        {
            base.SetDescriptor(new CanonDescriptor(this));
        }

        /// <summary>
        /// Provides the name of the directory, for display purposes.  E.g. Exif 
        /// </summary>
        /// <returns>the name of the directory</returns>
        public override string GetName()
        {
            return CanonDirectory.BUNDLE["MARKER_NOTE_NAME"];
        }

        /// <summary>
        /// Provides the map of tag names, hashed by tag type identifier. 
        /// </summary>
        /// <returns>the map of tag names</returns>
        protected override Dictionary<int, string> GetTagNameMap()
        {
            return CanonDirectory.tagNameMap;
        }

        /// <summary>
        /// We need special handling for selected tags.
        /// </summary>
        /// <param name="aTagType">the tag type</param>
        /// <param name="someInts">what to set</param>
        public override void SetIntArray(int tagType, int[] ints)
        {
            if (tagType == TAG_CANON_CAMERA_STATE_1)
            {
                // this single tag has multiple values within
                int subTagTypeBase = 0xC100;
                // we intentionally skip the first array member
                for (int i = 1; i < ints.Length; i++)
                {
                    base.SetObject(subTagTypeBase + i, ints[i]);
                }
            }
            else if (tagType == TAG_CANON_CAMERA_STATE_2)
            {
                // this single tag has multiple values within
                int subTagTypeBase = 0xC200;
                // we intentionally skip the first array member
                for (int i = 1; i < ints.Length; i++)
                {
                    base.SetObject(subTagTypeBase + i, ints[i]);
                }
            }
            if (tagType == TAG_CANON_CUSTOM_FUNCTIONS)
            {
                // this single tag has multiple values within
                int subTagTypeBase = 0xC300;
                // we intentionally skip the first array member
                for (int i = 1; i < ints.Length; i++)
                {
                    base.SetObject(subTagTypeBase + i + 1, ints[i] & 0x0F);
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