using System;
using System.Text;
using System.Collections;
using System.IO;
using com.drew.metadata;
using com.drew.imaging.jpg;

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
	/// The JPEG reader class
	/// </summary>
	public class JpegReader : IMetadataReader 
	{
		/// <summary>
		/// The SOF0 data segment. 
		/// </summary>
		private byte[] _data;

		/// <summary>
		/// Creates a new IptcReader for the specified Jpeg jpegFile.
		/// </summary>
		/// <param name="jpegFile">where to read</param>
		public JpegReader(FileInfo jpegFile) : this(
			new JpegSegmentReader(jpegFile).ReadSegment(
			JpegSegmentReader.SEGMENT_SOF0)) 
		{
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="data">the data to read</param>
		public JpegReader(byte[] data) 
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

			JpegDirectory directory =
				(JpegDirectory) metadata.GetDirectory(Type.GetType("com.drew.metadata.jpeg.JpegDirectory"));

			try 
			{
				// data precision
				int dataPrecision =
					Get16Bits(JpegDirectory.TAG_JPEG_DATA_PRECISION);
				directory.SetObject(
					JpegDirectory.TAG_JPEG_DATA_PRECISION,
					dataPrecision);

				// process height
				int height = Get32Bits(JpegDirectory.TAG_JPEG_IMAGE_HEIGHT);
				directory.SetObject(JpegDirectory.TAG_JPEG_IMAGE_HEIGHT, height);

				// process width
				int width = Get32Bits(JpegDirectory.TAG_JPEG_IMAGE_WIDTH);
				directory.SetObject(JpegDirectory.TAG_JPEG_IMAGE_WIDTH, width);

				// number of components
				int numberOfComponents =
					Get16Bits(JpegDirectory.TAG_JPEG_NUMBER_OF_COMPONENTS);
				directory.SetObject(
					JpegDirectory.TAG_JPEG_NUMBER_OF_COMPONENTS,
					numberOfComponents);

				// for each component, there are three bytes of data:
				// 1 - Component ID: 1 = Y, 2 = Cb, 3 = Cr, 4 = I, 5 = Q
				// 2 - Sampling factors: bit 0-3 vertical, 4-7 horizontal
				// 3 - Quantization table number
				int offset = 6;
				for (int i = 0; i < numberOfComponents; i++) 
				{
					int componentId = Get16Bits(offset++);
					int samplingFactorByte = Get16Bits(offset++);
					int quantizationTableNumber = Get16Bits(offset++);
					JpegComponent component =
						new JpegComponent(
						componentId,
						samplingFactorByte,
						quantizationTableNumber);
					directory.SetObject(
						JpegDirectory.TAG_JPEG_COMPONENT_DATA_1 + i,
						component);
				}

			} 
			catch (MetadataException me) 
			{
				directory.AddError("MetadataException: " + me);
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
			if (offset + 1 >= _data.Length) 
			{
				throw new MetadataException("Attempt to read bytes from outside Jpeg segment data buffer");
			}

			return ((_data[offset] & 255) << 8) | (_data[offset + 1] & 255);
		}

		/// <summary>
		/// Returns an int calculated from one byte of data at the specified lcOffset.
		/// </summary>
		/// <param name="lcOffset">position within the data buffer to read byte</param>
		/// <returns>the 16 bit int value, between 0x00 and 0xFF</returns>
		private int Get16Bits(int offset) 
		{
			if (offset >= _data.Length) 
			{
				throw new MetadataException("Attempt to read bytes from outside Jpeg segment data buffer");
			}

			return (_data[offset] & 255);
		}
	}
}