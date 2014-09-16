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
	/// The Exif Directory class
	/// </summary>
	public class ExifDirectory : Directory 
	{
		// TODO do these tags belong in the exif directory?
		public const int TAG_SUB_IFDS = 0x014A;
		public const int TAG_GPS_INFO = 0x8825;

		/// <summary>
		/// The actual aperture value of lens when the image was taken. Unit is APEX. 
		/// To convert this value to ordinary F-number (F-stop), calculate this value's power 
		/// of root 2 (=1.4142). For example, if the ApertureValue is '5', F-number is 1.4142^5 = F5.6.
		/// </summary>
		public const int TAG_APERTURE = 0x9202;

		/// <summary>
		/// When image format is no compression, this value shows the number of bits 
		/// per component for each pixel. Usually this value is '8,8,8'.
		/// </summary>
		public const int TAG_BITS_PER_SAMPLE = 0x0102;

		/// <summary>
		/// Shows compression method. '1' means no compression, '6' means JPEG compression.
		/// </summary>
		public const int TAG_COMPRESSION = 0x0103;

		/// <summary>
		/// Shows the color space of the image data components. '1' means monochrome, 
		/// '2' means RGB, '6' means YCbCr.
		/// </summary>
		public const int TAG_PHOTOMETRIC_INTERPRETATION = 0x0106;
		public const int TAG_STRIP_OFFSETS = 0x0111;
		public const int TAG_SAMPLES_PER_PIXEL = 0x0115;
		public const int TAG_ROWS_PER_STRIP = 0x116;
		public const int TAG_STRIP_BYTE_COUNTS = 0x0117;

		/// <summary>
		/// When image format is no compression YCbCr, this value shows byte aligns of YCbCr data. 
		/// If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling pixel. 
		/// If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr plane format.
		/// </summary>
		public const int TAG_PLANAR_CONFIGURATION = 0x011C;
		public const int TAG_YCBCR_SUBSAMPLING = 0x0212;
		public const int TAG_IMAGE_DESCRIPTION = 0x010E;
		public const int TAG_SOFTWARE = 0x0131;
		public const int TAG_DATETIME = 0x0132;
		public const int TAG_WHITE_POINT = 0x013E;
		public const int TAG_PRIMARY_CHROMATICITIES = 0x013F;
		public const int TAG_YCBCR_COEFFICIENTS = 0x0211;
		public const int TAG_REFERENCE_BLACK_WHITE = 0x0214;
		public const int TAG_COPYRIGHT = 0x8298;
		public const int TAG_NEW_SUBFILE_TYPE = 0x00FE;
		public const int TAG_SUBFILE_TYPE = 0x00FF;
		public const int TAG_TRANSFER_FUNCTION = 0x012D;
		public const int TAG_ARTIST = 0x013B;
		public const int TAG_PREDICTOR = 0x013D;
		public const int TAG_TILE_WIDTH = 0x0142;
		public const int TAG_TILE_LENGTH = 0x0143;
		public const int TAG_TILE_OFFSETS = 0x0144;
		public const int TAG_TILE_BYTE_COUNTS = 0x0145;
		public const int TAG_JPEG_TABLES = 0x015B;
		public const int TAG_CFA_REPEAT_PATTERN_DIM = 0x828D;

		/// <summary>
		/// There are two definitions for CFA pattern, I don't know the difference...
		/// </summary>
		public const int TAG_CFA_PATTERN_2 = 0x828E;
		public const int TAG_BATTERY_LEVEL = 0x828F;
		public const int TAG_IPTC_NAA = 0x83BB;
		public const int TAG_INTER_COLOR_PROFILE = 0x8773;
		public const int TAG_SPECTRAL_SENSITIVITY = 0x8824;
		public const int TAG_OECF = 0x8828;
		public const int TAG_INTERLACE = 0x8829;
		public const int TAG_TIME_ZONE_OFFSET = 0x882A;
		public const int TAG_SELF_TIMER_MODE = 0x882B;
		public const int TAG_FLASH_ENERGY = 0x920B;
		public const int TAG_SPATIAL_FREQ_RESPONSE = 0x920C;
		public const int TAG_NOISE = 0x920D;
		public const int TAG_IMAGE_NUMBER = 0x9211;
		public const int TAG_SECURITY_CLASSIFICATION = 0x9212;
		public const int TAG_IMAGE_HISTORY = 0x9213;
		public const int TAG_SUBJECT_LOCATION = 0x9214;

		/// <summary>
		/// There are two definitions for exposure index, I don't know the difference...
		/// </summary>
		public const int TAG_EXPOSURE_INDEX_2 = 0x9215;
		public const int TAG_TIFF_EP_STANDARD_ID = 0x9216;
		public const int TAG_FLASH_ENERGY_2 = 0xA20B;
		public const int TAG_SPATIAL_FREQ_RESPONSE_2 = 0xA20C;
		public const int TAG_SUBJECT_LOCATION_2 = 0xA214;
		public const int TAG_MAKE = 0x010F;
		public const int TAG_MODEL = 0x0110;
		public const int TAG_ORIENTATION = 0x0112;
		public const int TAG_X_RESOLUTION = 0x011A;
		public const int TAG_Y_RESOLUTION = 0x011B;
		public const int TAG_RESOLUTION_UNIT = 0x0128;
		public const int TAG_THUMBNAIL_OFFSET = 0x0201;
		public const int TAG_THUMBNAIL_LENGTH = 0x0202;
		public const int TAG_YCBCR_POSITIONING = 0x0213;

		/// <summary>
		/// Exposure time (reciprocal of shutter speed). Unit is second. 
		/// </summary>
		public const int TAG_EXPOSURE_TIME = 0x829A;

		/// <summary>
		/// The actual F-number(F-stop) of lens when the image was taken.
		/// </summary>
		public const int TAG_FNUMBER = 0x829D;

		/// <summary>
		/// Exposure program that the camera used when image was taken. 
		/// '1' means manual control, '2' program normal, '3' aperture priority, '4' 
		/// shutter priority, '5' program creative (slow program), 
		/// '6' program action (high-speed program), '7' portrait mode, '8' landscape mode. 
		/// </summary>
		public const int TAG_EXPOSURE_PROGRAM = 0x8822;
		public const int TAG_ISO_EQUIVALENT = 0x8827;
		public const int TAG_EXIF_VERSION = 0x9000;
		public const int TAG_DATETIME_ORIGINAL = 0x9003;
		public const int TAG_DATETIME_DIGITIZED = 0x9004;
		public const int TAG_COMPONENTS_CONFIGURATION = 0x9101;

		/// <summary>
		/// Average (rough estimate) compression level in JPEG bits per pixel. 
		/// </summary>
		public const int TAG_COMPRESSION_LEVEL = 0x9102;

		/// <summary>
		/// Shutter speed by APEX value. To convert this value to ordinary 'Shutter Speed'; 
		/// calculate this value's power of 2, then reciprocal. For example, if the 
		/// ShutterSpeedValue is '4', shutter speed is 1/(24)=1/16 second.
		/// </summary>
		public const int TAG_SHUTTER_SPEED = 0x9201;
		public const int TAG_BRIGHTNESS_VALUE = 0x9203;
		public const int TAG_EXPOSURE_BIAS = 0x9204;

		/// <summary>
		/// Maximum aperture value of lens. You can convert to F-number by calculating 
		/// power of root 2 (same process of ApertureValue:0x9202).
		/// </summary>
		public const int TAG_MAX_APERTURE = 0x9205;
		public const int TAG_SUBJECT_DISTANCE = 0x9206;

		/// <summary>
		/// Exposure metering method. '0' means unknown, '1' average, '2' center 
		/// weighted average, '3' spot, '4' multi-spot, '5' multi-segment, '6' partial, '255' other.
		/// </summary>
		public const int TAG_METERING_MODE = 0x9207;

		public const int TAG_LIGHT_SOURCE = 0x9208;

		/// <summary>
		/// White balance (aka light source). '0' means unknown, '1' daylight, 
		/// '2' fluorescent, '3' tungsten, '10' flash, '17' standard light A, 
		/// '18' standard light B, '19' standard light C, '20' D55, '21' D65, 
		/// '22' D75, '255' other.
		/// </summary>
		public const int TAG_WHITE_BALANCE = 0xA403;

		/// <summary>
		/// '0' means flash did not fire, '1' flash fired, '5' flash fired but strobe 
		/// return light not detected, '7' flash fired and strobe return light detected.
		/// </summary>
		public const int TAG_FLASH = 0x9209;

		/// <summary>
		/// Focal length of lens used to take image. Unit is millimeter.
		/// </summary>
		public const int TAG_FOCAL_LENGTH = 0x920A;
		public const int TAG_USER_COMMENT = 0x9286;
		public const int TAG_SUBSECOND_TIME = 0x9290;
		public const int TAG_SUBSECOND_TIME_ORIGINAL = 0x9291;
		public const int TAG_SUBSECOND_TIME_DIGITIZED = 0x9292;
		public const int TAG_FLASHPIX_VERSION = 0xA000;

		/// <summary>
		/// Defines Color Space. DCF image must use sRGB color space so value is always '1'. 
		/// If the picture uses the other color space, value is '65535':Uncalibrated.
		/// </summary>
		public const int TAG_COLOR_SPACE = 0xA001;
		public const int TAG_EXIF_IMAGE_WIDTH = 0xA002;
		public const int TAG_EXIF_IMAGE_HEIGHT = 0xA003;
		public const int TAG_RELATED_SOUND_FILE = 0xA004;
		public const int TAG_FOCAL_PLANE_X_RES = 0xA20E;
		public const int TAG_FOCAL_PLANE_Y_RES = 0xA20F;

		/// <summary>
		/// Unit of FocalPlaneXResoluton/FocalPlaneYResolution. 
		/// '1' means no-unit, '2' inch, '3' centimeter.
		/// 
		/// Note: Some of Fujifilm's digicam(e.g.FX2700,FX2900,Finepix4700Z/40i etc) 
		/// uses value '3' so it must be 'centimeter', but it seems that they use a '8.3mm?'
		/// (1/3in.?) to their ResolutionUnit. Fuji's BUG? Finepix4900Z has been changed to 
		/// use value '2' but it doesn't match to actual value also.
		/// </summary>
		public const int TAG_FOCAL_PLANE_UNIT = 0xA210;
		public const int TAG_EXPOSURE_INDEX = 0xA215;
		public const int TAG_SENSING_METHOD = 0xA217;
		public const int TAG_FILE_SOURCE = 0xA300;
		public const int TAG_SCENE_TYPE = 0xA301;
		public const int TAG_CFA_PATTERN = 0xA302;

		public const int TAG_THUMBNAIL_IMAGE_WIDTH = 0x0100;
		public const int TAG_THUMBNAIL_IMAGE_HEIGHT = 0x0101;
		public const int TAG_THUMBNAIL_DATA = 0xF001;

		// are these two exif values?
		public const int TAG_FILL_ORDER = 0x010A;
		public const int TAG_DOCUMENT_NAME = 0x010D;

		public const int TAG_RELATED_IMAGE_FILE_FORMAT = 0x1000;
		public const int TAG_RELATED_IMAGE_WIDTH = 0x1001;
		public const int TAG_RELATED_IMAGE_LENGTH = 0x1002;
		public const int TAG_TRANSFER_RANGE=0x0156;
		public const int TAG_JPEG_PROC=0x0200;
		public const int TAG_EXIF_OFFSET=0x8769;
		public const int TAG_MARKER_NOTE=0x927C;
		public const int TAG_INTEROPERABILITY_OFFSET=0xA005;

		// Windows Attributes added/found by Ryan Patridge 
		public const int TAG_XP_TITLE = 0x9C9B;
		public const int TAG_XP_COMMENTS = 0x9C9C;
		public const int TAG_XP_AUTHOR = 0x9C9D;
		public const int TAG_XP_KEYWORDS = 0x9C9E;
		public const int TAG_XP_SUBJECT = 0x9C9F;

		// Added from Peter Hiemenz idea
		public const int TAG_CUSTOM_RENDERED = 0xA401; // Custom image  processing
		public const int TAG_EXPOSURE_MODE = 0xA402;
		public const int TAG_DIGITAL_ZOOM_RATIO = 0xA404;
		public const int TAG_FOCAL_LENGTH_IN_35MM_FILM = 0xA405;
		public const int TAG_SCENE_CAPTURE_TYPE = 0xA406;
		public const int TAG_GAIN_CONTROL = 0xA407;
		public const int TAG_CONTRAST = 0xA408;
		public const int TAG_SATURATION = 0xA409;
		public const int TAG_SHARPNESS = 0xA40A;
		public const int TAG_DEVICE_SETTING_DESCRIPTION = 0xA40B;
		public const int TAG_SUBJECT_DISTANCE_RANGE = 0xA40C;
		public const int TAG_IMAGE_UNIQUE_ID = 0xA420;


		protected static readonly ResourceBundle BUNDLE = new ResourceBundle("ExifMarkernote");
		protected static readonly IDictionary tagNameMap = ExifDirectory.InitTagMap();

		/// <summary>
		/// Initialize the tag map.
		/// </summary>
		/// <returns>the tag map</returns>
		private static IDictionary InitTagMap() 
		{
			IDictionary resu = new Hashtable();

			resu.Add(TAG_RELATED_IMAGE_FILE_FORMAT, BUNDLE["TAG_RELATED_IMAGE_FILE_FORMAT"]);
			resu.Add(TAG_RELATED_IMAGE_WIDTH, BUNDLE["TAG_RELATED_IMAGE_WIDTH"]);
			resu.Add(TAG_RELATED_IMAGE_LENGTH, BUNDLE["TAG_RELATED_IMAGE_LENGTH"]);
			resu.Add(TAG_TRANSFER_RANGE, BUNDLE["TAG_TRANSFER_RANGE"]);
			resu.Add(TAG_JPEG_PROC, BUNDLE["TAG_JPEG_PROC"]);
			resu.Add(TAG_EXIF_OFFSET, BUNDLE["TAG_EXIF_OFFSET"]);
			resu.Add(TAG_MARKER_NOTE, BUNDLE["TAG_MARKER_NOTE"]);
			resu.Add(TAG_INTEROPERABILITY_OFFSET, BUNDLE["TAG_INTEROPERABILITY_OFFSET"]);
			resu.Add(TAG_FILL_ORDER, BUNDLE["TAG_FILL_ORDER"]);
			resu.Add(TAG_DOCUMENT_NAME, BUNDLE["TAG_DOCUMENT_NAME"]);
			resu.Add(TAG_COMPRESSION_LEVEL,	BUNDLE["TAG_COMPRESSION_LEVEL"]);
			resu.Add(TAG_NEW_SUBFILE_TYPE, BUNDLE["TAG_NEW_SUBFILE_TYPE"]);
			resu.Add(TAG_SUBFILE_TYPE, BUNDLE["TAG_SUBFILE_TYPE"]);
			resu.Add(TAG_THUMBNAIL_IMAGE_WIDTH,	BUNDLE["TAG_THUMBNAIL_IMAGE_WIDTH"]);
			resu.Add(TAG_THUMBNAIL_IMAGE_HEIGHT, BUNDLE["TAG_THUMBNAIL_IMAGE_HEIGHT"]);
			resu.Add(TAG_BITS_PER_SAMPLE, BUNDLE["TAG_BITS_PER_SAMPLE"]);
			resu.Add(TAG_COMPRESSION, BUNDLE["TAG_COMPRESSION"]);
			resu.Add(TAG_PHOTOMETRIC_INTERPRETATION, BUNDLE["TAG_PHOTOMETRIC_INTERPRETATION"]);
			resu.Add(TAG_IMAGE_DESCRIPTION, BUNDLE["TAG_IMAGE_DESCRIPTION"]);
			resu.Add(TAG_MAKE, BUNDLE["TAG_MAKE"]);
			resu.Add(TAG_MODEL, BUNDLE["TAG_MODEL"]);
			resu.Add(TAG_STRIP_OFFSETS, BUNDLE["TAG_STRIP_OFFSETS"]);
			resu.Add(TAG_ORIENTATION, BUNDLE["TAG_ORIENTATION"]);
			resu.Add(TAG_SAMPLES_PER_PIXEL, BUNDLE["TAG_SAMPLES_PER_PIXEL"]);
			resu.Add(TAG_ROWS_PER_STRIP, BUNDLE["TAG_ROWS_PER_STRIP"]);
			resu.Add(TAG_STRIP_BYTE_COUNTS, BUNDLE["TAG_STRIP_BYTE_COUNTS"]);
			resu.Add(TAG_X_RESOLUTION, BUNDLE["TAG_X_RESOLUTION"]);
			resu.Add(TAG_Y_RESOLUTION, BUNDLE["TAG_Y_RESOLUTION"]);
			resu.Add(TAG_PLANAR_CONFIGURATION, BUNDLE["TAG_PLANAR_CONFIGURATION"]);
			resu.Add(TAG_RESOLUTION_UNIT, BUNDLE["TAG_RESOLUTION_UNIT"]);
			resu.Add(TAG_TRANSFER_FUNCTION, BUNDLE["TAG_TRANSFER_FUNCTION"]);
			resu.Add(TAG_SOFTWARE, BUNDLE["TAG_SOFTWARE"]);
			resu.Add(TAG_DATETIME, BUNDLE["TAG_DATETIME"]);
			resu.Add(TAG_ARTIST, BUNDLE["TAG_ARTIST"]);
			resu.Add(TAG_PREDICTOR, BUNDLE["TAG_PREDICTOR"]);
			resu.Add(TAG_WHITE_POINT, BUNDLE["TAG_WHITE_POINT"]);
			resu.Add(TAG_PRIMARY_CHROMATICITIES, BUNDLE["TAG_PRIMARY_CHROMATICITIES"]);
			resu.Add(TAG_TILE_WIDTH, BUNDLE["TAG_TILE_WIDTH"]);
			resu.Add(TAG_TILE_LENGTH, BUNDLE["TAG_TILE_LENGTH"]);
			resu.Add(TAG_TILE_OFFSETS, BUNDLE["TAG_TILE_OFFSETS"]);
			resu.Add(TAG_TILE_BYTE_COUNTS, BUNDLE["TAG_TILE_BYTE_COUNTS"]);
			resu.Add(TAG_SUB_IFDS, BUNDLE["TAG_SUB_IFDS"]);
			resu.Add(TAG_JPEG_TABLES, BUNDLE["TAG_JPEG_TABLES"]);
			resu.Add(TAG_THUMBNAIL_OFFSET, BUNDLE["TAG_THUMBNAIL_OFFSET"]);
			resu.Add(TAG_THUMBNAIL_LENGTH, BUNDLE["TAG_THUMBNAIL_LENGTH"]);
			resu.Add(TAG_THUMBNAIL_DATA, BUNDLE["TAG_THUMBNAIL_DATA"]);
			resu.Add(TAG_YCBCR_COEFFICIENTS, BUNDLE["TAG_YCBCR_COEFFICIENTS"]);
			resu.Add(TAG_YCBCR_SUBSAMPLING,	BUNDLE["TAG_YCBCR_SUBSAMPLING"]);
			resu.Add(TAG_YCBCR_POSITIONING, BUNDLE["TAG_YCBCR_POSITIONING"]);
			resu.Add(TAG_REFERENCE_BLACK_WHITE,	BUNDLE["TAG_REFERENCE_BLACK_WHITE"]);
			resu.Add(TAG_CFA_REPEAT_PATTERN_DIM, BUNDLE["TAG_CFA_REPEAT_PATTERN_DIM"]);
			resu.Add(TAG_CFA_PATTERN_2, BUNDLE["TAG_CFA_PATTERN_2"]);
			resu.Add(TAG_BATTERY_LEVEL, BUNDLE["TAG_BATTERY_LEVEL"]);
			resu.Add(TAG_COPYRIGHT, BUNDLE["TAG_COPYRIGHT"]);
			resu.Add(TAG_EXPOSURE_TIME, BUNDLE["TAG_EXPOSURE_TIME"]);
			resu.Add(TAG_FNUMBER, BUNDLE["TAG_FNUMBER"]);
			resu.Add(TAG_IPTC_NAA, BUNDLE["TAG_IPTC_NAA"]);
			resu.Add(TAG_INTER_COLOR_PROFILE, BUNDLE["TAG_INTER_COLOR_PROFILE"]);
			resu.Add(TAG_EXPOSURE_PROGRAM, BUNDLE["TAG_EXPOSURE_PROGRAM"]);
			resu.Add(TAG_SPECTRAL_SENSITIVITY, BUNDLE["TAG_SPECTRAL_SENSITIVITY"]);
			resu.Add(TAG_GPS_INFO, BUNDLE["TAG_GPS_INFO"]);
			resu.Add(TAG_ISO_EQUIVALENT, BUNDLE["TAG_ISO_EQUIVALENT"]);
			resu.Add(TAG_OECF, BUNDLE["TAG_OECF"]);
			resu.Add(TAG_INTERLACE, BUNDLE["TAG_INTERLACE"]);
			resu.Add(TAG_TIME_ZONE_OFFSET, BUNDLE["TAG_TIME_ZONE_OFFSET"]);
			resu.Add(TAG_SELF_TIMER_MODE, BUNDLE["TAG_SELF_TIMER_MODE"]);
			resu.Add(TAG_EXIF_VERSION, BUNDLE["TAG_EXIF_VERSION"]);
			resu.Add(TAG_DATETIME_ORIGINAL,	BUNDLE["TAG_DATETIME_ORIGINAL"]);
			resu.Add(TAG_DATETIME_DIGITIZED, BUNDLE["TAG_DATETIME_DIGITIZED"]);
			resu.Add(TAG_COMPONENTS_CONFIGURATION, BUNDLE["TAG_COMPONENTS_CONFIGURATION"]);
			resu.Add(TAG_SHUTTER_SPEED, BUNDLE["TAG_SHUTTER_SPEED"]);
			resu.Add(TAG_APERTURE, BUNDLE["TAG_APERTURE"]);
			resu.Add(TAG_BRIGHTNESS_VALUE, BUNDLE["TAG_BRIGHTNESS_VALUE"]);
			resu.Add(TAG_EXPOSURE_BIAS, BUNDLE["TAG_EXPOSURE_BIAS"]);
			resu.Add(TAG_MAX_APERTURE, BUNDLE["TAG_MAX_APERTURE"]);
			resu.Add(TAG_SUBJECT_DISTANCE, BUNDLE["TAG_SUBJECT_DISTANCE"]);
			resu.Add(TAG_METERING_MODE, BUNDLE["TAG_METERING_MODE"]);
			resu.Add(TAG_WHITE_BALANCE, BUNDLE["TAG_WHITE_BALANCE"]);
			resu.Add(TAG_FLASH, BUNDLE["TAG_FLASH"]);
			resu.Add(TAG_FOCAL_LENGTH, BUNDLE["TAG_FOCAL_LENGTH"]);
			resu.Add(TAG_FLASH_ENERGY, BUNDLE["TAG_FLASH_ENERGY"]);
			resu.Add(TAG_SPATIAL_FREQ_RESPONSE, BUNDLE["TAG_SPATIAL_FREQ_RESPONSE"]);
			resu.Add(TAG_NOISE, BUNDLE["TAG_NOISE"]);
			resu.Add(TAG_IMAGE_NUMBER, BUNDLE["TAG_IMAGE_NUMBER"]);
			resu.Add(TAG_SECURITY_CLASSIFICATION, BUNDLE["TAG_SECURITY_CLASSIFICATION"]);
			resu.Add(TAG_IMAGE_HISTORY, BUNDLE["TAG_IMAGE_HISTORY"]);
			resu.Add(TAG_SUBJECT_LOCATION, BUNDLE["TAG_SUBJECT_LOCATION"]);
			resu.Add(TAG_EXPOSURE_INDEX, BUNDLE["TAG_EXPOSURE_INDEX"]);
			resu.Add(TAG_TIFF_EP_STANDARD_ID, BUNDLE["TAG_TIFF_EP_STANDARD_ID"]);
			resu.Add(TAG_USER_COMMENT, BUNDLE["TAG_USER_COMMENT"]);
			resu.Add(TAG_SUBSECOND_TIME, BUNDLE["TAG_SUBSECOND_TIME"]);
			resu.Add(TAG_SUBSECOND_TIME_ORIGINAL, BUNDLE["TAG_SUBSECOND_TIME_ORIGINAL"]);
			resu.Add(TAG_SUBSECOND_TIME_DIGITIZED, BUNDLE["TAG_SUBSECOND_TIME_DIGITIZED"]);
			resu.Add(TAG_FLASHPIX_VERSION, BUNDLE["TAG_FLASHPIX_VERSION"]);
			resu.Add(TAG_COLOR_SPACE, BUNDLE["TAG_COLOR_SPACE"]);
			resu.Add(TAG_EXIF_IMAGE_WIDTH, BUNDLE["TAG_EXIF_IMAGE_WIDTH"]);
			resu.Add(TAG_EXIF_IMAGE_HEIGHT, BUNDLE["TAG_EXIF_IMAGE_HEIGHT"]);
			resu.Add(TAG_RELATED_SOUND_FILE, BUNDLE["TAG_RELATED_SOUND_FILE"]);
			// 0x920B in TIFF/EP
			resu.Add(TAG_FLASH_ENERGY_2, BUNDLE["TAG_FLASH_ENERGY_2"]);
			// 0x920C in TIFF/EP
			resu.Add(TAG_SPATIAL_FREQ_RESPONSE_2, BUNDLE["TAG_SPATIAL_FREQ_RESPONSE_2"]);
			// 0x920E in TIFF/EP
			resu.Add(TAG_FOCAL_PLANE_X_RES, BUNDLE["TAG_FOCAL_PLANE_X_RES"]);
			// 0x920F in TIFF/EP
			resu.Add(TAG_FOCAL_PLANE_Y_RES,	BUNDLE["TAG_FOCAL_PLANE_Y_RES"]);
			// 0x9210 in TIFF/EP
			resu.Add(TAG_FOCAL_PLANE_UNIT, BUNDLE["TAG_FOCAL_PLANE_UNIT"]);
			// 0x9214 in TIFF/EP
			resu.Add(TAG_SUBJECT_LOCATION_2, BUNDLE["TAG_SUBJECT_LOCATION_2"]);
			// 0x9215 in TIFF/EP
			resu.Add(TAG_EXPOSURE_INDEX_2, BUNDLE["TAG_EXPOSURE_INDEX_2"]);
			// 0x9217 in TIFF/EP
			resu.Add(TAG_SENSING_METHOD, BUNDLE["TAG_SENSING_METHOD"]);
			resu.Add(TAG_FILE_SOURCE, BUNDLE["TAG_FILE_SOURCE"]);
			resu.Add(TAG_SCENE_TYPE, BUNDLE["TAG_SCENE_TYPE"]);
			resu.Add(TAG_CFA_PATTERN, BUNDLE["TAG_CFA_PATTERN"]);

			// Windows Attributes added/found by Ryan Patridge 
			resu.Add(TAG_XP_TITLE, BUNDLE["TAG_XP_TITLE"]);
			resu.Add(TAG_XP_COMMENTS, BUNDLE["TAG_XP_COMMENTS"]);
			resu.Add(TAG_XP_AUTHOR, BUNDLE["TAG_XP_AUTHOR"]);
			resu.Add(TAG_XP_KEYWORDS, BUNDLE["TAG_XP_KEYWORDS"]);
			resu.Add(TAG_XP_SUBJECT, BUNDLE["TAG_XP_SUBJECT"]);

			// Added from Peter Hiemenz idea
			resu.Add(TAG_CUSTOM_RENDERED, BUNDLE["TAG_CUSTOM_RENDERED"]);
			resu.Add(TAG_EXPOSURE_MODE, BUNDLE["TAG_EXPOSURE_MODE"]);
			resu.Add(TAG_DIGITAL_ZOOM_RATIO, BUNDLE["TAG_DIGITAL_ZOOM_RATIO"]);
			resu.Add(TAG_FOCAL_LENGTH_IN_35MM_FILM, BUNDLE["TAG_FOCAL_LENGTH_IN_35MM_FILM"]);
			resu.Add(TAG_SCENE_CAPTURE_TYPE, BUNDLE["TAG_SCENE_CAPTURE_TYPE"]);
			resu.Add(TAG_GAIN_CONTROL, BUNDLE["TAG_GAIN_CONTROL"]);
			resu.Add(TAG_CONTRAST, BUNDLE["TAG_CONTRAST"]);
			resu.Add(TAG_SATURATION, BUNDLE["TAG_SATURATION"]);
			resu.Add(TAG_SHARPNESS, BUNDLE["TAG_SHARPNESS"]);
			resu.Add(TAG_DEVICE_SETTING_DESCRIPTION, BUNDLE["TAG_DEVICE_SETTING_DESCRIPTION"]);
			resu.Add(TAG_SUBJECT_DISTANCE_RANGE, BUNDLE["TAG_SUBJECT_DISTANCE_RANGE"]);
			resu.Add(TAG_IMAGE_UNIQUE_ID, BUNDLE["TAG_IMAGE_UNIQUE_ID"]);
			return resu;
		}

		/// <summary>
		/// Constructor of the object.
		/// </summary>
		public ExifDirectory() : base()
		{
			this.SetDescriptor(new ExifDescriptor(this));
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
		/// Gets the thumbnail data.
		/// </summary>
		/// <returns>the thumbnail data or null if none</returns>
		public byte[] GetThumbnailData() 
		{
			if (!ContainsThumbnail())
				return null;

			return this.GetByteArray(ExifDirectory.TAG_THUMBNAIL_DATA);
		}

		/// <summary>
		/// Writes the thumbnail in the given file
		/// </summary>
		/// <param name="filename">where to write the thumbnail</param>
		/// <exception cref="MetadataException">if there is not data in thumbnail</exception>
		public void WriteThumbnail(string filename) 
		{
			byte[] data = GetThumbnailData();

			if (data == null) 
			{
				throw new MetadataException("No thumbnail data exists.");
			}

			FileStream stream = null;
			try 
			{
				stream = new FileStream(filename, FileMode.CreateNew);
				stream.Write(data, 0, data.Length);
			} 
			finally 
			{
				if (stream != null)
					stream.Close();
			}
		}

		/// <summary>
		/// Indicates if there is thumbnail data or not
		/// </summary>
		/// <returns>true if there is thumbnail data, false if not</returns>
		public bool ContainsThumbnail() 
		{
			return ContainsTag(ExifDirectory.TAG_THUMBNAIL_DATA);
		}
	}
}