using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using com.drew.lang;
using com.drew.metadata.iptc;
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
namespace com.drew.metadata
{
	/// <summary>
	/// Base class for all Metadata directory types with supporting 
	/// methods for setting and getting tag values.
	/// </summary>
	[Serializable]
	public abstract class AbstractDirectory 
	{
		/// <summary>
		/// Map of values hashed by type identifiers. 
		/// </summary>
		protected Dictionary<int, object> tagMap;

		/// <summary>
		/// The descriptor used to interperet tag values.
		/// </summary>
		protected AbstractTagDescriptor descriptor;

		/// <summary>
		/// A convenient list holding tag values in the order in which they were stored. This is used for creation of an iterator, and for counting the number of defined tags. 
		/// </summary>
		protected List<Tag> definedTagList;

		private List<string> errorList;


		/// <summary>
		/// Creates a new Directory. 
		/// </summary>
		public AbstractDirectory() : base()
		{
            this.tagMap = new Dictionary<int, object>();
            this.definedTagList = new List<Tag>();
            this.errorList = new List<string>(0);
		}

		/// <summary>
		/// Indicates whether the specified tag type has been set. 
		/// </summary>
		/// <param name="aTagType">the tag type to check for</param>
		/// <returns>true if a value exists for the specified tag type, false if not</returns>
		public bool ContainsTag(int aTagType) 
		{
            return this.tagMap.ContainsKey(aTagType);
		}

		/// <summary>
		/// Returns an Iterator of Tag instances that have been set in this Directory. 
		/// </summary>
		/// <returns>an Iterator of Tag instances</returns>
		public IEnumerator<Tag> GetTagIterator() 
		{
            return this.definedTagList.GetEnumerator();
		}

		/// <summary>
		/// Returns the number of tags set in this Directory. 
		/// </summary>
		/// <returns>the number of tags set in this Directory</returns>
		public int GetTagCount() 
		{
            return this.definedTagList.Count;
		}

		/// <summary>
		/// Sets the descriptor used to interperet tag values. 
		/// </summary>
		/// <param name="aDescriptor">the descriptor used to interperet tag values</param>
		/// <exception cref="NullReferenceException">if aDescriptor is null</exception>
		public void SetDescriptor(AbstractTagDescriptor aDescriptor) 
		{
			if (aDescriptor == null) 
			{
				throw new NullReferenceException("cannot set a null descriptor");
			}
            this.descriptor = aDescriptor;
		}

		/// <summary>
		/// Adds an error
		/// </summary>
		/// <param name="aMessage">the error aMessage</param>
		public void AddError(string aMessage) 
		{
            this.errorList.Add(aMessage);
		}

		/// <summary>
		/// Checks if there is error
		/// </summary>
		/// <returns>true if there is error, false otherwise</returns>
		public bool HasErrors() 
		{
            return this.errorList.Count > 0;
		}

		/// <summary>
		/// Gets an enumerator upon errors
		/// </summary>
		/// <returns>en enumerator for errors</returns>
		public IEnumerator<string> GetErrors() 
		{
            return this.errorList.GetEnumerator();
		}

		/// <summary>
		/// Gives the number of errors
		/// </summary>
		/// <returns>the number of errors</returns>
		public int GetErrorCount() 
		{
            return this.errorList.Count;
		}

		/// <summary>
		/// Sets an int array for the specified tag. 
		/// </summary>
		/// <param name="aTagType">the tag identifier</param>
		/// <param name="someInts">the int array to store</param>
		public virtual void SetIntArray(int aTagType, int[] someInts) 
		{
            this.SetObject(aTagType, someInts);
		}

