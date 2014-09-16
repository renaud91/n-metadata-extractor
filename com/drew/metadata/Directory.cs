using System;
using System.IO;
using System.Text;
using System.Collections;
using com.drew.lang;

using com.drew.metadata.iptc;

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
namespace com.drew.metadata
{
	/// <summary>
	/// Base class for all Metadata directory types with supporting 
	/// methods for setting and getting tag values.
	/// </summary>
	[Serializable]
	public abstract class Directory 
	{
		/// <summary>
		/// Map of values hashed by type identifiers. 
		/// </summary>
		protected IDictionary _tagMap;

		/// <summary>
		/// The descriptor used to interperet tag values.
		/// </summary>
		protected TagDescriptor _descriptor;

		/// <summary>
		/// A convenient list holding tag values in the order in which they were stored. This is used for creation of an iterator, and for counting the number of defined tags. 
		/// </summary>
		protected IList _definedTagList;

		private IList _errorList;

		/// <summary>
		/// Provides the name of the directory, for display purposes.  E.g. Exif 
		/// </summary>
		/// <returns>the name of the directory</returns>
		public abstract string GetName();

		/// <summary>
		/// Provides the map of tag names, hashed by tag type identifier. 
		/// </summary>
		/// <returns>the map of tag names</returns>
		protected abstract IDictionary GetTagNameMap();

		/// <summary>
		/// Creates a new Directory. 
		/// </summary>
		public Directory() : base()
		{
			_tagMap = new Hashtable();
			_definedTagList = new ArrayList();
			_errorList = new ArrayList(0);
		}

		/// <summary>
		/// Indicates whether the specified tag type has been set. 
		/// </summary>
		/// <param name="tagType">the tag type to check for</param>
		/// <returns>true if a value exists for the specified tag type, false if not</returns>
		public bool ContainsTag(int tagType) 
		{
			return _tagMap.Contains(tagType);
		}

		/// <summary>
		/// Returns an Iterator of Tag instances that have been set in this Directory. 
		/// </summary>
		/// <returns>an Iterator of Tag instances</returns>
		public IEnumerator GetTagIterator() 
		{
			return _definedTagList.GetEnumerator();
		}

		/// <summary>
		/// Returns the number of tags set in this Directory. 
		/// </summary>
		/// <returns>the number of tags set in this Directory</returns>
		public int GetTagCount() 
		{
			return _definedTagList.Count;
		}

		/// <summary>
		/// Sets the descriptor used to interperet tag values. 
		/// </summary>
		/// <param name="aDescriptor">the descriptor used to interperet tag values</param>
		/// <exception cref="NullReferenceException">if aDescriptor is null</exception>
		public void SetDescriptor(TagDescriptor aDescriptor) 
		{
			if (aDescriptor == null) 
			{
				throw new NullReferenceException("cannot set a null descriptor");
			}
			_descriptor = aDescriptor;
		}

		/// <summary>
		/// Adds an error
		/// </summary>
		/// <param name="message">the error message</param>
		public void AddError(string message) 
		{
			_errorList.Add(message);
		}

		/// <summary>
		/// Checks if there is error
		/// </summary>
		/// <returns>true if there is error, false otherwise</returns>
		public bool HasErrors() 
		{
			return (_errorList.Count > 0);
		}

		/// <summary>
		/// Gets an enumerator upon errors
		/// </summary>
		/// <returns>en enumerator for errors</returns>
		public IEnumerator GetErrors() 
		{
			return _errorList.GetEnumerator();
		}

		/// <summary>
		/// Gives the number of errors
		/// </summary>
		/// <returns>the number of errors</returns>
		public int GetErrorCount() 
		{
			return _errorList.Count;
		}

		/// <summary>
		/// Sets an int array for the specified tag. 
		/// </summary>
		/// <param name="tagType">the tag identifier</param>
		/// <param name="ints">the int array to store</param>
		public virtual void SetIntArray(int tagType, int[] ints) 
		{
			SetObject(tagType, ints);
		}

