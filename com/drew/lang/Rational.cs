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
namespace com.drew.lang 
{
	/// <summary>
	/// Created on 6 May 2002, 18:06
	/// Updated 26 Aug 2002 by Drew
	/// - Added toSimpleString() method, which returns a simplified and hopefully 
	///   more readable version of the Rational.  i.e. 2/10 -> 1/5, and 10/2 -> 5
	/// Modified 29 Oct 2002 (v1.2)
	/// - Improved toSimpleString() to factor more complex rational numbers into 
	///   a simpler form i.e. 10/15 -> 2/3
	/// - toSimpleString() now accepts a boolean flag, 'allowDecimals' which 
	///   will display the rational number in decimal form if it fits within 5 
	///   digits i.e. 3/4 -> 0.75 when allowDecimal == true
	/// </summary>
	[Serializable]
	public class Rational
	{
		/// <summary>
		/// Holds the numerator.
		/// </summary>
		private readonly int numerator;

		/// <summary>
		/// Holds the denominator.
		/// </summary>
		private readonly int denominator;

		private int maxSimplificationCalculations = 1000;

		/// <summary>
		/// Creates a new instance of Rational. 
		/// Rational objects are immutable, so once you've set your numerator and 
		/// denominator values here, you're stuck with them! 
		/// </summary>
		/// <param name="aNumerator">a numerator</param>
		/// <param name="aDenominator"> a denominator</param>
		public Rational(int aNumerator, int aDenominator) 
		{
			this.numerator = aNumerator;
			this.denominator = aDenominator;
		}

		/// <summary>
		/// Returns the value of the specified number as a double. This may involve rounding. 
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type double.</returns>
		public double DoubleValue() 
		{
			return (double) numerator / (double) denominator;
		}

		/// <summary>
		/// Returns the value of the specified number as a float. This may involve rounding.
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type float.</returns>
		public float FloatValue() 
		{
			return (float) numerator / (float) denominator;
		}

		/// <summary>
		/// Returns the value of the specified number as a byte. 
		/// This may involve rounding or truncation.  
		/// This implementation simply casts the result of doubleValue() to byte. 
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type byte.</returns>
		public byte ByteValue() 
		{
			return (byte) DoubleValue();
		}

		/// <summary>
		/// Returns the value of the specified number as an int. 
		/// This may involve rounding or truncation.
		/// This implementation simply casts the result of doubleValue() to int. 
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type int.</returns>
		public int IntValue() 
		{
			return (int) DoubleValue();
		}

		/// <summary>
		/// Returns the value of the specified number as a long.
		/// This may involve rounding or truncation.
		/// This implementation simply casts the result of doubleValue() to long.
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type long.</returns>
		public long LongValue() 
		{
			return (long) DoubleValue();
		}

		/// <summary>
		/// Returns the value of the specified number as a short. 
		/// This may involve rounding or truncation.
		/// This implementation simply casts the result of doubleValue() to short. 
		/// </summary>
		/// <returns>the numeric value represented by this object after conversion to type short.</returns>
		public short ShortValue() 
		{
			return (short) DoubleValue();
		}

		/// <summary>
		/// Returns the denominator. 
		/// </summary>
		/// <returns>the denominator.</returns>
		public int GetDenominator() 
		{
			return this.denominator;
		}

		/// <summary>
		/// Returns the numerator. 
		/// </summary>
		/// <returns>the numerator.</returns>
		public int GetNumerator() 
		{
			return this.numerator;
		}

		/// <summary>
		/// Returns the reciprocal value of this obejct as a new Rational. 
		/// </summary>
		/// <returns>the reciprocal in a new object</returns>
		public Rational GetReciprocal() 
		{
			return new Rational(this.denominator, this.numerator);
		}

		/// <summary>
		/// Checks if this rational number is an Integer, either positive or negative. 
		/// </summary>
		/// <returns>true is Rational is an integer, false otherwize</returns>
		public bool IsInteger() 
		{
			return (denominator == 1
				|| (denominator != 0 && (numerator % denominator == 0))
				|| (denominator == 0 && numerator == 0));
		}

		/// <summary>
		/// Returns a string representation of the object of form numerator/denominator. 
		/// </summary>
		/// <returns>a string representation of the object.</returns>
		public override String ToString() 
		{
			return numerator + "/" + denominator;
		}