		/// <summary>
		/// Helper method, containing common functionality for all 'add' methods.
		/// </summary>
		/// <param name="aTagType">the tag value as an int</param>
		/// <param name="aValue">the value for the specified tag</param>
		/// <exception cref="NullReferenceException">if aValue is null</exception>
		public void SetObject(int aTagType, object aValue) 
		{
			if (aValue == null) 
			{
				throw new NullReferenceException("cannot set a null object");
			}

            if (!this.tagMap.ContainsKey(aTagType)) 
			{
                this.tagMap.Add(aTagType, aValue);
                this.definedTagList.Add(new Tag(aTagType, this));
			} 
			else 
			{
				// We remove it and re-add it with the new value
                this.tagMap.Remove(aTagType);
                this.tagMap.Add(aTagType, aValue);
			}
		}

		/// <summary>
		/// Returns the specified tag value as an int, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as an int, if possible.</returns>
		/// <exception cref="MetadataException">if tag not found</exception>
        public int GetInt(int aTagType)
        {
            object lcObj = this.GetObject(aTagType);
            if (lcObj == null)
            {
                throw new MetadataException(
                    "Tag "
                    + GetTagName(aTagType)
                    + " has not been set -- check using containsTag() first");
            }
            else if (lcObj is string)
            {
                try
                {
                    return Convert.ToInt32((string)lcObj);
                }
                catch (FormatException)
                {
                    // convert the char array to an int
                    string lcStr = (string)lcObj;
                    int lcVal = 0;
                    for (int i = lcStr.Length - 1; i >= 0; i--)
                    {
                        lcVal += lcStr[i] << (i * 8);
                    }
                    return lcVal;
                }
            }
            else if (lcObj is Rational)
            {
                return ((Rational)lcObj).IntValue();
            }
            else if (lcObj is byte[])
            {
                byte[] lcTab = (byte[])lcObj;
                if (lcTab.Length >= 0)
                {
                    return (int)lcTab[0];
                }

            }
            else if (lcObj is int || lcObj is byte || lcObj is long || lcObj is float || lcObj is double)
            {
                try
                {
                    return Convert.ToInt32(lcObj);
                }
                catch (FormatException e)
                {
                    throw new MetadataException("Unable to parse as int object of type:'" + lcObj.GetType().Name + "' that look like:'" + lcObj.ToString() + "'", e);
                }
            }
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Gets the specified tag value as a string array, if possible.  Only supported where the tag is set as string[], string, int[], byte[] or Rational[].
		/// </summary>
		/// <param name="aTagType">the tag identifier</param>
		/// <returns>the tag value as an array of Strings</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a string[]</exception>
		public string[] GetStringArray(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is string[]) 
			{
				return (string[]) lcObj;
			} 
			else if (lcObj is string) 
			{
                return new string[] { (string)lcObj };
			} 
			else if (lcObj is int[]) 
			{
				int[] lcInts = (int[]) lcObj;
				string[] lcStrings = new string[lcInts.Length];
				for (int i = 0; i < lcStrings.Length; i++) 
				{
					lcStrings[i] = lcInts[i].ToString();
				}
				return lcStrings;
			} 
			else if (lcObj is byte[]) 
			{
				byte[] lcBytes = (byte[]) lcObj;
				string[] lcStrings = new string[lcBytes.Length];
				for (int i = 0; i < lcStrings.Length; i++) 
				{
					lcStrings[i] = lcBytes[i].ToString();
				}
				return lcStrings;
			} 
			else if (lcObj is Rational[]) 
			{
				Rational[] lcRationals = (Rational[]) lcObj;
				string[] lcStrings = new string[lcRationals.Length];
				for (int i = 0; i < lcStrings.Length; i++) 
				{
					lcStrings[i] = lcRationals[i].ToSimpleString(false);
				}
				return lcStrings;
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Gets the specified tag value as an int array, if possible.  Only supported where the tag is set as string, int[], byte[] or Rational[]. 
		/// </summary>
		/// <param name="aTagType">the tag identifier</param>
		/// <returns>the tag value as an int array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a int[]</exception>
		public int[] GetIntArray(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Rational[]) 
			{
				Rational[] lcRationals = (Rational[]) lcObj;
				int[] lcInts = new int[lcRationals.Length];
				for (int i = 0; i < lcInts.Length; i++) 
				{
					lcInts[i] = lcRationals[i].IntValue();
				}
				return lcInts;
			} 
			else if (lcObj is int[]) 
			{
				return (int[]) lcObj;
			} 
			else if (lcObj is byte[]) 
			{
				byte[] lcBytes = (byte[]) lcObj;
				int[] lcInts = new int[lcBytes.Length];
				for (int i = 0; i < lcBytes.Length; i++) 
				{
					lcInts[i] = lcBytes[i];
				}
				return lcInts;
			} 
			else if (lcObj is string) 
			{
				string lcStr = (string) lcObj;
				int[] lcInts = new int[lcStr.Length];
				for (int i = 0; i < lcStr.Length; i++) 
				{
					lcInts[i] = lcStr[i];
				}
				return lcInts;
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Gets the specified tag value as an byte array, if possible.  Only supported where the tag is set as string, int[], byte[] or Rational[].
		/// </summary>
		/// <param name="aTagType">the tag identifier</param>
		/// <returns>the tag value as a byte array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a byte[]</exception>
		public byte[] GetByteArray(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Rational[]) 
			{
				Rational[] lcRationals = (Rational[]) lcObj;
				byte[] lcBytes = new byte[lcRationals.Length];
				for (int i = 0; i < lcBytes.Length; i++) 
				{
					lcBytes[i] = lcRationals[i].ByteValue();
				}
				return lcBytes;
			} 
			else if (lcObj is byte[]) 
			{
				return (byte[]) lcObj;
			} 
			else if (lcObj is int[]) 
			{
				int[] lcInts = (int[]) lcObj;
				byte[] lcBytes = new byte[lcInts.Length];
				for (int i = 0; i < lcInts.Length; i++) 
				{
					lcBytes[i] = (byte) lcInts[i];
				}
				return lcBytes;
			} 
			else if (lcObj is string) 
			{
				string lcStr = (string) lcObj;
				byte[] lcBytes = new byte[lcStr.Length];
				for (int i = 0; i < lcStr.Length; i++) 
				{
					lcBytes[i] = (byte) lcStr[i];
				}
				return lcBytes;
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Returns the specified tag value as a double, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a double, if possible.</returns>
        public double GetDouble(int aTagType)
        {
            object lcObj = GetObject(aTagType);
            if (lcObj == null)
            {
                throw new MetadataException(
                    "Tag "
                    + GetTagName(aTagType)
                    + " has not been set -- check using containsTag() first");
            }
            else if (lcObj is Rational)
            {
                return ((Rational)lcObj).DoubleValue();
            }
            else if (lcObj is double || lcObj is string || lcObj is int || lcObj is byte || lcObj is long || lcObj is float)
            {
                try
                {
                    return Convert.ToDouble(lcObj);
                }
                catch (FormatException e)
                {
                    throw new MetadataException("Unable to parse as double object of type:'" + lcObj.GetType().Name + "' that look like:'" + lcObj.ToString() + "'", e);
                }
            }
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Returns the specified tag value as a float, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a float, if possible.</returns>
		public float GetFloat(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Rational) 
			{		
				return ((Rational)lcObj).FloatValue();
			}
            else if (lcObj is float || lcObj is string || lcObj is int || lcObj is byte || lcObj is long || lcObj is double)
            {
                try
                {
                    return (float)Convert.ToDouble(lcObj);
                }
                catch (FormatException e)
                {
                    throw new MetadataException("Unable to parse as float object of type:'" + lcObj.GetType().Name + "' that look like:'" + lcObj.ToString() + "'", e);
                }
            }
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Returns the specified tag value as a long, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a long, if possible.</returns>
		public long GetLong(int aTagType)  
		{
            object lcObj = GetObject(aTagType);
            if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			}
            else if (lcObj is Rational)
            {
                return ((Rational)lcObj).LongValue();
            }
            else if (lcObj is long || lcObj is string || lcObj is int || lcObj is byte || lcObj is double || lcObj is double)
            {
                try
                {
                    return Convert.ToInt64(lcObj);
                }
                catch (FormatException e)
                {
                    throw new MetadataException("Unable to parse as long object of type:'" + lcObj.GetType().Name + "' that look like:'" + lcObj.ToString() + "'", e);
                }
            }
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
		}

		/// <summary>
		/// Returns the specified tag value as a boolean, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a boolean, if possible.</returns>
		public bool GetBoolean(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Boolean) 
			{
				return ((Boolean) lcObj);
			} 
			else if (lcObj is string) 
			{
				try 
				{
					return Convert.ToBoolean((string) lcObj);
				} 
				catch (FormatException e) 
				{
                    throw new MetadataException("Unable to parse as boolean object of type:'" + lcObj.GetType().Name + "' that look like:'" + lcObj.ToString() + "'", e);
                }
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
		}

		/// <summary>
		/// Returns the specified tag value as a date, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a date, if possible.</returns>
		public DateTime GetDate(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is DateTime) 
			{
				return (DateTime) lcObj;
			} 
			else if (lcObj is string) 
			{
				string lcDateString = (string) lcObj;
				try 
				{
					return DateTime.Parse(lcDateString);
				} 
				catch (FormatException ex) 
				{
					Console.Error.WriteLine(ex.StackTrace);
				}
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
		}

		/// <summary>
		/// Returns the specified tag value as a rational, if possible. 
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the specified tag value as a rational, if possible.</returns>
		public Rational GetRational(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Rational) 
			{
				return (Rational) lcObj;
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
		}

		/// <summary>
		/// Gets the specified tag value as a rational array, if possible.  Only supported where the tag is set as Rational[].
		/// </summary>
		/// <param name="aTagType">the tag identifier</param>
		/// <returns>the tag value as a rational array</returns>
		/// <exception cref="MetadataException">if tag not found or if it cannot be represented as a rational[]</exception>
		public Rational[] GetRationalArray(int aTagType) 
		{
			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				throw new MetadataException(
					"Tag "
					+ GetTagName(aTagType)
					+ " has not been set -- check using containsTag() first");
			} 
			else if (lcObj is Rational[]) 
			{
				return (Rational[]) lcObj;
			}
            throw new MetadataException("Obj is :" + lcObj.GetType().Name + " and look like:" + lcObj.ToString());
        }

		/// <summary>
		/// Returns the specified tag value as a string.
		/// This value is the 'raw' value.
		/// A more presentable decoding of this value may be obtained from the corresponding Descriptor.
		/// </summary>
		/// <param name="aTagType">the specified tag type</param>
		/// <returns>the string reprensentation of the tag value, or null if the tag hasn't been defined.</returns>
		public string GetString(int aTagType) 
		{

			object lcObj = GetObject(aTagType);
			if (lcObj == null) 
			{
				return null;
			} 
			else if (lcObj is Rational) 
			{
				return ((Rational) lcObj).ToSimpleString(true);
			} 
			else if (lcObj.GetType().IsArray) 
			{
				string lcStr = lcObj.GetType().ToString();

				int lcArrayLength = 0;
				
				if (lcStr.IndexOf("Int")!=-1)  
				{
					// handle arrays of objects and primitives
					lcArrayLength = ((int[])lcObj).Length;
				} 
				else if (lcStr.IndexOf("Rational")!=-1)  
				{
					lcArrayLength = ((Rational[])lcObj).Length;
				} 
				else  if (lcStr.IndexOf("string")!=-1 || lcStr.IndexOf("String")!=-1)
				{
					lcArrayLength = ((string[])lcObj).Length;
				}
								
				StringBuilder lcBuff = new StringBuilder();
				for (int i = 0; i < lcArrayLength; i++) 
				{
					if (i != 0) 
					{
						lcBuff.Append(' ');
					}
					if (lcStr.IndexOf("Int")!=-1)  
					{
						lcBuff.Append(((int[])lcObj)[i].ToString());					
					} 
					else if (lcStr.IndexOf("Rational")!=-1)  
					{
						lcBuff.Append(((Rational[])lcObj)[i].ToString());
					} 
					else if (lcStr.IndexOf("string")!=-1 || lcStr.IndexOf("String")!=-1)
					{
						lcBuff.Append(((string[])lcObj)[i].ToString());
					}
				}
				return lcBuff.ToString();
			} 
			return lcObj.ToString();
		}

		/// <summary>
		/// Returns the object hashed for the particular tag type specified, if available. 
		/// </summary>
		/// <param name="aTagType">the tag type identifier</param>
		/// <returns>the tag value as an object if available, else null</returns>
		public object GetObject(int aTagType) 
		{
            if (this.tagMap.ContainsKey(aTagType))
            {
                return this.tagMap[aTagType];
            }
            return null;
		}

		/// <summary>
		/// Returns the name of a specified tag as a string. 
		/// </summary>
		/// <param name="aTagType">the tag type identifier</param>
		/// <returns>the tag name as a string</returns>
		public string GetTagName(int aTagType) 
		{		
			Dictionary<int, string> lcNameMap = this.GetTagNameMap();
			if (!lcNameMap.ContainsKey(aTagType)) 
			{
				string lcHex =  aTagType.ToString("X");
				while (lcHex.Length < 4) 
				{
					lcHex = "0" + lcHex;
				}
				return "Unknown tag (0x" + lcHex + ")";
			}
			return lcNameMap[aTagType];
		}

		/// <summary>
		/// Provides a description of a tag value using the descriptor set by setDescriptor(Descriptor). 
		/// </summary>
		/// <param name="aTagType">the tag type identifier</param>
		/// <returns>the tag value'str description as a string</returns>
		/// <exception cref="MetadataException">if a descriptor hasn't been set, or if an error occurs during calculation of the description within the Descriptor</exception>
		public string GetDescription(int aTagType) 
		{
			if (this.descriptor == null) 
			{
				throw new MetadataException("A descriptor must be set using setDescriptor(...) before descriptions can be provided");
			}

			return this.descriptor.GetDescription(aTagType);
		}

        /// <summary>
        /// Provides the name of the directory, for display purposes.  E.g. Exif 
        /// </summary>
        /// <returns>the name of the directory</returns>
        public abstract string GetName();

        /// <summary>
        /// Provides the map of tag names, hashed by tag type identifier. 
        /// </summary>
        /// <returns>the map of tag names</returns>
        protected abstract Dictionary<int, string> GetTagNameMap();

        /// <summary>
        /// Fill the map with all (TAG_xxx value, BUNDLE[TAG_xxx name]).
        /// </summary>
        /// <param name="aType">where to look for fields like TAG_xxx</param>
        /// <param name="aTagMap">where to put tag found</param>
        protected static Dictionary<int, string> FillTagMap(Type aType,  ResourceBundle aBundle)
        {
            FieldInfo[] lcAllContTag = aType.GetFields();
            Dictionary<int, string> lcResu = new Dictionary<int, string>(lcAllContTag.Length);
            for (int i = 0; i < lcAllContTag.Length; i++)
            {
                string lcMemberName = lcAllContTag[i].Name;
                if (lcAllContTag[i].IsPublic && lcMemberName.StartsWith("TAG_"))
                {
                    int lcMemberValue = (int)lcAllContTag[i].GetValue(null);
                    try
                    {
                        lcResu.Add(lcMemberValue, aBundle[lcMemberName]);
                    }
                    catch (MissingResourceException mre)
                    {
                        Console.Error.WriteLine(mre.Message);
                    }
                }
            }
            return lcResu;
        }
	}
}