		/// <summary>
		/// Helper method, containing common functionality for all 'add' methods.
		/// </summary>
		/// <param name="tagType">the tag's value as an int</param>
		/// <param name="aValue">the value for the specified tag</param>
		/// <exception cref="NullReferenceException">if aValue is null</exception>
		public void SetObject(int tagType, object aValue) 
		{
			if (aValue == null) 
			{
				throw new NullReferenceException("cannot set a null object");
			}

			if (!_tagMap.Contains(tagType)) 
			{
				_tagMap.Add(tagType, aValue);
				_definedTagList.Add(new Tag(tagType, this));
			} 
			else 
			{
				// We remove it and re-add it with the new value
				_tagMap.Remove(tagType);
				_tagMap.Add(tagType, aValue);
			}
		}

		/// <summary>
		/// Returns the specified tag's value as an int, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as an int, if possible.</returns>
		/// <exception cref="MetadataException">if tag not found</exception>
		public int GetInt(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is string) 
			{
				try 
				{
					return Convert.ToInt32((string) o);
				} 
				catch (FormatException ) 
				{
					// convert the char array to an int
					string s = (string) o;
					int val = 0;
					for (int i = s.Length - 1; i >= 0; i--) 
					{
						val += s[i] << (i * 8);
					}
					return val;
				}
			} 
			else if (o is Rational) 
			{
				return ((Rational)o).IntValue();
			}
			return (int)o;
		}

