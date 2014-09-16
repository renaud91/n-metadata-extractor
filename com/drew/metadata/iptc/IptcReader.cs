using System;
using System.Collections;
using System.IO;
using com.drew.lang;
using com.drew.metadata;
using com.drew.imaging.jpg;
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
namespace com.drew.metadata.iptc
{
    /// <summary>
    /// The Iptc reader class
    /// </summary>
    public class IptcReader : IMetadataReader
    {
        /*
            public const int DIRECTORY_IPTC = 2;
	
            public const int ENVELOPE_RECORD = 1;
            public const int APPLICATION_RECORD_2 = 2;
            public const int APPLICATION_RECORD_3 = 3;
            public const int APPLICATION_RECORD_4 = 4;
            public const int APPLICATION_RECORD_5 = 5;
            public const int APPLICATION_RECORD_6 = 6;
            public const int PRE_DATA_RECORD = 7;
            public const int DATA_RECORD = 8;
            public const int POST_DATA_RECORD = 9;
        */

        /// <summary>
        /// The Iptc data segment
        /// </summary>
        private readonly byte[] _data;

        /// <summary>
        /// Creates a new IptcReader for the specified Jpeg jpegFile.
        /// </summary>
        /// <param name="jpegFile">where to read</param>
        public IptcReader(FileInfo jpegFile)
            : this(
            new JpegSegmentReader(jpegFile).ReadSegment(
            JpegSegmentReader.SEGMENT_APPD))
        {
        }

        /// <summary>
        /// Constructor of the object
        /// </summary>
        /// <param name="data">the data to read</param>
        public IptcReader(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Performs the Iptc data extraction, returning a new instance of Metadata. 
        /// </summary>
        /// <returns>a new instance of Metadata</returns>
        public Metadata Extract()
        {
            return Extract(new Metadata());
        }

        /// <summary>
        /// Extracts aMetadata
        /// </summary>
        /// <param name="aMetadata">where to add aMetadata</param>
        /// <returns>the aMetadata found</returns>
        public Metadata Extract(Metadata metadata)
        {
            if (_data == null)
            {
                return metadata;
            }

            AbstractDirectory directory = metadata.GetDirectory(Type.GetType("com.drew.metadata.iptc.IptcDirectory"));

            // find start of data
            int offset = 0;
            try
            {
                while (offset < _data.Length - 1 && Get32Bits(offset) != 0x1c02)
                {
                    offset++;
                }
            }
            catch (MetadataException)
            {
                directory.AddError(
                    "Couldn't find start of Iptc data (invalid segment)");
                return metadata;
            }

            // for each tag
            while (offset < _data.Length)
            {
                // identifies start of a tag
                if (_data[offset] != 0x1c)
                {
                    break;
                }
                // we need at least five bytes left to read a tag
                if ((offset + 5) >= _data.Length)
                {
                    break;
                }

                offset++;

                int directoryType;
                int tagType;
                int tagByteCount;
                try
                {
                    directoryType = _data[offset++];
                    tagType = _data[offset++];
                    tagByteCount = Get32Bits(offset);
                }
                catch (MetadataException)
                {
                    directory.AddError(
                        "Iptc data segment ended mid-way through tag descriptor");
                    return metadata;
                }
                offset += 2;
                if ((offset + tagByteCount) > _data.Length)
                {
                    directory.AddError(
                        "data for tag extends beyond end of iptc segment");
                    break;
                }

                ProcessTag(directory, directoryType, tagType, offset, tagByteCount);
                offset += tagByteCount;
            }

            return metadata;
        }

        /// <summary>
        /// Returns an int calculated from two bytes of data at the specified lcOffset (MSB, LSB).
        /// </summary>
        /// <param name="lcOffset">position within the data buffer to read first byte</param>
        /// <returns>the 32 bit int value, between 0x0000 and 0xFFFF</returns>
        private int Get32Bits(int offset)
        {
            if (offset >= _data.Length)
            {
                throw new MetadataException("Attempt to read bytes from outside Iptc data buffer");
            }
            return ((_data[offset] & 255) << 8) | (_data[offset + 1] & 255);
        }

        /// <summary>
        /// This method serves as marsheller of objects for dataset. 
        /// It converts from IPTC octets to relevant java object.
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <param name="directoryType">the directory type</param>
        /// <param name="aTagType">the tag type</param>
        /// <param name="lcOffset">the lcOffset</param>
        /// <param name="tagByteCount">the tag byte count</param>
        private void ProcessTag(
            AbstractDirectory directory,
            int directoryType,
            int tagType,
            int offset,
            int tagByteCount)
        {
            int tagIdentifier = tagType | (directoryType << 8);

            switch (tagIdentifier)
            {
                case IptcDirectory.TAG_RECORD_VERSION:
                    // short
                    short shortValue = (short)((_data[offset] << 8) | _data[offset + 1]);
                    directory.SetObject(tagIdentifier, shortValue);
                    return;
                case IptcDirectory.TAG_URGENCY:
                    // byte
                    directory.SetObject(tagIdentifier, _data[offset]);
                    return;
                case IptcDirectory.TAG_RELEASE_DATE:
                case IptcDirectory.TAG_DATE_CREATED:
                    // Date object
                    if (tagByteCount >= 8)
                    {
                        string dateStr = Utils.Decode(_data, offset, tagByteCount, false);
                        try
                        {
                            int year = Convert.ToInt32(dateStr.Substring(0, 4));
                            int month = Convert.ToInt32(dateStr.Substring(4, 2)); //No -1 here;
                            int day = Convert.ToInt32(dateStr.Substring(6, 2));
                            DateTime date = new DateTime(year, month, day);
                            directory.SetObject(tagIdentifier, date);
                            return;
                        }
                        catch (FormatException)
                        {
                            // fall through and we'll store whatever was there as a String
                        }
                    }
                    break; // Added for .Net compiler
                //case IptcDirectory.TAG_RELEASE_TIME:
                //case IptcDirectory.TAG_TIME_CREATED: 
            }
            // If no special handling by now, treat it as a string
            string str;
            if (tagByteCount < 1)
            {
                str = "";
            }
            else
            {
                str = Utils.Decode(_data, offset, tagByteCount, false);
            }
            if (directory.ContainsTag(tagIdentifier))
            {
                string[] oldStrings;
                string[] newStrings;
                try
                {
                    oldStrings = directory.GetStringArray(tagIdentifier);
                }
                catch (MetadataException)
                {
                    oldStrings = null;
                }
                if (oldStrings == null)
                {
                    newStrings = new String[1];
                }
                else
                {
                    newStrings = new string[oldStrings.Length + 1];
                    for (int i = 0; i < oldStrings.Length; i++)
                    {
                        newStrings[i] = oldStrings[i];
                    }
                }
                newStrings[newStrings.Length - 1] = str;
                directory.SetObject(tagIdentifier, newStrings);
            }
            else
            {
                directory.SetObject(tagIdentifier, str);
            }
        }
    }
}