using System;
using System.Collections;
using System.IO;
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
namespace com.drew.metadata.jpeg
{
	/// <summary>
	/// The Jpeg reader class
	/// </summary>
	public class JpegCommentReader : IMetadataReader 
	{
		/// <summary>
		/// The COM data segment.
		/// </summary>
		private byte[] _data;

		/// <summary>
		/// Creates a new JpegReader for the specified Jpeg jpegFile.
		/// </summary>
		/// <param name="jpegFile">where to read</param>
		public JpegCommentReader(FileInfo jpegFile) : this(
			new JpegSegmentReader(jpegFile).ReadSegment(
			JpegSegmentReader.SEGMENT_COM))
		{
		}

		/// <summary>
		/// Creates a new JpegReader for the specified Jpeg jpegFile.
		/// </summary>
		/// <param name="data">where to read</param>
		public JpegCommentReader(byte[] data) 
		{
			_data = data;
		}

		/// <summary>
		/// Performs the Jpeg data extraction, returning a new instance of Metadata. 
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

			JpegCommentDirectory directory =
				(JpegCommentDirectory) metadata.GetDirectory(
				Type.GetType("com.drew.metadata.jpeg.JpegCommentDirectory"));
            string comment = Utils.Decode(_data, true);
			directory.SetObject(
				JpegCommentDirectory.TAG_JPEG_COMMENT,
				comment);

			return metadata;
		}
	}
}