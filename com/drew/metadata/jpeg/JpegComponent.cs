using System;
using System.Collections;
using System.IO;
using com.drew.metadata;


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
	/// The Jpeg component class
	/// </summary>
	[Serializable]
	public class JpegComponent 
	{
		private int _componentId;
		private int _samplingFactorByte;
		private int _quantizationTableNumber;

		/// <summary>
		/// The constructor of the object
		/// </summary>
		/// <param name="componentId">the component id</param>
		/// <param name="samplingFactorByte">the sampling lcFactor byte</param>
		/// <param name="quantizationTableNumber">the quantization table number</param>
		public JpegComponent(
			int componentId,
			int samplingFactorByte,
			int quantizationTableNumber) : base()
		{
			_componentId = componentId;
			_samplingFactorByte = samplingFactorByte;
			_quantizationTableNumber = quantizationTableNumber;
		}

		/// <summary>
		/// Gets the component id
		/// </summary>
		/// <returns>the component id</returns>
		public int GetComponentId() 
		{
			return _componentId;
		}

		/// <summary>
		/// The component name
		/// </summary>
		/// <returns>The component name</returns>
		public string GetComponentName() 
		{
			switch (_componentId) 
			{
				case 1 :
					return "Y";
				case 2 :
					return "Cb";
				case 3 :
					return "Cr";
				case 4 :
					return "I";
				case 5 :
					return "Q";
			}

			throw new MetadataException("Unsupported component id: " + _componentId);
		}

		/// <summary>
		/// Gets the Quantization Table Number
		/// </summary>
		/// <returns>the Quantization Table Number</returns>
		public int GetQuantizationTableNumber() 
		{
			return _quantizationTableNumber;
		}

		/// <summary>
		/// Gets the Horizontal Sampling Factor
		/// </summary>
		/// <returns>the Horizontal Sampling Factor</returns>
		public int GetHorizontalSamplingFactor() 
		{
			return _samplingFactorByte & 0x0F;
		}

		/// <summary>
		/// Gets the Vertical Sampling Factor
		/// </summary>
		/// <returns>the Vertical Sampling Factor</returns>
		public int GetVerticalSamplingFactor() 
		{
			return (_samplingFactorByte >> 4) & 0x0F;
		}
	}
}
