using System;
using com.drew.metadata;

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
	/// Tag descriptor for a Canon camera
	/// </summary>
	public class CanonMakernoteDescriptor : TagDescriptor 
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="directory">a directory</param>
		public CanonMakernoteDescriptor(Directory directory) : base(directory)
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
				case CanonMakernoteDirectory.TAG_CANON_STATE1_MACRO_MODE :
					return GetMacroModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_SELF_TIMER_DELAY:
					return GetSelfTimerDelayDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_MODE :
					return GetFlashModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE:
					return GetContinuousDriveModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_1:
					return GetFocusMode1Description();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_IMAGE_SIZE:
					return GetImageSizeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_EASY_SHOOTING_MODE:
					return GetEasyShootingModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_CONTRAST:
					return GetContrastDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_SATURATION:
					return GetSaturationDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_SHARPNESS:
					return GetSharpnessDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_ISO:
					return GetIsoDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_METERING_MODE:
					return GetMeteringModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_AF_POINT_SELECTED:
					return GetAfPointSelectedDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_EXPOSURE_MODE:
					return GetExposureModeDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_LONG_FOCAL_LENGTH:
					return GetLongFocalLengthDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_SHORT_FOCAL_LENGTH:
					return GetShortFocalLengthDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_FOCAL_UNITS_PER_MM:
					return GetFocalUnitsPerMillimetreDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_DETAILS:
					return GetFlashDetailsDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_2:
					return GetFocusMode2Description();
				case CanonMakernoteDirectory.TAG_CANON_STATE2_WHITE_BALANCE:
					return GetWhiteBalanceDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE2_AF_POINT_USED:
					return GetAfPointUsedDescription();
				case CanonMakernoteDirectory.TAG_CANON_STATE2_FLASH_BIAS:
					return GetFlashBiasDescription();
				default : 
					return _directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Flash Bias  Description. 
		/// </summary>
		/// <returns>the Flash Bias  Description.</returns>
		private string GetFlashBiasDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE2_FLASH_BIAS))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE2_FLASH_BIAS);
			switch (aValue) 
			{
				case 0xffc0 :
					return BUNDLE["FLASH_BIAS_N_2"];
				case 0xffcc :
					return BUNDLE["FLASH_BIAS_N_167"];
				case 0xffd0 :
					return BUNDLE["FLASH_BIAS_N_150"];
				case 0xffd4 :
					return BUNDLE["FLASH_BIAS_N_133"];
				case 0xffe0 :
					return BUNDLE["FLASH_BIAS_N_1"];
				case 0xffec :
					return BUNDLE["FLASH_BIAS_N_067"];
				case 0xfff0 :
					return BUNDLE["FLASH_BIAS_N_050"];
				case 0xfff4 :
					return BUNDLE["FLASH_BIAS_N_033"];
				case 0x0000 :
					return BUNDLE["FLASH_BIAS_P_0"];
				case 0x000c :
					return BUNDLE["FLASH_BIAS_P_033"];
				case 0x0010 :
					return BUNDLE["FLASH_BIAS_P_050"];
				case 0x0014 :
					return BUNDLE["FLASH_BIAS_P_067"];
				case 0x0020 :
					return BUNDLE["FLASH_BIAS_P_1"];
				case 0x002c :
					return BUNDLE["FLASH_BIAS_P_133"];
				case 0x0030 :
					return BUNDLE["FLASH_BIAS_P_150"];
				case 0x0034 :
					return BUNDLE["FLASH_BIAS_P_167"];
				case 0x0040 :
					return BUNDLE["FLASH_BIAS_P_2"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Af Point Used Description. 
		/// </summary>
		/// <returns>the Af Point Used Description.</returns>
		private string GetAfPointUsedDescription()
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE2_AF_POINT_USED))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE2_AF_POINT_USED);
			if ((aValue & 0x7) == 0) 
			{
				return BUNDLE["RIGHT"];
			} 
			else if ((aValue & 0x7) == 1) 
			{
				return BUNDLE["CENTER"];;
			} 
			else if ((aValue & 0x7) == 2) 
			{
				return BUNDLE["LEFT"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns White Balance Description. 
		/// </summary>
		/// <returns>the White Balance Description.</returns>
		private string GetWhiteBalanceDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE2_WHITE_BALANCE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE2_WHITE_BALANCE);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["AUTO"];
				case 1 :
					return BUNDLE["SUNNY"];
				case 2 :
					return BUNDLE["CLOUDY"];
				case 3 :
					return BUNDLE["TUNGSTEN"];
				case 4 :
					return BUNDLE["FLOURESCENT"];
				case 5 :
					return BUNDLE["FLASH"];
				case 6 :
					return BUNDLE["CUSTOM"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Focus Mode 2 description. 
		/// </summary>
		/// <returns>the Focus Mode 2 description</returns>
		private string GetFocusMode2Description() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_2))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_2);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["SINGLE"];
				case 1 :
					return BUNDLE["CONTINUOUS"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Flash Details description. 
		/// </summary>
		/// <returns>the Flash Details description</returns>
		private string GetFlashDetailsDescription()  
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_DETAILS))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_DETAILS);
			if (((aValue << 14) & 1) > 0) 
			{
				return BUNDLE["EXTERNAL_E_TTL"];
			}
			if (((aValue << 13) & 1) > 0) 
			{
				return BUNDLE["INTERNAL_FLASH"];
			}
			if (((aValue << 11) & 1) > 0) 
			{
				return BUNDLE["FP_SYNC_USED"];
			}
			if (((aValue << 4) & 1) > 0) 
			{
				return BUNDLE["FP_SYNC_ENABLED"];
			}
			return BUNDLE["UNKNOWN", aValue.ToString()];
		}

		/// <summary>
		/// Returns Focal Units Per Millimetre description. 
		/// </summary>
		/// <returns>the Focal Units Per Millimetre description</returns>
		private string GetFocalUnitsPerMillimetreDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FOCAL_UNITS_PER_MM))
				return "";
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FOCAL_UNITS_PER_MM);
			if (aValue != 0) 
			{
				return aValue.ToString();
			} 
			else 
			{
				return "";
			}
		}

		/// <summary>
		/// Returns Short Focal Length description. 
		/// </summary>
		/// <returns>the Short Focal Length description</returns>
		private string GetShortFocalLengthDescription()  
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SHORT_FOCAL_LENGTH))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SHORT_FOCAL_LENGTH);
			string units = GetFocalUnitsPerMillimetreDescription();
			return BUNDLE["FOCAL_LENGTH", aValue.ToString(), units];
		}

		/// <summary>
		/// Returns Long Focal Length description. 
		/// </summary>
		/// <returns>the Long Focal Length description</returns>
		private string GetLongFocalLengthDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_LONG_FOCAL_LENGTH))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_LONG_FOCAL_LENGTH);
			string units = GetFocalUnitsPerMillimetreDescription();
			return BUNDLE["FOCAL_LENGTH", aValue.ToString(), units];
		}

		/// <summary>
		/// Returns Exposure Mode description. 
		/// </summary>
		/// <returns>the Exposure Mode description</returns>
		private string GetExposureModeDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_EXPOSURE_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_EXPOSURE_MODE);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["EASY_SHOOTING"];
				case 1 :
					return BUNDLE["PROGRAM"];
				case 2 :
					return BUNDLE["TV_PRIORITY"];
				case 3 :
					return BUNDLE["AV_PRIORITY"];
				case 4 :
					return BUNDLE["MANUAL"];
				case 5 :
					return BUNDLE["A_DEP"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Af Point Selected description. 
		/// </summary>
		/// <returns>the Af Point Selected description</returns>
		private string GetAfPointSelectedDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_AF_POINT_SELECTED))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_AF_POINT_SELECTED);
			switch (aValue) 
			{
				case 0x3000 :
					return BUNDLE["NONE_MF"];
				case 0x3001 :
					return BUNDLE["AUTO_SELECTED"];
				case 0x3002 :
					return BUNDLE["RIGHT"];
				case 0x3003 :
					return BUNDLE["CENTER"];
				case 0x3004 :
					return BUNDLE["LEFT"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Metering Mode description. 
		/// </summary>
		/// <returns>the Metering Mode description</returns>
		private string GetMeteringModeDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_METERING_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_METERING_MODE);
			switch (aValue) 
			{
				case 3 :
					return BUNDLE["EVALUATIVE"];
				case 4 :
					return BUNDLE["PARTIAL"];
				case 5 :
					return BUNDLE["CENTRE_WEIGHTED"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns ISO description. 
		/// </summary>
		/// <returns>the ISO description</returns>
		private string GetIsoDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_ISO))
				return null;
			int aValue =
				_directory.GetInt(CanonMakernoteDirectory.TAG_CANON_STATE1_ISO);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["ISO_NOT_SPECIFIED"];
				case 15 :
					return BUNDLE["AUTO"];
				case 16 :
					return BUNDLE["ISO_50"];
				case 17 :
					return BUNDLE["ISO_100"];
				case 18 :
					return BUNDLE["ISO_200"];
				case 19 :
					return BUNDLE["ISO_400"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Sharpness description. 
		/// </summary>
		/// <returns>the Sharpness description</returns>
		private string GetSharpnessDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_SHARPNESS))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SHARPNESS);
			switch (aValue) 
			{
				case 0xFFFF :
					return BUNDLE["LOW"];
				case 0x000 :
					return BUNDLE["NORMAL"];
				case 0x001 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Saturation description. 
		/// </summary>
		/// <returns>the Saturation description</returns>
		private string GetSaturationDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_SATURATION))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SATURATION);
			switch (aValue) 
			{
				case 0xFFFF :
					return BUNDLE["LOW"];
				case 0x000 :
					return BUNDLE["NORMAL"];
				case 0x001 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Contrast description. 
		/// </summary>
		/// <returns>the Contrast description</returns>
		private string GetContrastDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_CONTRAST))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_CONTRAST);
			switch (aValue) 
			{
				case 0xFFFF :
					return BUNDLE["LOW"];
				case 0x000 :
					return BUNDLE["NORMAL"];
				case 0x001 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Easy Shooting Mode description. 
		/// </summary>
		/// <returns>the Easy Shooting Mode description</returns>
		private string GetEasyShootingModeDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_EASY_SHOOTING_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_EASY_SHOOTING_MODE);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["FULL_AUTO"];
				case 1 :
					return BUNDLE["MANUAL"];
				case 2 :
					return BUNDLE["LANDSCAPE"];
				case 3 :
					return BUNDLE["FAST_SHUTTER"];
				case 4 :
					return BUNDLE["SLOW_SHUTTER"];
				case 5 :
					return BUNDLE["NIGHT"];
				case 6 :
					return BUNDLE["B_W"];
				case 7 :
					return BUNDLE["SEPIA"];
				case 8 :
					return BUNDLE["PORTRAIT"];
				case 9 :
					return BUNDLE["SPORTS"];
				case 10 :
					return BUNDLE["MACRO_CLOSEUP"];
				case 11 :
					return BUNDLE["PAN_FOCUS"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Image Size description. 
		/// </summary>
		/// <returns>the Image Size description</returns>
		private string GetImageSizeDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_IMAGE_SIZE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_IMAGE_SIZE);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["LARGE"];
				case 1 :
					return BUNDLE["MEDIUM"];
				case 2 :
					return BUNDLE["SMALL"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Focus Mode 1 description. 
		/// </summary>
		/// <returns>the Focus Mode 1 description</returns>
		private string GetFocusMode1Description() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_1))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FOCUS_MODE_1);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["ONE_SHOT"];
				case 1 :
					return BUNDLE["AI_SERVO"];
				case 2 :
					return BUNDLE["AI_FOCUS"];
				case 3 :
					return BUNDLE["MF"];
				case 4 :
					// TODO should check field 32 here (FOCUS_MODE_2)
					return BUNDLE["SINGLE"];
				case 5 :
					return BUNDLE["CONTINUOUS"];
				case 6 :
					return BUNDLE["MF"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Continuous Drive Mode description. 
		/// </summary>
		/// <returns>the Continuous Drive Mode description</returns>
		private string GetContinuousDriveModeDescription()
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory
				.TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_CONTINUOUS_DRIVE_MODE);
			switch (aValue) 
			{
				case 0 :
					if (_directory
						.GetInt(
						CanonMakernoteDirectory
						.TAG_CANON_STATE1_SELF_TIMER_DELAY)
						== 0) 
					{
						return BUNDLE["SINGLE_SHOT"];
					} 
					else 
					{
						return BUNDLE["SINGLE_SHOT_WITH_SELF_TIMER"];
					}
				case 1 :
					return BUNDLE["CONTINUOUS"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Flash Mode description. 
		/// </summary>
		/// <returns>the Flash Mode description</returns>
		private string GetFlashModeDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_FLASH_MODE);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["NO_FLASH_FIRED"];
				case 1 :
					return BUNDLE["AUTO"];
				case 2 :
					return BUNDLE["ON"];
				case 3 :
					return BUNDLE["RED_YEY_REDUCTION"];
				case 4 :
					return BUNDLE["SLOW_SYNCHRO"];
				case 5 :
					return BUNDLE["AUTO_AND_RED_YEY_REDUCTION"];
				case 6 :
					return BUNDLE["ON_AND_RED_YEY_REDUCTION"];
				case 16 :
					// note: this aValue not set on Canon D30
					return BUNDLE["EXTERNAL_FLASH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns Self Timer Delay description. 
		/// </summary>
		/// <returns>the Self Timer Delay description</returns>
		private string GetSelfTimerDelayDescription() 
		{
			if (!_directory
				.ContainsTag(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SELF_TIMER_DELAY))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_SELF_TIMER_DELAY);
			if (aValue == 0) 
			{
				return BUNDLE["SELF_TIMER_DELAY_NOT_USED"];
			} 
			else 
			{
				// TODO find an image that tests this calculation
				return BUNDLE["SELF_TIMER_DELAY", ((double) aValue * 0.1d).ToString()];
			}
		}

		/// <summary>
		/// Returns Macro Mode description. 
		/// </summary>
		/// <returns>the Macro Mode description</returns>
		private string GetMacroModeDescription() 
		{
			if (!_directory
				.ContainsTag(CanonMakernoteDirectory.TAG_CANON_STATE1_MACRO_MODE))
				return null;
			int aValue =
				_directory.GetInt(
				CanonMakernoteDirectory.TAG_CANON_STATE1_MACRO_MODE);
			switch (aValue) 
			{
				case 1 :
					return BUNDLE["MACRO"];
				case 2 :
					return BUNDLE["NORMAL"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}
	}
}