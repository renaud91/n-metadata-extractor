using System;
using System.Collections;
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
	/// Tag descriptor for a casio camera
	/// </summary>
	public class CasioMakernoteDescriptor : TagDescriptor 					
	{
		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="directory">a directory</param>
		public CasioMakernoteDescriptor(Directory directory) : base(directory) 
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
				case CasioMakernoteDirectory.TAG_CASIO_RECORDING_MODE:
					return GetRecordingModeDescription();
				case CasioMakernoteDirectory.TAG_CASIO_QUALITY:
					return GetQualityDescription();
				case CasioMakernoteDirectory.TAG_CASIO_FOCUSING_MODE:
					return GetFocusingModeDescription();
				case CasioMakernoteDirectory.TAG_CASIO_FLASH_MODE:
					return GetFlashModeDescription();
				case CasioMakernoteDirectory.TAG_CASIO_FLASH_INTENSITY:
					return GetFlashIntensityDescription();
				case CasioMakernoteDirectory.TAG_CASIO_OBJECT_DISTANCE:
					return GetObjectDistanceDescription();
				case CasioMakernoteDirectory.TAG_CASIO_WHITE_BALANCE:
					return GetWhiteBalanceDescription();
				case CasioMakernoteDirectory.TAG_CASIO_DIGITAL_ZOOM:
					return GetDigitalZoomDescription();
				case CasioMakernoteDirectory.TAG_CASIO_SHARPNESS:
					return GetSharpnessDescription();
				case CasioMakernoteDirectory.TAG_CASIO_CONTRAST:
					return GetContrastDescription();
				case CasioMakernoteDirectory.TAG_CASIO_SATURATION:
					return GetSaturationDescription();
				case CasioMakernoteDirectory.TAG_CASIO_CCD_SENSITIVITY:
					return GetCcdSensitivityDescription();
				default :
					return _directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Ccd Sensitivity Description. 
		/// </summary>
		/// <returns>the Ccd Sensitivity Description.</returns>
		private string GetCcdSensitivityDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_CCD_SENSITIVITY))
				return null;
			int aValue =
				_directory.GetInt(
				CasioMakernoteDirectory.TAG_CASIO_CCD_SENSITIVITY);
			switch (aValue) 
			{
					// these four for QV3000
				case 64 :
					return BUNDLE["NORMAL"];
				case 125 :
					return BUNDLE["CCD_P_1"];
				case 250 :
					return BUNDLE["CCD_P_2"];
				case 244 :
					return BUNDLE["CCD_P_3"];
					// these two for QV8000/2000
				case 80 :
					return BUNDLE["NORMAL"];
				case 100 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the saturation Description. 
		/// </summary>
		/// <returns>the saturation Description.</returns>
		private string GetSaturationDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_SATURATION))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_SATURATION);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["NORMAL"];
				case 1 :
					return BUNDLE["LOW"];
				case 2 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the contrast Description. 
		/// </summary>
		/// <returns>the contrast Description.</returns>
		private string GetContrastDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_CONTRAST))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_CONTRAST);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["NORMAL"];
				case 1 :
					return BUNDLE["LOW"];
				case 2 :
					return BUNDLE["HIGH"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the sharpness Description. 
		/// </summary>
		/// <returns>the sharpness Description.</returns>
		private string GetSharpnessDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_SHARPNESS))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_SHARPNESS);
			switch (aValue) 
			{
				case 0 :
					return BUNDLE["NORMAL"];
				case 1 :
					return BUNDLE["SOFT"];;
				case 2 :
					return BUNDLE["HARD"];;
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Digital Zoom Description. 
		/// </summary>
		/// <returns>the Digital Zoom Description.</returns>
		private string GetDigitalZoomDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_DIGITAL_ZOOM))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_DIGITAL_ZOOM);
			switch (aValue) 
			{
				case 65536 :
					return BUNDLE["NO_DIGITAL_ZOOM"];
				case 65537 :
					return BUNDLE["2_X_DIGITAL_ZOOM"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the White Balance Description. 
		/// </summary>
		/// <returns>the White Balance Description.</returns>
		private string GetWhiteBalanceDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_WHITE_BALANCE))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_WHITE_BALANCE);
			switch (aValue) 
			{
				case 1 :
					return BUNDLE["AUTO"];
				case 2 :
					return BUNDLE["TUNGSTEN"];
				case 3 :
					return BUNDLE["DAYLIGHT"];
				case 4 :
					return BUNDLE["FLOURESCENT"];
				case 5 :
					return BUNDLE["SHADE"];
				case 129 :
					return BUNDLE["MANUAL"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Object Distance Description. 
		/// </summary>
		/// <returns>the Object Distance Description.</returns>
		private string GetObjectDistanceDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_OBJECT_DISTANCE))
				return null;
			int aValue =
				_directory.GetInt(
				CasioMakernoteDirectory.TAG_CASIO_OBJECT_DISTANCE);
			return BUNDLE["DISTANCE_MM", aValue.ToString()];
		}

		/// <summary>
		/// Returns the Flash Intensity Description. 
		/// </summary>
		/// <returns>the Flash Intensity Description.</returns>
		private string GetFlashIntensityDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_FLASH_INTENSITY))
				return null;
			int aValue =
				_directory.GetInt(
				CasioMakernoteDirectory.TAG_CASIO_FLASH_INTENSITY);
			switch (aValue) 
			{
				case 11 :
					return BUNDLE["WEAK"];
				case 13 :
					return BUNDLE["NORMAL"];
				case 15 :
					return BUNDLE["STRONG"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Flash Mode Description. 
		/// </summary>
		/// <returns>the Flash Mode Description.</returns>
		private string GetFlashModeDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_FLASH_MODE))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_FLASH_MODE);
			switch (aValue) 
			{
				case 1 :
					return BUNDLE["AUTO"];
				case 2 :
					return BUNDLE["ON"];
				case 3 :
					return BUNDLE["OFF"];
				case 4 :
					return BUNDLE["RED_YEY_REDUCTION"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Focusing Mode Description. 
		/// </summary>
		/// <returns>the Focusing Mode Description.</returns>
		private string GetFocusingModeDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_FOCUSING_MODE))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_FOCUSING_MODE);
			switch (aValue) 
			{
				case 2 :
					return BUNDLE["MACRO"];
				case 3 :
					return BUNDLE["AUTO_FOCUS"];
				case 4 :
					return BUNDLE["MANUAL_FOCUS"];
				case 5 :
					return BUNDLE["INFINITY"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the quality Description. 
		/// </summary>
		/// <returns>the quality Description.</returns>
		private string GetQualityDescription() 
		{
			if (!_directory.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_QUALITY))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_QUALITY);
			switch (aValue) 
			{
				case 1 :
					return BUNDLE["ECONOMY"];
				case 2 :
					return BUNDLE["NORMAL"];
				case 3 :
					return BUNDLE["FINE"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Focussing Mode Description. 
		/// </summary>
		/// <returns>the Focussing Mode Description.</returns>
		private string GetFocussingModeDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_FOCUSING_MODE))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_FOCUSING_MODE);
			switch (aValue) 
			{
				case 2 :
					return BUNDLE["MACRO"];
				case 3 :
					return BUNDLE["AUTO_FOCUS"];
				case 4 :
					return BUNDLE["MANUAL_FOCUS"];
				case 5 :
					return BUNDLE["INFINITY"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}

		/// <summary>
		/// Returns the Recording Mode Description. 
		/// </summary>
		/// <returns>the Recording Mode Description.</returns>
		private string GetRecordingModeDescription() 
		{
			if (!_directory
				.ContainsTag(CasioMakernoteDirectory.TAG_CASIO_RECORDING_MODE))
				return null;
			int aValue =
				_directory.GetInt(CasioMakernoteDirectory.TAG_CASIO_RECORDING_MODE);
			switch (aValue) 
			{
				case 1 :
					return BUNDLE["SINGLE_SHUTTER"];
				case 2 :
					return BUNDLE["PANORAMA"];
				case 3 :
					return BUNDLE["NIGHT_SCENE"];
				case 4 :
					return BUNDLE["PORTRAIT"];
				case 5 :
					return BUNDLE["LANDSCAPE"];
				default :
					return BUNDLE["UNKNOWN", aValue.ToString()];
			}
		}
	}
}