		/// <summary>
		/// Returns the simplest represenation of this Rational's value possible. 
		/// </summary>
		/// <param name="allowDecimal">if true then decimal will be showned</param>
		/// <returns>the simplest represenation of this Rational's value possible.</returns>
		public String ToSimpleString(bool allowDecimal) 
		{
			if (denominator == 0 && numerator != 0) 
			{
				return this.ToString();
			} 
			else if (IsInteger()) 
			{
				return IntValue().ToString();
			} 
			else if (numerator != 1 && denominator % numerator == 0) 
			{
				// common factor between denominator and numerator
				int newDenominator = denominator / numerator;
				return new Rational(1, newDenominator).ToSimpleString(allowDecimal);
			} 
			else 
			{
				Rational simplifiedInstance = GetSimplifiedInstance();
				if (allowDecimal) 
				{
					String doubleString =
						simplifiedInstance.DoubleValue().ToString();
					if (doubleString.Length < 5) 
					{
						return doubleString;
					}
				}
				return simplifiedInstance.ToString();
			}
		}


		/// <summary>
		/// Decides whether a brute-force simplification calculation should be avoided by comparing the 
		/// maximum number of possible calculations with some threshold. 
		/// </summary>
		/// <returns>true if the simplification should be performed, otherwise false</returns>
		private bool TooComplexForSimplification() 
		{
			double maxPossibleCalculations =
				(((double) (Math.Min(denominator, numerator) - 1) / 5d) + 2);
			return maxPossibleCalculations > maxSimplificationCalculations;
		}

		/// <summary>
		/// Compares two Rational instances, returning true if they are mathematically equivalent. 
		/// </summary>
		/// <param name="obj">the Rational to compare this instance to.</param>
		/// <returns>true if instances are mathematically equivalent, otherwise false. Will also return false if obj is not an instance of Rational.</returns>
		public override bool Equals(object obj) 
		{
			if (obj==null) return false;
			if (obj==this) return true;
			if (obj is Rational) 
			{
				Rational that = (Rational) obj;
				return this.DoubleValue() == that.DoubleValue();
			}
			return false;
		}

		/// <summary>
		/// Simplifies the Rational number.
		/// 
		/// Prime number series: 1, 2, 3, 5, 7, 9, 11, 13, 17
		/// 
		/// To reduce a rational, need to see if both numerator and denominator are divisible
		/// by a common factor.  Using the prime number series in ascending order guarantees
		/// the minimun number of checks required.
		/// 
		/// However, generating the prime number series seems to be a hefty task.  Perhaps
		/// it's simpler to check if both d & n are divisible by all numbers from 2 ->
		/// (Math.min(denominator, numerator) / 2).  In doing this, one can check for 2
		/// and 5 once, then ignore all even numbers, and all numbers ending in 0 or 5.
		/// This leaves four numbers from every ten to check.
		/// 
		/// Therefore, the max number of pairs of modulus divisions required will be:
		///
		///    4   Math.min(denominator, numerator) - 1
		///   -- * ------------------------------------ + 2
		///   10                    2
		///
		///   Math.min(denominator, numerator) - 1
		/// = ------------------------------------ + 2
		///                  5
		/// </summary>
		/// <returns>a simplified instance, or if the Rational could not be simpliffied, returns itself (unchanged)</returns>
		public Rational GetSimplifiedInstance() 
		{
			if (TooComplexForSimplification()) 
			{
				return this;
			}
			for (int factor = 2;
				factor <= Math.Min(denominator, numerator);
				factor++) 
			{
				if ((factor % 2 == 0 && factor > 2)
					|| (factor % 5 == 0 && factor > 5)) 
				{
					continue;
				}
				if (denominator % factor == 0 && numerator % factor == 0) 
				{
					// found a common factor
					return new Rational(numerator / factor, denominator / factor);
				}
			}
			return this;
		}

		/// <summary>
		/// Returns the hash code of the object
		/// </summary>
		/// <returns>the hash code of the object</returns>
		public override int GetHashCode() 
		{
			return this.denominator.GetHashCode()>>this.numerator.GetHashCode()*this.DoubleValue().GetHashCode();
		}
	}
}