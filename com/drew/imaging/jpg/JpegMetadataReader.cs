using System;
using System.IO;
using com.codec.jpeg;
using com.drew.metadata;
using com.drew.metadata.jpeg;
using com.drew.metadata.iptc;
using com.drew.metadata.exif;

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
namespace com.drew.imaging.jpg
{
	/// <summary>
	/// This class will extract MetaData from a picture.
	/// </summary>
	public class JpegMetadataReader
	{
		/// <summary>
		/// Reads MetaData from a stream
		/// </summary>
		/// <param name="aStream">where to read information</param>
		/// <returns>the metadata object</returns>
		public static Metadata ReadMetadata(Stream aStream) 
		{
			JpegSegmentReader segmentReader = new JpegSegmentReader(aStream);
			return ExtractJpegSegmentReaderMetadata(segmentReader);
		}

		/// <summary>
		/// Reads MetaData from a file
		/// </summary>
		/// <param name="aFile">where to read information</param>
		/// <returns>the metadata object</returns>
		public static Metadata ReadMetadata(FileInfo aFile) 
		{
			JpegSegmentReader segmentReader = new JpegSegmentReader(aFile);
			return ExtractJpegSegmentReaderMetadata(segmentReader);
		}

		/// <summary>
		/// Extracts metadata from a SegmentReader
		/// </summary>
		/// <param name="segmentReader">where to extract metadata</param>
		/// <returns>the metadata found</returns>
		private static Metadata ExtractJpegSegmentReaderMetadata(JpegSegmentReader segmentReader) 
		{
			Metadata metadata = new Metadata();
			try 
			{
				byte[] exifSegment =
					segmentReader.ReadSegment(JpegSegmentReader.SEGMENT_APP1);
				new ExifReader(exifSegment).Extract(metadata);
			} 
			catch (JpegProcessingException ) 
			{
				// in the interests of catching as much data as possible, continue
				// TODO lodge error message within exif directory?
			}

			try 
			{
				byte[] iptcSegment =
					segmentReader.ReadSegment(JpegSegmentReader.SEGMENT_APPD);
				new IptcReader(iptcSegment).Extract(metadata);
			} 
			catch (JpegProcessingException ) 
			{
				// TODO log error message within iptc directory?
			}

			try 
			{
				byte[] jpegSegment =
					segmentReader.ReadSegment(JpegSegmentReader.SEGMENT_SOF0);
				new JpegReader(jpegSegment).Extract(metadata);
			} 
			catch (JpegProcessingException ) 
			{
				// TODO log error message within jpeg directory?
			}

			try 
			{
				byte[] jpegCommentSegment =
					segmentReader.ReadSegment(JpegSegmentReader.SEGMENT_COM);
				new JpegCommentReader(jpegCommentSegment).Extract(metadata);
			} 
			catch (JpegProcessingException ) 
			{
				// TODO log error message within jpegcomment directory?
			}

			return metadata;
		}

		/// <summary>
		/// Reads metadata from a JPEGDecodeParam object
		/// </summary>
		/// <param name="decodeParam">where to find metadata</param>
		/// <returns>the metadata found</returns>
		public static Metadata ReadMetadata(JPEGDecodeParam decodeParam) 
		{
			Metadata metadata = new Metadata();

			// We should only really be seeing Exif in _data[0]... the 2D array exists
			// because markers can theoretically appear multiple times in the file.			
			// TODO test this method
			byte[][] exifSegment =
				decodeParam.GetMarkerData(JPEGDecodeParam.APP1_MARKER);
			if (exifSegment != null && exifSegment[0].Length > 0) 
			{
				new ExifReader(exifSegment[0]).Extract(metadata);
			}

			// similarly, use only the first IPTC segment
			byte[][] iptcSegment =
				decodeParam.GetMarkerData(JPEGDecodeParam.APPD_MARKER);
			if (iptcSegment != null && iptcSegment[0].Length > 0) 
			{
				new IptcReader(iptcSegment[0]).Extract(metadata);
			}

			// NOTE: Unable to utilise JpegReader for the SOF0 frame here, as the decodeParam doesn't contain the byte[]

			// similarly, use only the first Jpeg Comment segment
			byte[][] jpegCommentSegment =
				decodeParam.GetMarkerData(JPEGDecodeParam.COMMENT_MARKER);
			if (jpegCommentSegment != null && jpegCommentSegment[0].Length > 0) 
			{
				new JpegCommentReader(jpegCommentSegment[0]).Extract(metadata);
			}

			return metadata;
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <exception cref="Exception">Allways</exception>
		private JpegMetadataReader() : base() 
		{
			throw new Exception("Do not use");
		}
	}
}