		/// <summary>
		/// Gets the specified tag's value as a string array, if possible.  Only supported where the tag is set as string[], string, int[], byte[] or Rational[].
		/// </summary>
		/// <param name="tagType">the tag identifier</param>
		/// <returns>the tag's value as an array of Strings</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a string[]</exception>
		public string[] GetStringArray(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is string[]) 
			{
				return (string[]) o;
			} 
			else if (o is string) 
			{
				string[] strings = {(string) o };
				return strings;
			} 
			else if (o is int[]) 
			{
				int[] ints = (int[]) o;
				string[] strings = new string[ints.Length];
				for (int i = 0; i < strings.Length; i++) 
				{
					strings[i] = ints[i].ToString();
				}
				return strings;
			} 
			else if (o is byte[]) 
			{
				byte[] bytes = (byte[]) o;
				string[] strings = new string[bytes.Length];
				for (int i = 0; i < strings.Length; i++) 
				{
					strings[i] = bytes[i].ToString();
				}
				return strings;
			} 
			else if (o is Rational[]) 
			{
				Rational[] rationals = (Rational[]) o;
				string[] strings = new string[rationals.Length];
				for (int i = 0; i < strings.Length; i++) 
				{
					strings[i] = rationals[i].ToSimpleString(false);
				}
				return strings;
			}
			throw new MetadataException(
				"Requested tag cannot be cast to string array ("
				+ o.GetType().ToString()
				+ ")");
		}

		/// <summary>
		/// Gets the specified tag's value as an int array, if possible.  Only supported where the tag is set as string, int[], byte[] or Rational[]. 
		/// </summary>
		/// <param name="tagType">the tag identifier</param>
		/// <returns>the tag's value as an int array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a int[]</exception>
		public int[] GetIntArray(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is Rational[]) 
			{
				Rational[] rationals = (Rational[]) o;
				int[] ints = new int[rationals.Length];
				for (int i = 0; i < ints.Length; i++) 
				{
					ints[i] = rationals[i].IntValue();
				}
				return ints;
			} 
			else if (o is int[]) 
			{
				return (int[]) o;
			} 
			else if (o is byte[]) 
			{
				byte[] bytes = (byte[]) o;
				int[] ints = new int[bytes.Length];
				for (int i = 0; i < bytes.Length; i++) 
				{
					byte b = bytes[i];
					ints[i] = b;
				}
				return ints;
			} 
			else if (o is string) 
			{
				string str = (string) o;
				int[] ints = new int[str.Length];
				for (int i = 0; i < str.Length; i++) 
				{
					ints[i] = str[i];
				}
				return ints;
			}
			throw new MetadataException(
				"Requested tag cannot be cast to int array ("
				+ o.GetType().ToString()
				+ ")");
		}

		/// <summary>
		/// Gets the specified tag's value as an byte array, if possible.  Only supported where the tag is set as string, int[], byte[] or Rational[].
		/// </summary>
		/// <param name="tagType">the tag identifier</param>
		/// <returns>the tag's value as a byte array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a byte[]</exception>
		public byte[] GetByteArray(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is Rational[]) 
			{
				Rational[] rationals = (Rational[]) o;
				byte[] bytes = new byte[rationals.Length];
				for (int i = 0; i < bytes.Length; i++) 
				{
					bytes[i] = rationals[i].ByteValue();
				}
				return bytes;
			} 
			else if (o is byte[]) 
			{
				return (byte[]) o;
			} 
			else if (o is int[]) 
			{
				int[] ints = (int[]) o;
				byte[] bytes = new byte[ints.Length];
				for (int i = 0; i < ints.Length; i++) 
				{
					bytes[i] = (byte) ints[i];
				}
				return bytes;
			} 
			else if (o is string) 
			{
				string str = (string) o;
				byte[] bytes = new byte[str.Length];
				for (int i = 0; i < str.Length; i++) 
				{
					bytes[i] = (byte) str[i];
				}
				return bytes;
			}
			throw new MetadataException(
				"Requested tag cannot be cast to byte array ("
				+ o.GetType().ToString()
				+ ")");
		}

		/// <summary>
		/// Returns the specified tag's value as a double, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a double, if possible.</returns>
		public double GetDouble(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is string) 
			{
				try 
				{
					return Convert.ToDouble((string) o);
				} 
				catch (FormatException nfe) 
				{
					throw new MetadataException(
						"unable to parse string " + o + " as a double",
						nfe);
				}
			} 
			else if (o is Rational) 
			{		
				return ((Rational)o).DoubleValue();
			}
			return (double)o;
		}

		/// <summary>
		/// Returns the specified tag's value as a float, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a float, if possible.</returns>
		public float GetFloat(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is string) 
			{
				try 
				{
					return (float)Convert.ToDouble((string) o);
				} 
				catch (FormatException nfe) 
				{
					throw new MetadataException(
						"unable to parse string " + o + " as a float",
						nfe);
				}
			} 		
			else if (o is Rational) 
			{		
				return ((Rational)o).FloatValue();
			}

			return (float)o;
		}

		/// <summary>
		/// Returns the specified tag's value as a long, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a long, if possible.</returns>
		public long GetLong(int tagType)  
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is string) 
			{
				try 
				{
					return Convert.ToInt64((string) o);
				} 
				catch (FormatException nfe) 
				{
					throw new MetadataException(
						"unable to parse string " + o + " as a long",
						nfe);
				}
			}
			else if (o is Rational) 
			{		
				return ((Rational)o).LongValue();
			}

			return (long)o;
		}

		/// <summary>
		/// Returns the specified tag's value as a boolean, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a boolean, if possible.</returns>
		public bool GetBoolean(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is Boolean) 
			{
				return ((Boolean) o);
			} 
			else if (o is string) 
			{
				try 
				{
					return Convert.ToBoolean((string) o);
				} 
				catch (FormatException nfe) 
				{
					throw new MetadataException(
						"unable to parse string " + o + " as a bool",
						nfe);
				}
			} 
			return (bool) o;
		}

		/// <summary>
		/// Returns the specified tag's value as a date, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a date, if possible.</returns>
		public DateTime GetDate(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is DateTime) 
			{
				return (DateTime) o;
			} 
			else if (o is string) 
			{
				string dateString = (string) o;
				try 
				{
					return DateTime.Parse(dateString);
				} 
				catch (FormatException ex) 
				{
					Console.Error.WriteLine(ex.StackTrace);
				}
			}
			throw new MetadataException("Requested tag cannot be cast to java.util.Date");
		}

		/// <summary>
		/// Returns the specified tag's value as a rational, if possible. 
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the specified tag's value as a rational, if possible.</returns>
		public Rational GetRational(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is Rational) 
			{
				return (Rational) o;
			}
			throw new MetadataException("Requested tag cannot be cast to Rational");
		}

		/// <summary>
		/// Gets the specified tag's value as a rational array, if possible.  Only supported where the tag is set as Rational[].
		/// </summary>
		/// <param name="tagType">the tag identifier</param>
		/// <returns>the tag's value as a rational array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a rational[]</exception>
		public Rational[] GetRationalArray(int tagType) 
		{
			object o = GetObject(tagType);
			if (o == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(tagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (o is Rational[]) 
			{
				return (Rational[]) o;
			}
			throw new MetadataException(
				"Requested tag cannot be cast to Rational array ("
				+ o.GetType().ToString()
				+ ")");
		}

		/// <summary>
		/// Returns the specified tag's value as a string.
		/// This value is the 'raw' value.
		/// A more presentable decoding of this value may be obtained from the corresponding Descriptor.
		/// </summary>
		/// <param name="tagType">the specified tag type</param>
		/// <returns>the string reprensentation of the tag's value, or null if the tag hasn't been defined.</returns>
		public string GetString(int tagType) 
		{

			object o = GetObject(tagType);
			if (o == null) 
			{
				return null;
			} 
			else if (o is Rational) 
			{
				return ((Rational) o).ToSimpleString(true);
			} 
			else if (o.GetType().IsArray) 
			{
				string s = o.GetType().ToString();

				int arrayLength = 0;
				
				if (s.IndexOf("Int")!=-1)  
				{
					// handle arrays of objects and primitives
					arrayLength = ((int[])o).Length;
				} 
				else if (s.IndexOf("Rational")!=-1)  
				{
					arrayLength = ((Rational[])o).Length;
				} 
				else  if (s.IndexOf("string")!=-1 || s.IndexOf("String")!=-1)
				{
					arrayLength = ((string[])o).Length;
				}
								
				StringBuilder sbuffer = new StringBuilder();
				for (int i = 0; i < arrayLength; i++) 
				{
					if (i != 0) 
					{
						sbuffer.Append(' ');
					}
					if (s.IndexOf("Int")!=-1)  
					{
						sbuffer.Append(((int[])o)[i].ToString());					
					} 
					else if (s.IndexOf("Rational")!=-1)  
					{
						sbuffer.Append(((Rational[])o)[i].ToString());
					} 
					else if (s.IndexOf("string")!=-1 || s.IndexOf("String")!=-1)
					{
						sbuffer.Append(((string[])o)[i].ToString());
					}
				}
				return sbuffer.ToString();
			} 
			else 
			{
				return o.ToString();
			}
		}

		/// <summary>
		/// Returns the object hashed for the particular tag type specified, if available. 
		/// </summary>
		/// <param name="tagType">the tag type identifier</param>
		/// <returns>the tag's value as an object if available, else null</returns>
		public object GetObject(int tagType) 
		{
			return _tagMap[tagType];
		}

		/// <summary>
		/// Returns the name of a specified tag as a string. 
		/// </summary>
		/// <param name="tagType">the tag type identifier</param>
		/// <returns>the tag's name as a string</returns>
		public string GetTagName(int tagType) 
		{		
			IDictionary nameMap = GetTagNameMap();
			if (!nameMap.Contains(tagType)) 
			{
				string hex =  tagType.ToString("X");
				while (hex.Length < 4) 
				{
					hex = "0" + hex;
				}
				return "Unknown tag (0x" + hex + ")";
			}
			return (string) nameMap[tagType];
		}

		/// <summary>
		/// Provides a description of a tag's value using the descriptor set by setDescriptor(Descriptor). 
		/// </summary>
		/// <param name="tagType">the tag type identifier</param>
		/// <returns>the tag value's description as a string</returns>
		/// <exception cref="MetadataException">if a descriptor hasn't been set, or if an error occurs during calculation of the description within the Descriptor</exception>
		public string GetDescription(int tagType) 
		{
			if (_descriptor == null) 
			{
				throw new MetadataException("a descriptor must be set using setDescriptor(...) before descriptions can be provided");
			}

			return _descriptor.GetDescription(tagType);
		}
	}
}