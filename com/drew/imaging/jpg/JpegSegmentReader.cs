using System;
using System.IO;
using System.Collections;

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
	/// Will analyze a stream form an image
	/// </summary>
	public class JpegSegmentReader
	{
		private FileInfo file;

		private byte[] data;

		private Stream stream;

		private IDictionary segmentDataMap;

		private readonly static byte SEGMENT_SOS = (byte) 0xDA;

		private readonly static  byte MARKER_EOI = (byte) 0xD9;

		public readonly static  byte SEGMENT_APP0 = (byte) 0xE0;
		public readonly static  byte SEGMENT_APP1 = (byte) 0xE1;
		public readonly static  byte SEGMENT_APP2 = (byte) 0xE2;
		public readonly static  byte SEGMENT_APP3 = (byte) 0xE3;
		public readonly static  byte SEGMENT_APP4 = (byte) 0xE4;
		public readonly static  byte SEGMENT_APP5 = (byte) 0xE5;
		public readonly static  byte SEGMENT_APP6 = (byte) 0xE6;
		public readonly static  byte SEGMENT_APP7 = (byte) 0xE7;
		public readonly static  byte SEGMENT_APP8 = (byte) 0xE8;
		public readonly static  byte SEGMENT_APP9 = (byte) 0xE9;
		public readonly static  byte SEGMENT_APPA = (byte) 0xEA;
		public readonly static  byte SEGMENT_APPB = (byte) 0xEB;
		public readonly static  byte SEGMENT_APPC = (byte) 0xEC;
		public readonly static  byte SEGMENT_APPD = (byte) 0xED;
		public readonly static  byte SEGMENT_APPE = (byte) 0xEE;
		public readonly static  byte SEGMENT_APPF = (byte) 0xEF;

		public readonly static  byte SEGMENT_SOI = (byte) 0xD8;
		public readonly static  byte SEGMENT_DQT = (byte) 0xDB;
		public readonly static  byte SEGMENT_DHT = (byte) 0xC4;
		public readonly static  byte SEGMENT_SOF0 = (byte) 0xC0;
		public readonly static  byte SEGMENT_COM = (byte) 0xFE;

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aFile">where to read</param>
		public JpegSegmentReader(FileInfo aFile) : base()
		{
			this.file = aFile;
			this.data = null;
			this.stream = null;
			this.ReadSegments();
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aFileContents">where to read</param>
		public JpegSegmentReader(byte[] aFileContents) 
		{
			this.file = null;
			this.stream = null;
			this.data = aFileContents;
			this.ReadSegments();
		}

		/// <summary>
		/// Constructor of the object
		/// </summary>
		/// <param name="aStream">where to read</param>
		public JpegSegmentReader(Stream aStream) 
		{
			this.stream = aStream;
			this.file = null;
			this.data = null;
			this.ReadSegments();
		}

		/// <summary>
		/// Reads the first instance of a given Jpeg segment, returning the contents as a byte array. 
		/// </summary>
		/// <param name="segmentMarker">the byte identifier for the desired segment</param>
		/// <returns>the byte array if found, else null</returns>
		/// <exception cref="JpegProcessingException">for any problems processing the Jpeg data</exception>
		public byte[] ReadSegment(byte segmentMarker) 
		{
			return ReadSegment(segmentMarker, 0);
		}

		/// <summary>
		/// Reads the first instance of a given Jpeg segment, returning the contents as a byte array. 
		/// </summary>
		/// <param name="segmentMarker">the byte identifier for the desired segment</param>
		/// <param name="occurrence">the occurrence of the specified segment within the jpeg file</param>
		/// <returns>the byte array if found, else null</returns>
		/// <exception cref="JpegProcessingException">for any problems processing the Jpeg data</exception>
		public byte[] ReadSegment(byte segmentMarker, int occurrence) 
		{
			if (segmentDataMap.Contains(segmentMarker)) 
			{
				IList segmentList = (IList) segmentDataMap[segmentMarker];
				if (segmentList.Count <= occurrence) 
				{
					return null;
				}
				return (byte[]) segmentList[occurrence];
			} 
			else 
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the number of segment
		/// </summary>
		/// <param name="segmentMarker">the byte identifier for the desired segment</param>
		/// <returns>the number of segment or zero if segment does not exist</returns>
		public  int GetSegmentCount(byte segmentMarker) 
		{
			IList segmentList =
				(IList) segmentDataMap[segmentMarker];
			if (segmentList == null) 
			{
				return 0;
			}
			return segmentList.Count;
		}

		/// <summary>
		/// Reads segments
		/// </summary>
		/// <exception cref="JpegProcessingException">for any problems processing the Jpeg data</exception>
		private void ReadSegments() 
		{
			segmentDataMap = new Hashtable();
			BufferedStream inStream = GetJpegInputStream();
			try 
			{
				int offset = 0;
				// first two bytes should be jpeg magic number
				if (!IsValidJpegHeaderBytes(inStream)) 
				{
					throw new JpegProcessingException("not a jpeg file");
				}
				offset += 2;
				do 
				{
					// next byte is 0xFF
					byte segmentIdentifier = (byte) (inStream.ReadByte() & 0xFF);
					if ((segmentIdentifier & 0xFF) != 0xFF) 
					{
						throw new JpegProcessingException(
							"expected jpeg segment start identifier 0xFF at offset "
							+ offset
							+ ", not 0x"
							+ (segmentIdentifier & 0xFF).ToString("X"));
					}
					offset++;
					// next byte is <segment-marker>
					byte thisSegmentMarker = (byte) (inStream.ReadByte() & 0xFF);
					offset++;
					// next 2-bytes are <segment-size>: [high-byte] [low-byte]
					byte[] segmentLengthBytes = new byte[2];
					inStream.Read(segmentLengthBytes, 0, 2);
					offset += 2;
					int segmentLength =
						((segmentLengthBytes[0] << 8) & 0xFF00)
						| (segmentLengthBytes[1] & 0xFF);
					// segment length includes size bytes, so subtract two
					segmentLength -= 2;
					if (segmentLength > (inStream.Length-inStream.Position)) 
					{
						throw new JpegProcessingException("segment size would extend beyond file stream length");
					}
					byte[] segmentBytes = new byte[segmentLength];
					inStream.Read(segmentBytes, 0, segmentLength);
					offset += segmentLength;
					if ((thisSegmentMarker & 0xFF) == (SEGMENT_SOS & 0xFF)) 
					{
						// The 'Start-Of-Scan' segment's length doesn't include the image data, instead would
						// have to search for the two bytes: 0xFF 0xD9 (EOI).
						// It comes last so simply return at this point
						return;
					} 
					else if ((thisSegmentMarker & 0xFF) == (MARKER_EOI & 0xFF)) 
					{
						// the 'End-Of-Image' segment -- this should never be found in this fashion
						return;
					} 
					else 
					{
						IList segmentList;
						if (segmentDataMap.Contains(thisSegmentMarker)) 
						{
							segmentList = (IList) segmentDataMap[thisSegmentMarker];
						} 
						else 
						{
							segmentList = new ArrayList();
							segmentDataMap.Add(thisSegmentMarker, segmentList);
						}
						segmentList.Add(segmentBytes);
					}
					// didn't find the one we're looking for, loop through to the next segment
				} while (true);
			} 
			catch (IOException ioe) 
			{
				//throw new JpegProcessingException("IOException processing Jpeg file", ioe);
				throw new JpegProcessingException(
					"IOException processing Jpeg file: " + ioe.Message,
					ioe);
			} 
			finally 
			{
				try 
				{
					if (inStream != null) 
					{
						inStream.Close();
					}
				} 
				catch (IOException ioe) 
				{
					//throw new JpegProcessingException("IOException processing Jpeg file", ioe);
					throw new JpegProcessingException(
						"IOException processing Jpeg file: " + ioe.Message,
						ioe);
				}
			}
		}

		/// <summary>
		/// Private helper method to create a BufferedInputStream of Jpeg data 
		/// from whichever data source was specified upon construction of this instance. 
		/// </summary>
		/// <returns>a a BufferedStream of Jpeg data</returns>
		/// <exception cref="JpegProcessingException">for any problems processing the Jpeg data</exception>
		private BufferedStream GetJpegInputStream() 
		{
			if (stream != null) 
			{
				if (stream is BufferedStream) 
				{
					return (BufferedStream) stream;
				} 
				else 
				{
					return new BufferedStream(stream);
				}
			}
			Stream inputStream = null;
			if (data == null) 
			{
				try 
				{
					// Added read only access for ASPX use, thanks for Ryan Patridge
					inputStream = file.Open(FileMode.Open, FileAccess.Read);
				} 
				catch (FileNotFoundException e) 
				{
					throw new JpegProcessingException(
						"Jpeg file \""+file.FullName+"\" does not exist",
						e);
				}
			} 
			else 
			{
				inputStream = new MemoryStream(data);
			}
			return new BufferedStream(inputStream);
		}

		/// <summary>
		/// Helper method that validates the Jpeg file's magic number. 
		/// </summary>
		/// <param name="fileStream">the InputStream to read bytes from, which must be positioned at its start (i.e. no bytes read yet)</param>
		/// <returns>true if the magic number is Jpeg (0xFFD8)</returns>
		/// <exception cref="JpegProcessingException">for any problems processing the Jpeg data</exception>
		private bool IsValidJpegHeaderBytes(BufferedStream fileStream) 
		{
			byte[] header = new byte[2];
			fileStream.Read(header, 0, 2);
			return ((header[0] & 0xFF) == 0xFF && (header[1] & 0xFF) == 0xD8);
		}
	}
}