using System;
using System.Collections;
using System.Text;
using System.IO;
using com.drew.metadata;
using com.drew.imaging.jpg;
using com.drew.lang;
using com.utils;

/// <summary>
/// This class based upon code from Jhead, a C program for extracting and 
/// manipulating the Exif data within files written by Matthias Wandel. 
/// http://www.sentex.net/~mwandel/jhead/
///
/// Jhead is public domain software - that is, you can do whatever 
/// you want with it, and include it software that is licensed under 
/// the GNU or the BSD license, or whatever other licence you choose, 
/// including proprietary closed source licenses.  Similarly, I release 
/// this Java version under the same license, though I do ask that you 
/// leave this header in tact.
///
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
/// Created on 28 April 2002, 23:54
/// Modified 04 Aug 2002
/// - Renamed constants to be inline with changes to ExifTagValues interface
/// - Substituted usage of JDK 1.4 features (java.nio package)
/// Modified 29 Oct 2002 (v1.2)
/// - Proper traversing of Exif file structure and complete refactor & tidy of 
///   the codebase (a few unnoticed bugs removed)
/// - Reads makernote data for 6 families of camera (5 makes)
/// - Tags now stored in directories... use the IFD_* constants to refer to the 
///   image file directory you require (Exif, Interop, GPS and Makernote*) 
///   -- this avoids collisions where two tags share the same code
/// - Takes componentCount of unknown tags into account
/// - Now understands GPS tags (thanks to Colin Briton for his help with this)
/// - Some other bug fixes, pointed out by users around the world.  Thanks!
/// Modified 27 Nov 2002 (v2.0)
/// - Renamed to ExifReader
/// - Moved to new package com.drew.metadata.exif
/// 
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.drew.metadata.exif
{
	/// <summary>
	/// Extracts Exif data from a JPEG header segment, providing information about 
	/// the camera/scanner/capture device (if available).  
	/// Information is encapsulated in an Metadata object.
	/// </summary>
	public class ExifReader : MetadataReader 
	{
		/// <summary>
		/// The JPEG segment as an array of bytes.
		/// </summary>
		private byte[] _data;

		/// <summary>
		/// Represents the native byte ordering used in the JPEG segment.
		/// If true, then we're using Motorolla ordering (Big endian), else 
		/// we're using Intel ordering (Little endian).
		/// </summary>
		private bool _isMotorollaByteOrder;

		/// <summary>
		/// Bean instance to store information about the image and camera/scanner/capture device.
		/// </summary>
		private Metadata _metadata;

		/// <summary>
		/// The number of bytes used per format descriptor.
		/// </summary>
		private static readonly int[] BYTES_PER_FORMAT = { 0, 1, 1, 2, 4, 8, 1, 1, 2, 4, 8, 4, 8 };

		/// <summary>
		/// The number of formats known.
		/// </summary>
		private static readonly int MAX_FORMAT_CODE = 12;

		// the format enumeration
		// TODO use the new DataFormat enumeration instead of these values
		private static readonly int FMT_BYTE = 1;
		private static readonly int FMT_STRING = 2;
		private static readonly int FMT_USHORT = 3;
		private static readonly int FMT_ULONG = 4;
		private static readonly int FMT_URATIONAL = 5;
		private static readonly int FMT_SBYTE = 6;
		private static readonly int FMT_UNDEFINED = 7;
		private static readonly int FMT_SSHORT = 8;
		private static readonly int FMT_SLONG = 9;
		private static readonly int FMT_SRATIONAL = 10;
		private static readonly int FMT_SINGLE = 11;
		private static readonly int FMT_DOUBLE = 12;

		public const int TAG_EXIF_OFFSET = 0x8769;
		public const int TAG_INTEROP_OFFSET = 0xA005;
		public const int TAG_GPS_INFO_OFFSET = 0x8825;
		public const int TAG_MAKER_NOTE = 0x927C;

		// NOT READONLY
		public static int TIFF_HEADER_START_OFFSET = 6;

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="file">the file to read</param>
		public ExifReader(FileInfo file) : this(
			new JpegSegmentReader(file).ReadSegment(
			JpegSegmentReader.SEGMENT_APP1))
		{	
		}

		/**
		 * Creates an ExifReader for the given JPEG header segment.
		 */

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="data">the data</param>
		public ExifReader(byte[] data) 
		{
			_data = data;
		}

		/// <summary>
		/// Performs the Exif data extraction, returning a new instance of Metadata. 
		/// </summary>
		/// <returns>a new instance of Metadata</returns>
		public Metadata Extract() 
		{
			return Extract(new Metadata());
		}

		/// <summary>
		/// Performs the Exif data extraction, adding found values to the specified instance of Metadata.
		/// </summary>
		/// <param name="metadata">where to add meta data</param>
		/// <returns>the metadata</returns>
		public Metadata Extract(Metadata metadata) 
		{
			_metadata = metadata;
			if (_data == null) 
			{
				return _metadata;
			}

			// once we know there's some data, create the directory and start working on it
			Directory directory = _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory"));
			if (_data.Length <= 14) 
			{
				directory.AddError("Exif data segment must contain at least 14 bytes");
				return _metadata;
			}
			if (!"Exif\0\0".Equals(Utils.Decode(_data, 0, 6, false))) 
			{
				directory.AddError("Exif data segment doesn't begin with 'Exif'");
				return _metadata;
			}

			// this should be either "MM" or "II"
			string byteOrderIdentifier = Utils.Decode(_data, 6, 2, false);
			if (!SetByteOrder(byteOrderIdentifier)) 
			{
				directory.AddError("Unclear distinction between Motorola/Intel byte ordering");
				return _metadata;
			}

			// Check the next two values for correctness.
			if (Get16Bits(8) != 0x2a) 
			{
				directory.AddError("Invalid Exif start - should have 0x2A at offSet 8 in Exif header");
				return _metadata;
			}

			int firstDirectoryOffSet = Get32Bits(10) + TIFF_HEADER_START_OFFSET;

			// David Ekholm sent an digital camera image that has this problem
			if (firstDirectoryOffSet >= _data.Length - 1) 
			{
				directory.AddError("First exif directory offSet is beyond end of Exif data segment");
				// First directory normally starts 14 bytes in -- try it here and catch another error in the worst case
				firstDirectoryOffSet = 14;
			}

			// 0th IFD (we merge with Exif IFD)
			ProcessDirectory(directory, firstDirectoryOffSet);

			// after the extraction process, if we have the correct tags, we may be able to extract thumbnail information
			ExtractThumbnail(directory);

			return _metadata;
		}

		/// <summary>
		/// Extract Thumbnail
		/// </summary>
		/// <param name="exifDirectory">where to take the information</param>
		private void ExtractThumbnail(Directory exifDirectory) 
		{
			if (!(exifDirectory is ExifDirectory)) 
			{
				return;
			}

			if (!exifDirectory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_LENGTH)
				|| !exifDirectory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_OFFSET)) 
			{
				return;
			}

			try 
			{
				int offSet =
					exifDirectory.GetInt(ExifDirectory.TAG_THUMBNAIL_OFFSET);
				int Length =
					exifDirectory.GetInt(ExifDirectory.TAG_THUMBNAIL_LENGTH);
				byte[] result = new byte[Length];
				for (int i = 0; i < result.Length; i++) 
				{
					result[i] = _data[TIFF_HEADER_START_OFFSET + offSet + i];
				}
				exifDirectory.SetObject(
					ExifDirectory.TAG_THUMBNAIL_DATA,
					result);
			} 
			catch (Exception e) 
			{
				exifDirectory.AddError("Unable to extract thumbnail: " + e.Message);
			}
		}

		/// <summary>
		/// Sets Motorolla byte order
		/// </summary>
		/// <param name="byteOrderIdentifier">the Motorolla byte order identifier (MM=true, II=false) </param>
		/// <returns></returns>
		private bool SetByteOrder(string byteOrderIdentifier) 
		{
			if ("MM".Equals(byteOrderIdentifier)) 
			{
				_isMotorollaByteOrder = true;
			} 
			else if ("II".Equals(byteOrderIdentifier)) 
			{
				_isMotorollaByteOrder = false;
			} 
			else 
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Process one of the nested Tiff IFD directories.
		/// 2 bytes: number of tags for each tag
		///		2 bytes: tag type	
		///		2 bytes: format code	
		/// 	4 bytes: component count
		/// </summary>
		/// <param name="directory">the directory</param>
		/// <param name="dirStartOffSet">where to start</param>
		private void ProcessDirectory(Directory directory, int dirStartOffSet) 
		{
			if (dirStartOffSet >= _data.Length || dirStartOffSet < 0) 
			{
				directory.AddError("Ignored directory marked to start outside data segement");
				return;
			}

			// First two bytes in the IFD are the tag count
			int dirTagCount = Get16Bits(dirStartOffSet);

			if (!isDirectoryLengthValid(dirStartOffSet)) 
			{
				directory.AddError("Illegally sized directory");
				return;
			}


			// Handle each tag in this directory
			for (int dirEntry = 0; dirEntry < dirTagCount; dirEntry++) 
			{
				int dirEntryOffSet =
					CalculateDirectoryEntryOffSet(dirStartOffSet, dirEntry);
				int tagType = Get16Bits(dirEntryOffSet);
				// Console.WriteLine("TagType="+tagType);
				int formatCode = Get16Bits(dirEntryOffSet + 2);
				if (formatCode < 0 || formatCode > MAX_FORMAT_CODE) 
				{
					directory.AddError("Invalid format code: " + formatCode);
					continue;
				}

				// 4 bytes indicating number of formatCode type data for this tag
				int componentCount = Get32Bits(dirEntryOffSet + 4);
				int byteCount = componentCount * BYTES_PER_FORMAT[formatCode];
				int tagValueOffSet = CalculateTagValueOffSet(byteCount, dirEntryOffSet);
				if (tagValueOffSet < 0) 
				{
					directory.AddError("Illegal pointer offSet aValue in EXIF");
					continue;
				}

				// Calculate the aValue as an offSet for cases where the tag represents directory
				int subdirOffSet =
					TIFF_HEADER_START_OFFSET + Get32Bits(tagValueOffSet);

				if (tagType == TAG_EXIF_OFFSET) 
				{
					// Console.WriteLine("TagType is TAG_EXIF_OFFSET ("+tagType+")");
					ProcessDirectory(
						_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory")),
						subdirOffSet);
					continue;
				} 
				else if (tagType == TAG_INTEROP_OFFSET) 
				{
					// Console.WriteLine("TagType is TAG_INTEROP_OFFSET ("+tagType+")");
					ProcessDirectory(
						_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifInteropDirectory")),
						subdirOffSet);
					continue;
				} 
				else if (tagType == TAG_GPS_INFO_OFFSET) 
				{
					// Console.WriteLine("TagType is TAG_GPS_INFO_OFFSET ("+tagType+")");
					ProcessDirectory(
						_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.GpsDirectory")),
						subdirOffSet);
					continue;
				} 
				else if (tagType ==  TAG_MAKER_NOTE) 
				{
					// Console.WriteLine("TagType is TAG_MAKER_NOTE ("+tagType+")");
					ProcessMakerNote(tagValueOffSet);
					continue;
				} 
				else 
				{
					// Console.WriteLine("TagType is ???? ("+tagType+")");
					ProcessTag(
						directory,
						tagType,
						tagValueOffSet,
						componentCount,
						formatCode);
				}
			}
			// At the end of each IFD is an optional link to the next IFD.  This link is after
			// the 2-byte tag count, and after 12 bytes for each of these tags, hence
			int nextDirectoryOffSet =
				Get32Bits(dirStartOffSet + 2 + 12 * dirTagCount);
			if (nextDirectoryOffSet != 0) 
			{
				nextDirectoryOffSet += TIFF_HEADER_START_OFFSET;
				if (nextDirectoryOffSet >= _data.Length) 
				{
					// Last 4 bytes of IFD reference another IFD with an address that is out of bounds
					// Note this could have been caused by jhead 1.3 cropping too much
					return;
				}
				// the next directory is of same type as this one
				ProcessDirectory(directory, nextDirectoryOffSet);
			}
		}

		/// <summary>
		/// Determine the camera model and makernote format
		/// </summary>
		/// <param name="subdirOffSet">the sub offset dir</param>
		private void ProcessMakerNote(int subdirOffSet) 
		{
			// Console.WriteLine("ProcessMakerNote value="+subdirOffSet);
			// Determine the camera model and makernote format
			Directory exifDirectory = _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory"));
			if (exifDirectory == null) 
			{
				return;
			}

			string cameraModel = exifDirectory.GetString(ExifDirectory.TAG_MAKE);
			if ("OLYMP".Equals(Utils.Decode(_data, subdirOffSet, 5, false))) 
			{
				// Olympus Makernote
				ProcessDirectory(
					_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.OlympusMakernoteDirectory")),
					subdirOffSet + 8);
			} 
			else if (
				cameraModel != null
				&& cameraModel.Trim().ToUpper().StartsWith("NIKON")) 
			{
				if ("Nikon".Equals(Utils.Decode(_data, subdirOffSet, 5, false))) 
				{
					// There are two scenarios here:
					// Type 1:
					// :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
					// :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
					// Type 3:
					// :0000: 4E 69 6B 6F 6E 00 02 00-00 00 4D 4D 00 2A 00 00 Nikon....MM.*...
					// :0010: 00 08 00 1E 00 01 00 07-00 00 00 04 30 32 30 30 ............0200
					if (_data[subdirOffSet + 6] == 1) 
					{
						// Nikon type 1 Makernote
						ProcessDirectory(
							_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType1MakernoteDirectory")),
							subdirOffSet + 8);
					} 
					else if (_data[subdirOffSet + 6] == 2) 
					{
						// Nikon type 3 Makernote
						// TODO at this point we're assuming that the MM ordering is continuous 
						// with the rest of the file
						// (this seems to be the case, but I don't have many sample images)
						// TODO shouldn't be messing around with this static variable (not threadsafe)
						// instead, should pass an additional offSet to the ProcessDirectory method
						int oldHeaderStartOffSet = TIFF_HEADER_START_OFFSET;
						TIFF_HEADER_START_OFFSET = subdirOffSet + 10;
						ProcessDirectory(
							_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType3MakernoteDirectory")),
							subdirOffSet + 18);
						TIFF_HEADER_START_OFFSET = oldHeaderStartOffSet;
					} 
					else 
					{
						exifDirectory.AddError(
							"Unsupported makernote data ignored.");
					}
				} 
				else 
				{
					// Nikon type 2 Makernote
					ProcessDirectory(
						_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType2MakernoteDirectory")),
						subdirOffSet);
				}
			} 
			else if ("Canon".ToUpper().Equals(cameraModel.ToUpper())) 
			{
				// Console.WriteLine("CanonMakernoteDirectory is found");
				// Canon Makernote
				ProcessDirectory(
					_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CanonMakernoteDirectory")),
					subdirOffSet);
			} 
			else if ("Casio".ToUpper().Equals(cameraModel.ToUpper())) 
			{
				// Casio Makernote
				ProcessDirectory(
					_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CasioMakernoteDirectory")),
					subdirOffSet);
			} 
			else if (
				"FUJIFILM".Equals(Utils.Decode(_data, subdirOffSet, 8, false))
				|| "Fujifilm".ToUpper().Equals(cameraModel.ToUpper())) 
			{
				// Fujifile Makernote
				bool byteOrderBefore = _isMotorollaByteOrder;
				// bug in fujifilm makernote ifd means we temporarily use Intel byte ordering
				_isMotorollaByteOrder = false;
				// the 4 bytes after "FUJIFILM" in the makernote point to the start of the makernote
				// IFD, though the offSet is relative to the start of the makernote, not the TIFF
				// header (like everywhere else)
				int ifdStart = subdirOffSet + Get32Bits(subdirOffSet + 8);
				ProcessDirectory(
					_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.FujiFilmMakernoteDirectory")),
					ifdStart);
				_isMotorollaByteOrder = byteOrderBefore;
			} 
			else 
			{
				// TODO how to store makernote data when it's not from a supported camera model?
				exifDirectory.AddError("Unsupported makernote data ignored.");
			}
		}

		/// <summary>
		/// Indicates if Directory Length is valid or not
		/// </summary>
		/// <param name="dirStartOffSet">where to start</param>
		/// <returns>true if Directory Length is valid</returns>
		private bool isDirectoryLengthValid(int dirStartOffSet) 
		{
			int dirTagCount = Get16Bits(dirStartOffSet);
			int dirLength = (2 + (12 * dirTagCount) + 4);
			return !(dirLength + dirStartOffSet + TIFF_HEADER_START_OFFSET >= _data.Length);
		}

		/// <summary>
		/// Processes tag
		/// </summary>
		/// <param name="directory">the directory</param>
		/// <param name="tagType">the tag type</param>
		/// <param name="tagValueOffSet">the offset value</param>
		/// <param name="componentCount">the component count</param>
		/// <param name="formatCode">the format code</param>
		private void ProcessTag(
			Directory directory,
			int tagType,
			int tagValueOffSet,
			int componentCount,
			int formatCode) 
		{
			// Directory simply stores raw values
			// The display side uses a Descriptor class per directory to turn the 
			// raw values into 'pretty' descriptions
			if (formatCode == FMT_UNDEFINED || formatCode == FMT_STRING) 
			{
				string s = null;
				if (tagType == ExifDirectory.TAG_USER_COMMENT) 
				{
					s =
						ReadCommentString(
						tagValueOffSet,
						componentCount,
						formatCode);
				} 
				else 
				{
					s = ReadString(tagValueOffSet, componentCount);
				}
				directory.SetObject(tagType, s);
			} 
			else if (formatCode == FMT_SRATIONAL || formatCode == FMT_URATIONAL) 
			{
				if (componentCount == 1) 
				{
					Rational rational =
						new Rational(
						Get32Bits(tagValueOffSet),
						Get32Bits(tagValueOffSet + 4));
					directory.SetObject(tagType, rational);
				} 
				else 
				{
					Rational[] rationals = new Rational[componentCount];
					for (int i = 0; i < componentCount; i++) 
					{
						rationals[i] =
							new Rational(
							Get32Bits(tagValueOffSet + (8 * i)),
							Get32Bits(tagValueOffSet + 4 + (8 * i)));
					}
					directory.SetObject(tagType, rationals);
				}
			} 
			else if (formatCode == FMT_SBYTE || formatCode == FMT_BYTE) 
			{
				if (componentCount == 1) 
				{
					// this may need to be a byte, but I think casting to int is fine
					int b = _data[tagValueOffSet];
					directory.SetObject(tagType, b);
				} 
				else 
				{
					int[] bytes = new int[componentCount];
					for (int i = 0; i < componentCount; i++) 
					{
						bytes[i] = _data[tagValueOffSet + i];
					}
					directory.SetIntArray(tagType, bytes);
				}
			} 
			else if (formatCode == FMT_SINGLE  || formatCode == FMT_DOUBLE) 
			{
				if (componentCount == 1) 
				{
					int i = _data[tagValueOffSet];
					directory.SetObject(tagType, i);
				} 
				else 
				{
					int[] ints = new int[componentCount];
					for (int i = 0; i < componentCount; i++) 
					{
						ints[i] = _data[tagValueOffSet + i];
					}
					directory.SetIntArray(tagType, ints);
				}
			} 
			else if (formatCode == FMT_USHORT || formatCode == FMT_SSHORT) 
			{
				if (componentCount == 1) 
				{
					int i = Get16Bits(tagValueOffSet);
					directory.SetObject(tagType, i);
				} 
				else 
				{
					int[] ints = new int[componentCount];
					for (int i = 0; i < componentCount; i++) 
					{
						ints[i] = Get16Bits(tagValueOffSet + (i * 2));
					}
					directory.SetIntArray(tagType, ints);
				}
			} 
			else if (formatCode == FMT_SLONG || formatCode == FMT_ULONG) 
			{
				if (componentCount == 1) 
				{
					int i = Get32Bits(tagValueOffSet);
					directory.SetObject(tagType, i);
				} 
				else 
				{
					int[] ints = new int[componentCount];
					for (int i = 0; i < componentCount; i++) 
					{
						ints[i] = Get32Bits(tagValueOffSet + (i * 4));
					}
					directory.SetIntArray(tagType, ints);
				}
			} 
			else  
			{
				directory.AddError("unknown format code " + formatCode);
			}
		}

		/// <summary>
		/// Calculates tag value offset
		/// </summary>
		/// <param name="byteCount">the byte count</param>
		/// <param name="dirEntryOffSet">the dir entry offset</param>
		/// <returns>-1 if error, or the valus offset</returns>
		private int CalculateTagValueOffSet(int byteCount, int dirEntryOffSet) 
		{
			if (byteCount > 4) 
			{
				// If its bigger than 4 bytes, the dir entry contains an offSet.
				// TODO if we're reading FujiFilm makernote tags, the offSet is relative to the start of the makernote itself, not the TIFF segment
				int offSetVal = Get32Bits(dirEntryOffSet + 8);
				if (offSetVal + byteCount > _data.Length) 
				{
					// Bogus pointer offSet and / or bytecount aValue
					return -1; // signal error
				}
				return TIFF_HEADER_START_OFFSET + offSetVal;
			} 
			else 
			{
				// 4 bytes or less and aValue is in the dir entry itself
				return dirEntryOffSet + 8;
			}
		}

		/// <summary>
		/// Creates a string from the _data buffer starting at the specified offSet, 
		/// and ending where byte=='\0' or where Length==maxLength.
		/// </summary>
		/// <param name="offSet">the offset</param>
		/// <param name="maxLength">the max length</param>
		/// <returns>a string representing what was read</returns>
		private string ReadString(int offSet, int maxLength) 
		{
			int Length = 0;
			while ((offSet + Length) < _data.Length
				&& _data[offSet + Length] != '\0'
				&& Length < maxLength) 
			{
				Length++;
			}
			return Utils.Decode(_data, offSet, Length, false);
		}

		/// <summary>
		/// A special case of ReadString that handle Exif UserComment reading.
		/// This method is necessary as certain camere models prefix the comment string 
		/// with "ASCII\0", which is all that would be returned by ReadString(...).
		/// </summary>
		/// <param name="tagValueOffSet">the tag value offset</param>
		/// <param name="componentCount">the component count</param>
		/// <param name="formatCode">the format code</param>
		/// <returns>a string</returns>
		private string ReadCommentString(
			int tagValueOffSet,
			int componentCount,
			int formatCode) 
		{
			// Olympus has this padded with trailing spaces.  Remove these first.
			// ArrayIndexOutOfBoundsException bug fixed by Hendrik Wördehoff - 20 Sep 2002
			int byteCount = componentCount * BYTES_PER_FORMAT[formatCode];
			for (int i = byteCount - 1; i >= 0; i--) 
			{
				if (_data[tagValueOffSet + i] == ' ') 
				{
					_data[tagValueOffSet + i] = (byte) '\0';
				} 
				else 
				{
					break;
				}
			}
			// Copy the comment
			if ("ASCII".Equals(Utils.Decode(_data, tagValueOffSet, 5, false))) 
			{
				for (int i = 5; i < 10; i++) 
				{
					byte b = _data[tagValueOffSet + i];
					if (b != '\0' && b != ' ') 
					{
						return ReadString(tagValueOffSet + i, 1999);
					}
				}
			}
			else if ("UNICODE".Equals(Utils.Decode(_data, tagValueOffSet, 7, false))) 
			{
				int start = tagValueOffSet+7;
				for (int i = start; i < 10+start; i++) 
				{
					byte b = _data[i];
					if (b == 0 || (char)b == ' ') 
					{
						continue;
					}
					else 
					{
						start = i;
						break;
					}
					
				}
				int end = _data.Length;
				// TODO find a way to cut the string properly				
				return Utils.Decode(_data, start, end-start, true);

			}


			// TODO implement support for UNICODE and JIS UserComment encodings..?
			return ReadString(tagValueOffSet, 1999);
		}

		/// <summary>
		/// Determine the offSet at which a given InteropArray entry begins within the specified IFD.
		/// </summary>
		/// <param name="ifdStartOffSet">the offSet at which the IFD starts</param>
		/// <param name="entryNumber">the zero-based entry number</param>
		/// <returns>the directory entry offset</returns>
		private int CalculateDirectoryEntryOffSet(
			int ifdStartOffSet,
			int entryNumber) 
		{
			return (ifdStartOffSet + 2 + (12 * entryNumber));
		}

		/**
		 * 
		 */

		/// <summary>
		/// Gets a 16 bit aValue from file's native byte order.  Between 0x0000 and 0xFFFF.
		/// </summary>
		/// <param name="offSet">the offset</param>
		/// <returns>a 16 bit int</returns>
		private int Get16Bits(int offSet) 
		{
			if (offSet < 0 || offSet >= _data.Length) 
			{
				throw new IndexOutOfRangeException(
					"attempt to read data outside of exif segment (index "
					+ offSet
					+ " where max index is "
					+ (_data.Length - 1)
					+ ")");
			}
			if (_isMotorollaByteOrder) 
			{
				// Motorola big first
				return (_data[offSet] << 8 & 0xFF00) | (_data[offSet + 1] & 0xFF);
			} 
			else 
			{
				// Intel ordering
				return (_data[offSet + 1] << 8 & 0xFF00) | (_data[offSet] & 0xFF);
			}
		}

		/// <summary>
		/// Gets a 32 bit aValue from file's native byte order.
		/// </summary>
		/// <param name="offSet">the offset</param>
		/// <returns>a 32b int</returns>
		private int Get32Bits(int offSet) 
		{
			if (offSet < 0 || offSet >= _data.Length) 
			{
				throw new IndexOutOfRangeException(
					"attempt to read data outside of exif segment (index "
					+ offSet
					+ " where max index is "
					+ (_data.Length - 1)
					+ ")");
			}

			if (_isMotorollaByteOrder) 
			{
				// Motorola big first
				return (int) ( ((uint)(_data[offSet] << 24 & 0xFF000000))
					| ((uint)(_data[offSet + 1] << 16 & 0xFF0000))
					| ((uint)(_data[offSet + 2] << 8 & 0xFF00))
					| ((uint)(_data[offSet + 3] & 0xFF)));
			} 
			else 
			{
				// Intel ordering
				return (int) ( ((uint)(_data[offSet + 3] << 24 & 0xFF000000))
					| ((uint)(_data[offSet + 2] << 16 & 0xFF0000))
					| ((uint)(_data[offSet + 1] << 8 & 0xFF00))
					| ((uint)(_data[offSet] & 0xFF)));
			}
		}
	}
}
