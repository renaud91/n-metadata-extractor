using System;
using System.Collections;
using System.Collections.Generic;
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
/// leave this lcHeader in tact.
///
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
/// Created on 28 April 2002, 23:54
/// Modified 04 Aug 2002
/// - Renamed constants to be inline with changes to ExifTagValues interface
/// - Substituted usage of JDK 1.4 features (java.nio package)
/// Modified 29 Oct 2002 (v1.2)
/// - Proper traversing of Exif aFile structure and complete refactor & tidy of 
///   the codebase (a few unnoticed bugs removed)
/// - Reads makernote data for 6 families of camera (5 makes)
/// - Tags now stored in directories... use the IFD_* constants to refer to the 
///   image aFile directory you require (Exif, Interop, GPS and Makernote*) 
///   -- this avoids collisions where two tags share the same code
/// - Takes componentCount of unknown tags into account
/// - Now understands GPS tags (thanks to Colin Briton for his help with this)
/// - Some other bug fixes, pointed out by users around the world.  Thanks!
/// Modified 27 Nov 2002 (v2.0)
/// - Renamed to ExifReader
/// - Moved to new package com.drew.aMetadata.exif
/// 
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.drew.metadata.exif
{
    /// <summary>
    /// Extracts Exif data from a JPEG lcHeader segment, providing information about 
    /// the camera/scanner/capture device (if available).  
    /// Information is encapsulated in an Metadata object.
    /// </summary>
    public class ExifReader : IMetadataReader
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
        private const int FMT_BYTE = 1;
        private const int FMT_STRING = 2;
        private const int FMT_USHORT = 3;
        private const int FMT_ULONG = 4;
        private const int FMT_URATIONAL = 5;
        private const int FMT_SBYTE = 6;
        private const int FMT_UNDEFINED = 7;
        private const int FMT_SSHORT = 8;
        private const int FMT_SLONG = 9;
        private const int FMT_SRATIONAL = 10;
        private const int FMT_SINGLE = 11;
        private const int FMT_DOUBLE = 12;

        public const int TAG_EXIF_OFFSET = 0x8769;
        public const int TAG_INTEROP_OFFSET = 0xA005;
        public const int TAG_GPS_INFO_OFFSET = 0x8825;
        public const int TAG_MAKER_NOTE = 0x927C;

        // NOT READONLY
        public static int TIFF_HEADER_START_OFFSET = 6;

        private const string MARK_AS_PROCESSED = "processed";

        /// <summary>
        /// Constructor of the object
        /// </summary>
        /// <param name="aFile">the aFile to read</param>
        public ExifReader(FileInfo file)
            : this(
            new JpegSegmentReader(file).ReadSegment(
            JpegSegmentReader.SEGMENT_APP1))
        {
        }

        /**
         * Creates an ExifReader for the given JPEG lcHeader segment.
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
        /// <param name="aMetadata">where to add meta data</param>
        /// <returns>the aMetadata</returns>
        public Metadata Extract(Metadata metadata)
        {
            _metadata = metadata;
            if (_data == null)
            {
                return _metadata;
            }

            // once we know there'str some data, create the directory and start working on it
            ExifDirectory directory = (ExifDirectory)_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory"));

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
            //ProcessDirectory(directory, firstDirectoryOffSet);
            // after the extraction process, if we have the correct tags, we may be able to extract thumbnail information
            //ExtractThumbnail(directory);

            Dictionary<int, string> processedDirectoryOffsets = new Dictionary<int, string>();

            // 0th IFD (we merge with Exif IFD)
            ProcessDirectory(directory, processedDirectoryOffsets, firstDirectoryOffSet, TIFF_HEADER_START_OFFSET);

            // after the extraction process, if we have the correct tags, we may be able to store thumbnail information
            StoreThumbnailBytes(directory, TIFF_HEADER_START_OFFSET);


            return _metadata;
        }

        /// <summary>
        /// Will stock the thumbnail into exif directory if available.
        /// </summary>
        /// <param name="exifDirectory">where to stock the thumbnail</param>
        /// <param name="tiffHeaderOffset">the tiff lcHeader lcOffset value</param>
        private void StoreThumbnailBytes(ExifDirectory exifDirectory, int tiffHeaderOffset)
        {
            if (!exifDirectory.ContainsTag(ExifDirectory.TAG_COMPRESSION))
            {
                return;
            }

            if (!exifDirectory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_LENGTH) ||
                !exifDirectory.ContainsTag(ExifDirectory.TAG_THUMBNAIL_OFFSET))
            {
                return;
            }
            try
            {
                int offset = exifDirectory.GetInt(ExifDirectory.TAG_THUMBNAIL_OFFSET);
                int length = exifDirectory.GetInt(ExifDirectory.TAG_THUMBNAIL_LENGTH);
                byte[] result = new byte[length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _data[tiffHeaderOffset + offset + i];
                }
                exifDirectory.SetObject(ExifDirectory.TAG_THUMBNAIL_DATA, result);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unable to extract thumbnail: " + e.Message);
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
        /// Indicates if Directory Length is valid or not
        /// </summary>
        /// <param name="dirStartOffSet">where to start</param>
        /// <param name="tiffHeaderOffset">The tiff lcHeader lcOffset</param>
        /// <returns>true if Directory Length is valid</returns>
        private bool IsDirectoryLengthValid(int dirStartOffset, int tiffHeaderOffset)
        {
            int dirTagCount = Get16Bits(dirStartOffset);
            int dirLength = (2 + (12 * dirTagCount) + 4);
            if (dirLength + dirStartOffset + tiffHeaderOffset >= _data.Length)
            {
                // Note: Files that had thumbnails trimmed with jhead 1.3 or earlier might trigger this
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determine the lcOffset at which a given InteropArray entry begins within the specified IFD. 
        /// </summary>
        /// <param name="dirStartOffset">the lcOffset at which the IFD starts</param>
        /// <param name="entryNumber">the zero-based entry number</param>
        /// <returns>the lcOffset at which a given InteropArray entry begins within the specified IFD</returns>
        private int CalculateTagOffset(int dirStartOffset, int entryNumber)
        {
            // add 2 bytes for the tag count
            // each entry is 12 bytes, so we skip 12 * the number seen so far
            return dirStartOffset + 2 + (12 * entryNumber);
        }

        /// <summary>
        /// Calculates tag value lcOffset
        /// </summary>
        /// <param name="byteCount">the byte count</param>
        /// <param name="dirEntryOffset">the dir entry lcOffset</param>
        /// <param name="tiffHeaderOffset">the tiff lcHeader ofset</param>
        /// <returns>-1 if error, or the valus lcOffset</returns>
        private int CalculateTagValueOffset(int byteCount, int dirEntryOffset, int tiffHeaderOffset)
        {
            if (byteCount > 4)
            {
                // If its bigger than 4 bytes, the dir entry contains an lcOffset.
                // dirEntryOffset must be passed, as some makernote implementations (e.g. FujiFilm) incorrectly use an
                // lcOffset relative to the start of the makernote itself, not the TIFF segment.
                int offsetVal = Get32Bits(dirEntryOffset + 8);
                if (offsetVal + byteCount > _data.Length)
                {
                    // Bogus pointer lcOffset and / or bytecount value
                    return -1; // signal error
                }
                return tiffHeaderOffset + offsetVal;
            }
            // 4 bytes or less and value is in the dir entry itself
            return dirEntryOffset + 8;
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
        private void ProcessDirectory(AbstractDirectory directory, Dictionary<int, string> processedDirectoryOffsets, int dirStartOffset, int tiffHeaderOffset)
        {
            // check for directories we've already visited to avoid stack overflows when recursive/cyclic directory structures exist
            if (processedDirectoryOffsets.ContainsKey(dirStartOffset))
            {
                return;
            }
            // remember that we've visited this directory so that we don't visit it again later
            processedDirectoryOffsets.Add(dirStartOffset, MARK_AS_PROCESSED);

            if (dirStartOffset >= _data.Length || dirStartOffset < 0)
            {
                directory.AddError("Ignored directory marked to start outside data segement");
                return;
            }

            if (!IsDirectoryLengthValid(dirStartOffset, tiffHeaderOffset))
            {
                directory.AddError("Illegally sized directory");
                return;
            }

            // First two bytes in the IFD are the tag count
            int dirTagCount = Get16Bits(dirStartOffset);

            // Handle each tag in this directory
            for (int tagNumber = 0; tagNumber < dirTagCount; tagNumber++)
            {
                int tagOffset = CalculateTagOffset(dirStartOffset, tagNumber);

                // 2 bytes for the tag type
                int tagType = Get16Bits(tagOffset);

                // 2 bytes for the format code
                int formatCode = Get16Bits(tagOffset + 2);
                if (formatCode < 1 || formatCode > MAX_FORMAT_CODE)
                {
                    directory.AddError("Invalid format code: " + formatCode);
                    continue;
                }

                // 4 bytes dictate the number of components in this tag'lcStr data
                int componentCount = Get32Bits(tagOffset + 4);
                if (componentCount < 0)
                {
                    directory.AddError("Negative component count in EXIF");
                    continue;
                }

                // each component may have more than one byte... calculate the total number of bytes
                int byteCount = componentCount * BYTES_PER_FORMAT[formatCode];
                int tagValueOffset = CalculateTagValueOffset(byteCount, tagOffset, tiffHeaderOffset);
                if (tagValueOffset < 0 || tagValueOffset > _data.Length)
                {
                    directory.AddError("Illegal pointer offset value in EXIF");
                    continue;
                }


                // Check that this tag isn't going to allocate outside the bounds of the data array.
                // This addresses an uncommon OutOfMemoryError.
                if (byteCount < 0 || tagValueOffset + byteCount > _data.Length)
                {
                    directory.AddError("Illegal number of bytes: " + byteCount);
                    continue;
                }

                // Calculate the value as an lcOffset for cases where the tag represents directory
                int subdirOffset = tiffHeaderOffset + Get32Bits(tagValueOffset);

                switch (tagType)
                {
                    case TAG_EXIF_OFFSET:
                        ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
                        continue;
                    case TAG_INTEROP_OFFSET:
                        ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifInteropDirectory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
                        continue;
                    case TAG_GPS_INFO_OFFSET:
                        ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.GpsDirectory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
                        continue;
                    case TAG_MAKER_NOTE:
                        ProcessMakerNote(tagValueOffset, processedDirectoryOffsets, tiffHeaderOffset);
                        continue;
                    default:
                        ProcessTag(directory, tagType, tagValueOffset, componentCount, formatCode);
                        break;
                }
            } // End of for
            // at the end of each IFD is an optional link to the next IFD
            int finalTagOffset = CalculateTagOffset(dirStartOffset, dirTagCount);
            int nextDirectoryOffset = Get32Bits(finalTagOffset);
            if (nextDirectoryOffset != 0)
            {
                nextDirectoryOffset += tiffHeaderOffset;
                if (nextDirectoryOffset >= _data.Length)
                {
                    // Last 4 bytes of IFD reference another IFD with an address that is out of bounds
                    // Note this could have been caused by jhead 1.3 cropping too much
                    return;
                }
                else if (nextDirectoryOffset < dirStartOffset)
                {
                    // Last 4 bytes of IFD reference another IFD with an address that is before the start of this directory
                    return;
                }
                // the next directory is of same type as this one
                ProcessDirectory(directory, processedDirectoryOffsets, nextDirectoryOffset, tiffHeaderOffset);
            }
        }

        /// <summary>
        /// Determine the camera model and makernote format
        /// </summary>
        /// <param name="subdirOffset">the sub lcOffset dir</param>
        /// <param name="processedDirectoryOffsets">the processed directory offsets</param>
        /// <param name="tiffHeaderOffset">the tiff lcHeader lcOffset</param>
        private void ProcessMakerNote(int subdirOffset, Dictionary<int, string> processedDirectoryOffsets, int tiffHeaderOffset)
        {
            // Console.WriteLine("ProcessMakerNote value="+subdirOffSet);
            // Determine the camera model and makernote format
            AbstractDirectory exifDirectory = _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.ExifDirectory"));
            if (exifDirectory == null)
            {
                return;
            }

            string cameraModel = exifDirectory.GetString(ExifDirectory.TAG_MAKE);
            string firstTwoChars = Utils.Decode(_data, subdirOffset, 2, false);
            string firstThreeChars = Utils.Decode(_data, subdirOffset, 3, false);
            string firstFourChars = Utils.Decode(_data, subdirOffset, 4, false);
            string firstFiveChars = Utils.Decode(_data, subdirOffset, 5, false);
            string firstSixChars = Utils.Decode(_data, subdirOffset, 6, false);
            string firstSevenChars = Utils.Decode(_data, subdirOffset, 7, false);
            string firstEightChars = Utils.Decode(_data, subdirOffset, 8, false);

            if ("OLYMP".Equals(firstFiveChars) || "EPSON".Equals(firstFiveChars) || "AGFA".Equals(firstFourChars))
            {
                // Olympus Makernote
                // Epson and Agfa use Olypus maker note standard, see:
                //     http://www.ozhiker.com/electronics/pjmt/jpeg_info/
                ProcessDirectory(
                    _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.OlympusDirectory")), processedDirectoryOffsets, subdirOffset + 8, tiffHeaderOffset);
            }
            else if (cameraModel != null && cameraModel.Trim().ToUpper().StartsWith("NIKON"))
            {
                if ("Nikon".Equals(Utils.Decode(_data, subdirOffset, 5, false)))
                {
                    // There are two scenarios here:
                    // Type 1:
                    // :0000: 4E 69 6B 6F 6E 00 01 00-05 00 02 00 02 00 06 00 Nikon...........
                    // :0010: 00 00 EC 02 00 00 03 00-03 00 01 00 00 00 06 00 ................
                    // Type 3:
                    // :0000: 4E 69 6B 6F 6E 00 02 00-00 00 4D 4D 00 2A 00 00 Nikon....MM.*...
                    // :0010: 00 08 00 1E 00 01 00 07-00 00 00 04 30 32 30 30 ............0200
                    if (_data[subdirOffset + 6] == 1)
                    {
                        // Nikon type 1 Makernote
                        ProcessDirectory(
                            _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType1Directory")), processedDirectoryOffsets, subdirOffset + 8, tiffHeaderOffset);
                    }
                    else if (_data[subdirOffset + 6] == 2)
                    {
                        // Nikon type 2 Makernote
                        ProcessDirectory(
                            _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType2Directory")), processedDirectoryOffsets, subdirOffset + 18, subdirOffset + 10);
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
                        _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.NikonType2Directory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
                }
            }
            else if ("SONY CAM".Equals(firstEightChars) || "SONY DSC".Equals(firstEightChars))
            {
                ProcessDirectory(
                    _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.SonyDirectory")), processedDirectoryOffsets, subdirOffset + 12, tiffHeaderOffset);
            }
            else if ("KDK".Equals(firstThreeChars))
            {
                ProcessDirectory(
                    _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.KodakDirectory")), processedDirectoryOffsets, subdirOffset + 20, tiffHeaderOffset);
            }


            else if ("Canon".ToUpper().Equals(cameraModel.ToUpper()))
            {
                // Canon Makernote
                ProcessDirectory(
                    _metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CanonDirectory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
            }
            else if (cameraModel != null && cameraModel.ToUpper().StartsWith("CASIO"))
            {
                if ("QVC\u0000\u0000\u0000".Equals(firstSixChars))
                {
                    ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CasioType2Directory")), processedDirectoryOffsets, subdirOffset + 6, tiffHeaderOffset);
                }
                else
                {
                    ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CasioType1Directory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
                }
            }
            else if ("FUJIFILM".Equals(firstEightChars) || "Fujifilm".ToUpper().Equals(cameraModel.ToUpper()))
            {
                // TODO make this field a passed parameter, to avoid threading issues
                bool byteOrderBefore = _isMotorollaByteOrder;
                // bug in fujifilm makernote ifd means we temporarily use Intel byte ordering
                _isMotorollaByteOrder = false;
                // the 4 bytes after "FUJIFILM" in the makernote point to the start of the makernote
                // IFD, though the lcOffset is relative to the start of the makernote, not the TIFF
                // lcHeader (like everywhere else)
                int ifdStart = subdirOffset + Get32Bits(subdirOffset + 8);
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.FujifilmDirectory")), processedDirectoryOffsets, ifdStart, tiffHeaderOffset);
                _isMotorollaByteOrder = byteOrderBefore;
            }
            else if (cameraModel != null && cameraModel.ToUpper().StartsWith("MINOLTA"))
            {
                // Cases seen with the model starting with MINOLTA in capitals seem to have a valid Olympus makernote
                // area that commences immediately.
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.OlympusDirectory")), processedDirectoryOffsets, subdirOffset, tiffHeaderOffset);
            }
            else if ("KC".Equals(firstTwoChars) || "MINOL".Equals(firstFiveChars) || "MLY".Equals(firstThreeChars) || "+M+M+M+M".Equals(firstEightChars))
            {
                // This Konica data is not understood.  Header identified in accordance with information at this site:
                // http://www.ozhiker.com/electronics/pjmt/jpeg_info/minolta_mn.html
                // TODO determine how to process the information described at the above website
                exifDirectory.AddError("Unsupported Konica/Minolta data ignored.");
            }
            else if ("KYOCERA".Equals(firstSevenChars))
            {
                // http://www.ozhiker.com/electronics/pjmt/jpeg_info/kyocera_mn.html
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.KyoceraDirectory")), processedDirectoryOffsets, subdirOffset + 22, tiffHeaderOffset);
            }
            else if ("Panasonic\u0000\u0000\u0000".Equals(Utils.Decode(_data, subdirOffset, 12, false)))
            {
                // NON-Standard TIFF IFD Data using Panasonic Tags. There is no Next-IFD pointer after the IFD
                // Offsets are relative to the start of the TIFF lcHeader at the beginning of the EXIF segment
                // more information here: http://www.ozhiker.com/electronics/pjmt/jpeg_info/panasonic_mn.html
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.PanasonicDirectory")), processedDirectoryOffsets, subdirOffset + 12, tiffHeaderOffset);
            }
            else if ("AOC\u0000".Equals(firstFourChars))
            {
                // NON-Standard TIFF IFD Data using Casio Type 2 Tags
                // IFD has no Next-IFD pointer at end of IFD, and
                // Offsets are relative to the start of the current IFD tag, not the TIFF lcHeader
                // Observed for:
                // - Pentax ist D
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.CasioType2Directory")), processedDirectoryOffsets, subdirOffset + 6, subdirOffset);
            }
            else if (cameraModel != null && (cameraModel.ToUpper().StartsWith("PENTAX") || cameraModel.ToUpper().StartsWith("ASAHI")))
            {
                // NON-Standard TIFF IFD Data using Pentax Tags
                // IFD has no Next-IFD pointer at end of IFD, and
                // Offsets are relative to the start of the current IFD tag, not the TIFF lcHeader
                // Observed for:
                // - PENTAX Optio 330
                // - PENTAX Optio 430
                ProcessDirectory(_metadata.GetDirectory(Type.GetType("com.drew.metadata.exif.PentaxDirectory")), processedDirectoryOffsets, subdirOffset, subdirOffset);
            }
            else
            {
                // TODO how to store makernote data when it'str not from a supported camera model?
                exifDirectory.AddError("Unsupported makernote data ignored.");
            }
        }


        /// <summary>
        /// Processes tag
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <param name="aTagType">the tag type</param>
        /// <param name="tagValueOffset">the lcOffset value</param>
        /// <param name="componentCount">the component count</param>
        /// <param name="formatCode">the format code</param>
        private void ProcessTag(
            AbstractDirectory directory,
            int tagType,
            int tagValueOffset,
            int componentCount,
            int formatCode)
        {
            // Directory simply stores raw values
            // The display side uses a Descriptor class per directory to turn the raw values into 'pretty' descriptions
            switch (formatCode)
            {
                case FMT_UNDEFINED:
                    // this includes exif user comments
                    byte[] tagBytes = new byte[componentCount];
                    int byteCount = componentCount * BYTES_PER_FORMAT[formatCode];
                    for (int i = 0; i < byteCount; i++)
                    {
                        tagBytes[i] = _data[tagValueOffset + i];
                    }
                    directory.SetObject(tagType, tagBytes);
                    break;
                case FMT_STRING:
                    string lcStr = null;
                    if (tagType == ExifDirectory.TAG_USER_COMMENT)
                    {
                        lcStr =
                            ReadCommentString(
                            tagValueOffset,
                            componentCount,
                            formatCode);
                    }
                    else
                    {
                        lcStr = ReadString(tagValueOffset, componentCount);
                    }
                    directory.SetObject(tagType, lcStr);
                    break;
                case FMT_SRATIONAL: //goto case FMT_URATIONAL;
                case FMT_URATIONAL:
                    if (componentCount == 1)
                    {
                        Rational rational = new Rational(Get32Bits(tagValueOffset), Get32Bits(tagValueOffset + 4));
                        directory.SetObject(tagType, rational);
                    }
                    else
                    {
                        Rational[] rationals = new Rational[componentCount];
                        for (int i = 0; i < componentCount; i++)
                        {
                            rationals[i] = new Rational(Get32Bits(tagValueOffset + (8 * i)), Get32Bits(tagValueOffset + 4 + (8 * i)));
                        }
                        directory.SetObject(tagType, rationals);
                    }
                    break;
                case FMT_SBYTE: //goto case FMT_BYTE;
                case FMT_BYTE:
                    if (componentCount == 1)
                    {
                        // this may need to be a byte, but I think casting to int is fine
                        int b = _data[tagValueOffset];
                        directory.SetObject(tagType, b);
                    }
                    else
                    {
                        int[] bytes = new int[componentCount];
                        for (int i = 0; i < componentCount; i++)
                        {
                            bytes[i] = _data[tagValueOffset + i];
                        }
                        directory.SetIntArray(tagType, bytes);
                    }
                    break;
                case FMT_SINGLE: //goto case FMT_DOUBLE;
                case FMT_DOUBLE:
                    if (componentCount == 1)
                    {
                        int i = _data[tagValueOffset];
                        directory.SetObject(tagType, i);
                    }
                    else
                    {
                        int[] ints = new int[componentCount];
                        for (int i = 0; i < componentCount; i++)
                        {
                            ints[i] = _data[tagValueOffset + i];
                        }
                        directory.SetIntArray(tagType, ints);
                    }
                    break;
                case FMT_USHORT: //goto case FMT_SSHORT;
                case FMT_SSHORT:
                    if (componentCount == 1)
                    {
                        int i = Get16Bits(tagValueOffset);
                        directory.SetObject(tagType, i);
                    }
                    else
                    {
                        int[] ints = new int[componentCount];
                        for (int i = 0; i < componentCount; i++)
                            ints[i] = Get16Bits(tagValueOffset + (i * 2));
                        directory.SetIntArray(tagType, ints);
                    }
                    break;
                case FMT_SLONG: //goto case FMT_ULONG;
                case FMT_ULONG:
                    if (componentCount == 1)
                    {
                        int i = Get32Bits(tagValueOffset);
                        directory.SetObject(tagType, i);
                    }
                    else
                    {
                        int[] ints = new int[componentCount];
                        for (int i = 0; i < componentCount; i++)
                            ints[i] = Get32Bits(tagValueOffset + (i * 4));
                        directory.SetIntArray(tagType, ints);
                    }
                    break;
                default:
                    directory.AddError("Unknown format code " + formatCode + " for tag " + tagType);
                    break;
            }
        }

        /// <summary>
        /// Creates a string from the _data buffer starting at the specified offSet, 
        /// and ending where byte=='\0' or where Length==maxLength.
        /// </summary>
        /// <param name="offSet">the lcOffset</param>
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
        /// <param name="tagValueOffSet">the tag value lcOffset</param>
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
                    _data[tagValueOffSet + i] = (byte)'\0';
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
                int start = tagValueOffSet + 7;
                for (int i = start; i < 10 + start; i++)
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
                return Utils.Decode(_data, start, end - start, true);

            }

            // TODO implement support for UNICODE and JIS UserComment encodings..?
            return ReadString(tagValueOffSet, 1999);
        }

        /// <summary>
        /// Determine the offSet at which a given InteropArray entry begins within the specified IFD.
        /// </summary>
        /// <param name="ifdStartOffSet">the offSet at which the IFD starts</param>
        /// <param name="entryNumber">the zero-based entry number</param>
        /// <returns>the directory entry lcOffset</returns>
        private int CalculateDirectoryEntryOffSet(
            int ifdStartOffSet,
            int entryNumber)
        {
            return (ifdStartOffSet + 2 + (12 * entryNumber));
        }


        /// <summary>
        /// Gets a 16 bit aValue from aFile'str native byte order.  Between 0x0000 and 0xFFFF.
        /// </summary>
        /// <param name="offSet">the lcOffset</param>
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
        /// Gets a 32 bit aValue from aFile'str native byte order.
        /// </summary>
        /// <param name="offSet">the lcOffset</param>
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
                return (int)(((uint)(_data[offSet] << 24 & 0xFF000000))
                    | ((uint)(_data[offSet + 1] << 16 & 0xFF0000))
                    | ((uint)(_data[offSet + 2] << 8 & 0xFF00))
                    | ((uint)(_data[offSet + 3] & 0xFF)));
            }
            else
            {
                // Intel ordering
                return (int)(((uint)(_data[offSet + 3] << 24 & 0xFF000000))
                    | ((uint)(_data[offSet + 2] << 16 & 0xFF0000))
                    | ((uint)(_data[offSet + 1] << 8 & 0xFF00))
                    | ((uint)(_data[offSet] & 0xFF)));
            }
        }
    }
}
