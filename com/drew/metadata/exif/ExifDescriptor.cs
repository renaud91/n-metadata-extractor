using System;
using System.Collections;
using System.Text;
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
	/// Tag descriptor for almost every images
	/// </summary>
	public class ExifDescriptor : TagDescriptor 
	{
		/// <summary>
		/// Dictates whether rational values will be represented in decimal format in instances 
		/// where decimal notation is elegant (such as 1/2 -> 0.5, but not 1/3).
		/// </summary>
		private readonly bool _allowDecimalRepresentationOfRationals = true;

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="directory">a directory</param>
		public ExifDescriptor(Directory directory) : base(directory)
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
				case ExifDirectory.TAG_ORIENTATION:
					return GetOrientationDescription();
				case ExifDirectory.TAG_RESOLUTION_UNIT:
					return GetResolutionDescription();
				case ExifDirectory.TAG_YCBCR_POSITIONING:
					return GetYCbCrPositioningDescription();
				case ExifDirectory.TAG_EXPOSURE_TIME:
					return GetExposureTimeDescription();
				case ExifDirectory.TAG_SHUTTER_SPEED:
					return GetShutterSpeedDescription();
				case ExifDirectory.TAG_FNUMBER:
					return GetFNumberDescription();
				case ExifDirectory.TAG_X_RESOLUTION:
					return GetXResolutionDescription();
				case ExifDirectory.TAG_Y_RESOLUTION:
					return GetYResolutionDescription();
				case ExifDirectory.TAG_THUMBNAIL_OFFSET:
					return GetThumbnailOffSetDescription();
				case ExifDirectory.TAG_THUMBNAIL_LENGTH:
					return GetThumbnailLengthDescription();
				case ExifDirectory.TAG_COMPRESSION_LEVEL:
					return GetCompressionLevelDescription();
				case ExifDirectory.TAG_SUBJECT_DISTANCE:
					return GetSubjectDistanceDescription();
				case ExifDirectory.TAG_METERING_MODE:
					return GetMeteringModeDescription();
				case ExifDirectory.TAG_WHITE_BALANCE:
					return GetWhiteBalanceDescription();
				case ExifDirectory.TAG_FLASH:
					return GetFlashDescription();
				case ExifDirectory.TAG_FOCAL_LENGTH:
					return GetFocalLengthDescription();
				case ExifDirectory.TAG_COLOR_SPACE:
					return GetColorSpaceDescription();
				case ExifDirectory.TAG_EXIF_IMAGE_WIDTH:
					return GetExifImageWidthDescription();
				case ExifDirectory.TAG_EXIF_IMAGE_HEIGHT:
					return GetExifImageHeightDescription();
				case ExifDirectory.TAG_FOCAL_PLANE_UNIT:
					return GetFocalPlaneResolutionUnitDescription();
				case ExifDirectory.TAG_FOCAL_PLANE_X_RES:
					return GetFocalPlaneXResolutionDescription();
				case ExifDirectory.TAG_FOCAL_PLANE_Y_RES:
					return GetFocalPlaneYResolutionDescription();
				case ExifDirectory.TAG_THUMBNAIL_IMAGE_WIDTH:
					return GetThumbnailImageWidthDescription();
				case ExifDirectory.TAG_THUMBNAIL_IMAGE_HEIGHT:
					return GetThumbnailImageHeightDescription();
				case ExifDirectory.TAG_BITS_PER_SAMPLE:
					return GetBitsPerSampleDescription();
				case ExifDirectory.TAG_COMPRESSION:
					return GetCompressionDescription();
				case ExifDirectory.TAG_PHOTOMETRIC_INTERPRETATION:
					return GetPhotometricInterpretationDescription();
				case ExifDirectory.TAG_ROWS_PER_STRIP:
					return GetRowsPerStripDescription();
				case ExifDirectory.TAG_STRIP_BYTE_COUNTS:
					return GetStripByteCountsDescription();
				case ExifDirectory.TAG_SAMPLES_PER_PIXEL:
					return GetSamplesPerPixelDescription();
				case ExifDirectory.TAG_PLANAR_CONFIGURATION:
					return GetPlanarConfigurationDescription();
				case ExifDirectory.TAG_YCBCR_SUBSAMPLING:
					return GetYCbCrSubsamplingDescription();
				case ExifDirectory.TAG_EXPOSURE_PROGRAM:
					return GetExposureProgramDescription();
				case ExifDirectory.TAG_APERTURE:
					return GetApertureValueDescription();
				case ExifDirectory.TAG_MAX_APERTURE:
					return GetMaxApertureValueDescription();
				case ExifDirectory.TAG_SENSING_METHOD:
					return GetSensingMethodDescription();
				case ExifDirectory.TAG_EXPOSURE_BIAS:
					return GetExposureBiasDescription();
				case ExifDirectory.TAG_FILE_SOURCE:
					return GetFileSourceDescription();
				case ExifDirectory.TAG_SCENE_TYPE:
					return GetSceneTypeDescription();
				case ExifDirectory.TAG_COMPONENTS_CONFIGURATION:
					return GetComponentConfigurationDescription();
				case ExifDirectory.TAG_EXIF_VERSION:
					return GetExifVersionDescription();
				case ExifDirectory.TAG_FLASHPIX_VERSION:
					return GetFlashPixVersionDescription();
				case ExifDirectory.TAG_REFERENCE_BLACK_WHITE:
					return GetReferenceBlackWhiteDescription();
				case ExifDirectory.TAG_ISO_EQUIVALENT:
					return GetIsoEquivalentDescription();
				case ExifDirectory.TAG_THUMBNAIL_DATA:
					return GetThumbnailDescription();
				case ExifDirectory.TAG_XP_AUTHOR:
					return GetXPAuthorDescription();
				case ExifDirectory.TAG_XP_COMMENTS:
					return GetXPCommentsDescription();
				case ExifDirectory.TAG_XP_KEYWORDS:
					return GetXPKeywordsDescription();
				case ExifDirectory.TAG_XP_SUBJECT:
					return GetXPSubjectDescription();
				case ExifDirectory.TAG_XP_TITLE:
					return GetXPTitleDescription();
				default :
					return _directory.GetString(tagType);
			}
		}

		/// <summary>
		/// Returns the Thumbnail Description. 
		/// </summary>
		/// <returns>the Thumbnail Description.</returns>
		private string GetThumbnailDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_DATA))
				return null;
			int[] thumbnailBytes =
				_directory.GetIntArray(ExifDirectory.TAG_THUMBNAIL_DATA);
			return BUNDLE["THUMBNAIL_BYTES", thumbnailBytes.Length.ToString()];
		}

		/// <summary>
		/// Returns the Iso Equivalent Description. 
		/// </summary>
		/// <returns>the Iso Equivalent Description.</returns>
		private string GetIsoEquivalentDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_ISO_EQUIVALENT))
				return null;
			int isoEquiv = _directory.GetInt(ExifDirectory.TAG_ISO_EQUIVALENT);
			if (isoEquiv < 50) 
			{
				isoEquiv *= 200;
			}
			return isoEquiv.ToString();
		}

		/// <summary>
		/// Returns the Reference Black White Description. 
		/// </summary>
		/// <returns>the Reference Black White Description.</returns>
		private string GetReferenceBlackWhiteDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_REFERENCE_BLACK_WHITE))
				return null;
			int[] ints =
				_directory.GetIntArray(ExifDirectory.TAG_REFERENCE_BLACK_WHITE);

			string[] sPos = new string[] {ints[0].ToString(), ints[1].ToString(),ints[2].ToString(),ints[3].ToString(),ints[4].ToString(),ints[5].ToString()};
			return BUNDLE["POS",sPos];
		}

		/// <summary>
		/// Returns the Exif Version Description. 
		/// </summary>
		/// <returns>the Exif Version Description.</returns>
		private string GetExifVersionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXIF_VERSION))
				return null;
			int[] ints = _directory.GetIntArray(ExifDirectory.TAG_EXIF_VERSION);
			return ExifDescriptor.ConvertBytesToVersionString(ints);
		}

		/// <summary>
		/// Returns the Flash Pix Version Description. 
		/// </summary>
		/// <returns>the Flash Pix Version Description.</returns>
		private string GetFlashPixVersionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FLASHPIX_VERSION))
				return null;
			int[] ints = _directory.GetIntArray(ExifDirectory.TAG_FLASHPIX_VERSION);
			return ExifDescriptor.ConvertBytesToVersionString(ints);
		}

		/// <summary>
		/// Returns the Scene Type Description. 
		/// </summary>
		/// <returns>the Scene Type Description.</returns>
		private string GetSceneTypeDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_SCENE_TYPE))
				return null;
			int sceneType = _directory.GetInt(ExifDirectory.TAG_SCENE_TYPE);
			if (sceneType == 1) 
			{
				return BUNDLE["DIRECTLY_PHOTOGRAPHED_IMAGE"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN", sceneType.ToString()];
			}
		}

		/// <summary>
		/// Returns the File Source Description. 
		/// </summary>
		/// <returns>the File Source Description.</returns>
		private string GetFileSourceDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FILE_SOURCE))
				return null;
			int fileSource = _directory.GetInt(ExifDirectory.TAG_FILE_SOURCE);
			if (fileSource == 3) 
			{
				return BUNDLE["DIGITAL_STILL_CAMERA"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN", fileSource.ToString()];
			}
		}

		/// <summary>
		/// Returns the Exposure Bias Description. 
		/// </summary>
		/// <returns>the Exposure Bias Description.</returns>
		private string GetExposureBiasDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXPOSURE_BIAS))
				return null;
			Rational exposureBias =
				_directory.GetRational(ExifDirectory.TAG_EXPOSURE_BIAS);
			return exposureBias.ToSimpleString(true);
		}

		/// <summary>
		/// Returns the Max Aperture Value Description. 
		/// </summary>
		/// <returns>the Max Aperture Value Description.</returns>
		private string GetMaxApertureValueDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_MAX_APERTURE))
				return null;
			double apertureApex =
				_directory.GetDouble(ExifDirectory.TAG_MAX_APERTURE);
			double rootTwo = Math.Sqrt(2);
			double fStop = Math.Pow(rootTwo, apertureApex);
			return BUNDLE["APERTURE", fStop.ToString("0.#")];
		}

		/// <summary>
		/// Returns the Aperture Value Description. 
		/// </summary>
		/// <returns>the Aperture Value Description.</returns>
		private string GetApertureValueDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_APERTURE))
				return null;
			double apertureApex = _directory.GetDouble(ExifDirectory.TAG_APERTURE);
			double rootTwo = Math.Sqrt(2);
			double fStop = Math.Pow(rootTwo, apertureApex);
			return BUNDLE["APERTURE", fStop.ToString("0.#")];
		}

		/// <summary>
		/// Returns the Exposure Program Description. 
		/// </summary>
		/// <returns>the Exposure Program Description.</returns>
		private string GetExposureProgramDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXPOSURE_PROGRAM))
				return null;
			// '1' means manual control, '2' program normal, '3' aperture priority,
			// '4' shutter priority, '5' program creative (slow program),
			// '6' program action(high-speed program), '7' portrait mode, '8' landscape mode.
			switch (_directory.GetInt(ExifDirectory.TAG_EXPOSURE_PROGRAM)) 
			{
				case 1 :
					return BUNDLE["MANUAL_CONTROL"];
				case 2 :
					return BUNDLE["PROGRAM_NORMAL"];
				case 3 :
					return BUNDLE["APERTURE_PRIORITY"];
				case 4 :
					return BUNDLE["SHUTTER_PRIORITY"];
				case 5 :
					return BUNDLE["PROGRAM_CREATIVE"];
				case 6 :
					return BUNDLE["PROGRAM_ACTION"];
				case 7 :
					return BUNDLE["PORTRAIT_MODE"];
				case 8 :
					return BUNDLE["LANDSCAPE_MODE"];
				default :
					return BUNDLE["UNKNOWN_PROGRAM", _directory.GetInt(ExifDirectory.TAG_EXPOSURE_PROGRAM).ToString()];
			}
		}

		/// <summary>
		/// Returns the YCbCr Subsampling Description. 
		/// </summary>
		/// <returns>the YCbCr Subsampling Description.</returns>
		private string GetYCbCrSubsamplingDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_YCBCR_SUBSAMPLING))
				return null;
			int[] positions =
				_directory.GetIntArray(ExifDirectory.TAG_YCBCR_SUBSAMPLING);
			if (positions[0] == 2 && positions[1] == 1) 
			{
				return BUNDLE["YCBCR_422"];
			} 
			else if (positions[0] == 2 && positions[1] == 2) 
			{
				return BUNDLE["YCBCR_420"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN"];
			}
		}

		/// <summary>
		/// Returns the Planar Configuration Description. 
		/// </summary>
		/// <returns>the Planar Configuration Description.</returns>
		private string GetPlanarConfigurationDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_PLANAR_CONFIGURATION))
				return null;
			// When image format is no compression YCbCr, this aValue shows byte aligns of YCbCr
			// data. If aValue is '1', Y/Cb/Cr aValue is chunky format, contiguous for each subsampling
			// pixel. If aValue is '2', Y/Cb/Cr aValue is separated and stored to Y plane/Cb plane/Cr
			// plane format.

			switch (_directory.GetInt(ExifDirectory.TAG_PLANAR_CONFIGURATION)) 
			{
				case 1 :
					return BUNDLE["CHUNKY"];
				case 2 :
					return BUNDLE["SEPARATE"];
				default :
					return BUNDLE["UNKNOWN_CONFIGURATION"];
			}
		}

		/// <summary>
		/// Returns the Samples Per Pixel Description. 
		/// </summary>
		/// <returns>the Samples Per Pixel Description.</returns>
		private string GetSamplesPerPixelDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_SAMPLES_PER_PIXEL))
				return null;
			return BUNDLE["SAMPLES_PIXEL", _directory.GetString(ExifDirectory.TAG_SAMPLES_PER_PIXEL)];
		}

		/// <summary>
		/// Returns the Rows Per Strip Description. 
		/// </summary>
		/// <returns>the Rows Per Strip Description.</returns>
		private string GetRowsPerStripDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_ROWS_PER_STRIP))
				return null;
			return BUNDLE["ROWS_STRIP", _directory.GetString(ExifDirectory.TAG_ROWS_PER_STRIP)];
		}

		/// <summary>
		/// Returns the Strip Byte Counts Description. 
		/// </summary>
		/// <returns>the Strip Byte Counts Description.</returns>
		private string GetStripByteCountsDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_STRIP_BYTE_COUNTS))
				return null;
			return BUNDLE["BYTES", _directory.GetString(ExifDirectory.TAG_STRIP_BYTE_COUNTS)];
		}

		/// <summary>
		/// Returns the Photometric Interpretation Description. 
		/// </summary>
		/// <returns>the Photometric Interpretation Description.</returns>
		private string GetPhotometricInterpretationDescription()
		{
			if (!_directory
				.ContainsTag(ExifDirectory.TAG_PHOTOMETRIC_INTERPRETATION))
				return null;
			// Shows the color space of the image data components. '1' means monochrome,
			// '2' means RGB, '6' means YCbCr.
			switch (_directory
				.GetInt(ExifDirectory.TAG_PHOTOMETRIC_INTERPRETATION)) 
			{
				case 1 :
					return BUNDLE["MONOCHROME"];
				case 2 :
					return BUNDLE["RGB"];
				case 6 :
					return BUNDLE["YCBCR"];
				default :
					return BUNDLE["UNKNOWN_COLOUR_SPACE"];
			}
		}

		/// <summary>
		/// Returns the Compression Description. 
		/// </summary>
		/// <returns>the Compression Description.</returns>
		private string GetCompressionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_COMPRESSION))
				return null;
			// '1' means no compression, '6' means JPEG compression.
			switch (_directory.GetInt(ExifDirectory.TAG_COMPRESSION)) 
			{
				case 1 :
					return BUNDLE["NO_COMPRESSION"];
				case 6 :
					return BUNDLE["JPEG_COMPRESSION"];
				default :
					return BUNDLE["UNKNOWN_COMPRESSION"];
			}
		}

		/// <summary>
		/// Returns the Bits Per Sample Description. 
		/// </summary>
		/// <returns>the Bits Per Sample Description.</returns>
		private string GetBitsPerSampleDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_BITS_PER_SAMPLE))
				return null;
			return BUNDLE["BITS_COMPONENT_PIXEL",_directory.GetString(ExifDirectory.TAG_BITS_PER_SAMPLE)];
		}

		/// <summary>
		/// Returns the Thumbnail Image Width Description. 
		/// </summary>
		/// <returns>the Thumbnail Image Width Description.</returns>
		private string GetThumbnailImageWidthDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_IMAGE_WIDTH))
				return null;
			return BUNDLE["PIXELS", _directory.GetString(ExifDirectory.TAG_THUMBNAIL_IMAGE_WIDTH)];
		}

		/// <summary>
		/// Returns the Thumbnail Image Height Description. 
		/// </summary>
		/// <returns>the Thumbnail Image Height Description.</returns>
		private string GetThumbnailImageHeightDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_IMAGE_HEIGHT))
				return null;
			return BUNDLE["PIXELS", _directory.GetString(ExifDirectory.TAG_THUMBNAIL_IMAGE_HEIGHT)];
		}

		/// <summary>
		/// Returns the Focal Plane X Resolution Description. 
		/// </summary>
		/// <returns>the Focal Plane X Resolution Description.</returns>
		private string GetFocalPlaneXResolutionDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FOCAL_PLANE_X_RES))
				return null;
			Rational rational =
				_directory.GetRational(ExifDirectory.TAG_FOCAL_PLANE_X_RES);
			return BUNDLE["FOCAL_PLANE", rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals),
			GetFocalPlaneResolutionUnitDescription().ToLower()];
		}

		/// <summary>
		/// Returns the Focal Plane Y Resolution Description. 
		/// </summary>
		/// <returns>the Focal Plane Y Resolution Description.</returns>
		private string GetFocalPlaneYResolutionDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_COMPRESSION))
				return null;
			Rational rational =
				_directory.GetRational(ExifDirectory.TAG_FOCAL_PLANE_Y_RES);
			return BUNDLE["FOCAL_PLANE", rational.GetReciprocal().ToSimpleString(_allowDecimalRepresentationOfRationals),
				GetFocalPlaneResolutionUnitDescription().ToLower()];
		}

		/// <summary>
		/// Returns the Focal Plane Resolution Unit Description. 
		/// </summary>
		/// <returns>the Focal Plane Resolution Unit Description.</returns>
		private string GetFocalPlaneResolutionUnitDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FOCAL_PLANE_UNIT))
				return null;
			// Unit of FocalPlaneXResoluton/FocalPlaneYResolution. '1' means no-unit,
			// '2' inch, '3' centimeter.
			switch (_directory.GetInt(ExifDirectory.TAG_FOCAL_PLANE_UNIT)) 
			{
				case 1 :
					return BUNDLE["NO_UNIT"];
				case 2 :
					return BUNDLE["INCHES"];
				case 3 :
					return BUNDLE["CM"];
				default :
					return "";
			}
		}

		/// <summary>
		/// Returns the Exif Image Width Description. 
		/// </summary>
		/// <returns>the Exif Image Width Description.</returns>
		private string GetExifImageWidthDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXIF_IMAGE_WIDTH))
				return null;
			return BUNDLE["PIXELS", _directory.GetInt(ExifDirectory.TAG_EXIF_IMAGE_WIDTH).ToString()];
		}

		/// <summary>
		/// Returns the Exif Image Height Description. 
		/// </summary>
		/// <returns>the Exif Image Height Description.</returns>
		private string GetExifImageHeightDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXIF_IMAGE_HEIGHT))
				return null;
			return BUNDLE["PIXELS", _directory.GetInt(ExifDirectory.TAG_EXIF_IMAGE_HEIGHT).ToString()];
		}

		/// <summary>
		/// Returns the Color Space Description. 
		/// </summary>
		/// <returns>the Color Space Description.</returns>
		private string GetColorSpaceDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_COLOR_SPACE))
				return null;
			int colorSpace = _directory.GetInt(ExifDirectory.TAG_COLOR_SPACE);
			if (colorSpace == 1) 
			{
				return BUNDLE["SRGB"];
			} 
			else if (colorSpace == 65535) 
			{
				return BUNDLE["UNDEFINED"];
			} 
			else 
			{
				return BUNDLE["UNKNOWN"];
			}
		}

		/// <summary>
		/// Returns the Focal Length Description. 
		/// </summary>
		/// <returns>the Focal Length Description.</returns>
		private string GetFocalLengthDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FOCAL_LENGTH))
				return null;
			Rational focalLength =
				_directory.GetRational(ExifDirectory.TAG_FOCAL_LENGTH);
			return BUNDLE["DISTANCE_MM", (focalLength.DoubleValue()).ToString("0.0##")];
		}

		/// <summary>
		/// Returns the Flash Description. 
		/// </summary>
		/// <returns>the Flash Description.</returns>
		private string GetFlashDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FLASH))
				return null;
			// '0' means flash did not fire, '1' flash fired, '5' flash fired but strobe return
			// light not detected, '7' flash fired and strobe return light detected.
			switch (_directory.GetInt(ExifDirectory.TAG_FLASH)) 
			{
				case 0 :
					return BUNDLE["NO_FLASH_FIRED"];
				case 1 :
					return BUNDLE["FLASH_FIRED"];
				case 5 :
					return BUNDLE["FLASH_FIRED_LIGHT_NOT_DETECTED"];
				case 7 :
					return BUNDLE["FLASH_FIRED_LIGHT_DETECTED"];
				default :
					return BUNDLE["UNKNOWN", _directory.GetInt(ExifDirectory.TAG_FLASH).ToString()];
			}
		}

		/// <summary>
		/// Returns the White Balance Description. 
		/// </summary>
		/// <returns>the White Balance Description.</returns>
		private string GetWhiteBalanceDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_WHITE_BALANCE))
				return null;
			// '0' means unknown, '1' daylight, '2' fluorescent, '3' tungsten, '10' flash,
			// '17' standard light A, '18' standard light B, '19' standard light C, '20' D55,
			// '21' D65, '22' D75, '255' other.
			switch (_directory.GetInt(ExifDirectory.TAG_WHITE_BALANCE)) 
			{
				case 0 :
					return BUNDLE["UNKNOWN"];
				case 1 :
					return BUNDLE["DAYLIGHT"];
				case 2 :
					return BUNDLE["FLOURESCENT"];
				case 3 :
					return BUNDLE["TUNGSTEN"];
				case 10 :
					return BUNDLE["FLASH"];
				case 17 :
					return BUNDLE["STANDARD_LIGHT"];
				case 18 :
					return BUNDLE["STANDARD_LIGHT_B"];
				case 19 :
					return BUNDLE["STANDARD_LIGHT_C"];
				case 20 :
					return BUNDLE["D55"];
				case 21 :
					return BUNDLE["D65"];
				case 22 :
					return BUNDLE["D75"];
				case 255 :
					return BUNDLE["OTHER"];
				default :
					return BUNDLE["UNKNOWN", _directory.GetInt(ExifDirectory.TAG_WHITE_BALANCE).ToString()];
			}
		}

		/// <summary>
		/// Returns the Metering Mode Description. 
		/// </summary>
		/// <returns>the Metering Mode Description.</returns>
		private string GetMeteringModeDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_METERING_MODE))
				return null;
			// '0' means unknown, '1' average, '2' center weighted average, '3' spot
			// '4' multi-spot, '5' multi-segment, '6' partial, '255' other
			int meteringMode = _directory.GetInt(ExifDirectory.TAG_METERING_MODE);
			switch (meteringMode) 
			{
				case 0 :
					return BUNDLE["UNKNOWN"];
				case 1 :
					return BUNDLE["AVERAGE"];
				case 2 :
					return BUNDLE["CENTER_WEIGHTED_AVERAGE"];
				case 3 :
					return BUNDLE["SPOT"];
				case 4 :
					return BUNDLE["MULTI_SPOT"];
				case 5 :
					return BUNDLE["MULTI_SEGMENT"];
				case 6 :
					return BUNDLE["PARTIAL"];
				case 255 :
					return BUNDLE["OTHER"];
				default :
					return "";
			}
		}

		/// <summary>
		/// Returns the Subject Distance Description. 
		/// </summary>
		/// <returns>the Subject Distance Description.</returns>
		private string GetSubjectDistanceDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_SUBJECT_DISTANCE))
				return null;
			Rational distance =
				_directory.GetRational(ExifDirectory.TAG_SUBJECT_DISTANCE);
			return BUNDLE["METRES", (distance.DoubleValue()).ToString("0.0##")];
		}

		/// <summary>
		/// Returns the Compression Level Description. 
		/// </summary>
		/// <returns>the Compression Level Description.</returns>
		private string GetCompressionLevelDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_COMPRESSION_LEVEL))
				return null;
			Rational compressionRatio =
				_directory.GetRational(ExifDirectory.TAG_COMPRESSION_LEVEL);
			string ratio =
				compressionRatio.ToSimpleString(
				_allowDecimalRepresentationOfRationals);
			if (compressionRatio.IsInteger() && compressionRatio.IntValue() == 1) 
			{
				return BUNDLE["BIT_PIXEL", ratio];
			} 
			else 
			{
				return BUNDLE["BITS_PIXEL", ratio];
			}
		}

		/// <summary>
		/// Returns the Thumbnail Length Description. 
		/// </summary>
		/// <returns>the Thumbnail Length Description.</returns>
		private string GetThumbnailLengthDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_LENGTH))
				return null;
			return BUNDLE["BYTES", _directory.GetString(ExifDirectory.TAG_THUMBNAIL_LENGTH)];
		}

		/// <summary>
		/// Returns the Thumbnail OffSet Description. 
		/// </summary>
		/// <returns>the Thumbnail OffSet Description.</returns>
		private string GetThumbnailOffSetDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_OFFSET))
				return null;
			return BUNDLE["BYTES", _directory.GetString(ExifDirectory.TAG_THUMBNAIL_OFFSET)];
		}

		/// <summary>
		/// Returns the Y Resolution Description. 
		/// </summary>
		/// <returns>the Y Resolution Description.</returns>
		private string GetYResolutionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_Y_RESOLUTION))
				return null;
			Rational resolution =
				_directory.GetRational(ExifDirectory.TAG_Y_RESOLUTION);
			return BUNDLE["DOTS_PER", resolution.ToSimpleString(_allowDecimalRepresentationOfRationals),GetResolutionDescription().ToLower()];
		}

		/// <summary>
		/// Returns the X Resolution Description. 
		/// </summary>
		/// <returns>the X Resolution Description.</returns>
		private string GetXResolutionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_X_RESOLUTION))
				return null;
			Rational resolution =
				_directory.GetRational(ExifDirectory.TAG_X_RESOLUTION);
			return BUNDLE["DOTS_PER", resolution.ToSimpleString(_allowDecimalRepresentationOfRationals),GetResolutionDescription().ToLower()];
		}

		/// <summary>
		/// Returns the Exposure Time Description. 
		/// </summary>
		/// <returns>the Exposure Time Description.</returns>
		private string GetExposureTimeDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_EXPOSURE_TIME))
				return null;
			return BUNDLE["SEC", _directory.GetString(ExifDirectory.TAG_EXPOSURE_TIME)];
		}

		/// <summary>
		/// Returns the Shutter Speed Description. 
		/// </summary>
		/// <returns>the Shutter Speed Description.</returns>
		private string GetShutterSpeedDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_SHUTTER_SPEED))
				return null;
			// Incorrect math bug fixed by Hendrik Wördehoff - 20 Sep 2002
			int apexValue = _directory.GetInt(ExifDirectory.TAG_SHUTTER_SPEED);
			// int apexPower = (int)(Math.pow(2.0, apexValue) + 0.5);
			// addition of 0.5 removed upon suggestion of Varuni Witana, who 
			// detected incorrect values for Canon cameras,
			// which calculate both shutterspeed and exposuretime
			int apexPower = (int) Math.Pow(2.0, apexValue);
			return BUNDLE["SHUTTER_SPEED", apexPower.ToString()];
		}

		/// <summary>
		/// Returns the F Number Description. 
		/// </summary>
		/// <returns>the F Number Description.</returns>
		private string GetFNumberDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_FNUMBER))
				return null;
			Rational fNumber = _directory.GetRational(ExifDirectory.TAG_FNUMBER);
			return BUNDLE["APERTURE", fNumber.DoubleValue().ToString("0.#")];
		}

		/// <summary>
		/// Returns the YCbCr Positioning Description. 
		/// </summary>
		/// <returns>the YCbCr Positioning Description.</returns>
		private string GetYCbCrPositioningDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_YCBCR_POSITIONING))
				return null;
			int yCbCrPosition =
				_directory.GetInt(ExifDirectory.TAG_YCBCR_POSITIONING);
			switch (yCbCrPosition) 
			{
				case 1 :
					return BUNDLE["CENTER_OF_PIXEL_ARRAY"];
				case 2 :
					return BUNDLE["DATUM_POINT"];
				default :
					return yCbCrPosition.ToString();
			}
		}

		/// <summary>
		/// Returns the Orientation Description. 
		/// </summary>
		/// <returns>the Orientation Description.</returns>
		private string GetOrientationDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_ORIENTATION))
				return null;
			int orientation = _directory.GetInt(ExifDirectory.TAG_ORIENTATION);
			switch (orientation) 
			{
				case 1 :
					return BUNDLE["TOP_LEFT_SIDE"];
				case 2 :
					return BUNDLE["TOP_RIGHT_SIDE"];
				case 3 :
					return BUNDLE["BOTTOM_RIGHT_SIDE"];
				case 4 :
					return BUNDLE["BOTTOM_LEFT_SIDE"];
				case 5 :
					return BUNDLE["LEFT_SIDE_TOP"];
				case 6 :
					return BUNDLE["RIGHT_SIDE_TOP"];
				case 7 :
					return BUNDLE["RIGHT_SIDE_BOTTOM"];
				case 8 :
					return BUNDLE["LEFT_SIDE_BOTTOM"];
				default :
					return orientation.ToString();
			}
		}

		/// <summary>
		/// Returns the Resolution Description. 
		/// </summary>
		/// <returns>the Resolution Description.</returns>
		private string GetResolutionDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_RESOLUTION_UNIT))
				return "";
			// '1' means no-unit, '2' means inch, '3' means centimeter. Default aValue is '2'(inch)
			int resolutionUnit =
				_directory.GetInt(ExifDirectory.TAG_RESOLUTION_UNIT);
			switch (resolutionUnit) 
			{
				case 1 :
					return BUNDLE["NO_UNIT"];
				case 2 :
					return BUNDLE["INCHES"];
				case 3 :
					return BUNDLE["CM"];
				default :
					return "";
			}
		}

		/// <summary>
		/// Returns the Sensing Method Description. 
		/// </summary>
		/// <returns>the Sensing Method Description.</returns>
		private string GetSensingMethodDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_SENSING_METHOD))
				return null;
			// '1' Not defined, '2' One-chip color area sensor, '3' Two-chip color area sensor
			// '4' Three-chip color area sensor, '5' Color sequential area sensor
			// '7' Trilinear sensor '8' Color sequential linear sensor,  'Other' reserved
			int sensingMethod = _directory.GetInt(ExifDirectory.TAG_SENSING_METHOD);
			switch (sensingMethod) 
			{
				case 1 :
					return BUNDLE["NOT_DEFINED"];
				case 2 :
					return BUNDLE["ONE_CHIP_COLOR"];
				case 3 :
					return BUNDLE["TWO_CHIP_COLOR"];
				case 4 :
					return BUNDLE["THREE_CHIP_COLOR"];
				case 5 :
					return BUNDLE["COLOR_SEQUENTIAL"];
				case 7 :
					return BUNDLE["TRILINEAR_SENSOR"];
				case 8 :
					return BUNDLE["COLOR_SEQUENTIAL_LINEAR"];
				default :
					return "";
			}
		}

		/// <summary>
		/// Returns the XP author description. 
		/// </summary>
		/// <returns>the XP author description.</returns>
		private string GetXPAuthorDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_XP_AUTHOR))
				return null;
			return Utils.Decode(_directory.GetByteArray(ExifDirectory.TAG_XP_AUTHOR), true);
		}

		/// <summary>
		/// Returns the XP comments description. 
		/// </summary>
		/// <returns>the XP comments description.</returns>
		private string GetXPCommentsDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_XP_COMMENTS))
				return null;
			return Utils.Decode(_directory.GetByteArray(ExifDirectory.TAG_XP_COMMENTS), true);
        } 

		/// <summary>
		/// Returns the XP keywords description. 
		/// </summary>
		/// <returns>the XP keywords description.</returns>
		private string  GetXPKeywordsDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_XP_KEYWORDS))
				return null;
			return Utils.Decode(_directory.GetByteArray(ExifDirectory.TAG_XP_KEYWORDS), true);
		} 

		/// <summary>
		/// Returns the XP subject description. 
		/// </summary>
		/// <returns>the XP subject description.</returns>
		private string  GetXPSubjectDescription()
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_XP_SUBJECT))
				return null;
			return Utils.Decode(_directory.GetByteArray(ExifDirectory.TAG_XP_SUBJECT), true);
		} 

		/// <summary>
		/// Returns the XP title description. 
		/// </summary>
		/// <returns>the XP title description.</returns>
		private string  GetXPTitleDescription() 
		{
			if (!_directory.ContainsTag(ExifDirectory.TAG_XP_TITLE))
				return null;
			return Utils.Decode(_directory.GetByteArray(ExifDirectory.TAG_XP_TITLE), true);
		}


		/// <summary>
		/// Returns the Component Configuration Description. 
		/// </summary>
		/// <returns>the Component Configuration Description.</returns>
		private string GetComponentConfigurationDescription()
		{
			int[] components =
				_directory.GetIntArray(ExifDirectory.TAG_COMPONENTS_CONFIGURATION);
			string[] componentStrings = { "", "Y", "Cb", "Cr", "R", "G", "B" };
			StringBuilder componentConfig = new StringBuilder();
			for (int i = 0; i < Math.Min(4, components.Length); i++) 
			{
				int j = components[i];
				if (j > 0 && j < componentStrings.Length) 
				{
					componentConfig.Append(componentStrings[j]);
				}
			}
			return componentConfig.ToString();
		}

		/// <summary>
		/// Takes a series of 4 bytes from the specified offSet, and converts these to a 
		/// well-known version number, where possible.  For example, (hex) 30 32 31 30 == 2.10).
		/// </summary>
		/// <param name="components">the four version values</param>
		/// <returns>the version as a string of form 2.10</returns>
		public static string ConvertBytesToVersionString(int[] components) 
		{
			StringBuilder version = new StringBuilder();
			for (int i = 0; i < 4 && i < components.Length; i++) 
			{
				if (i == 2)
					version.Append('.');
				string digit = ((char) components[i]).ToString();
				if (i == 0 && "0".Equals(digit))
					continue;
				version.Append(digit);
			}
			return version.ToString();
		}
	}
}