using System;
using System.Collections.Generic;
using System.Text;

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
namespace com.drew.imaging
{
    /// <summary>
    /// Contains helper methods that perform photographic conversions.
    /// </summary>
    public class PhotographicConversions
    {
        public readonly static double ROOT_TWO = Math.Sqrt(2);

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        private PhotographicConversions() : base()
        {
            throw new Exception("Do not use");
        }

        /// <summary>
        /// Converts an aperture value to its corresponding F-stop number.
        /// </summary>
        /// <param name="anAperture">the aperture value to convert</param>
        /// <returns>the F-stop number of the specified aperture</returns>
        public static double ApertureToFStop(double anAperture)
        {
            return Math.Pow(ROOT_TWO, anAperture);

            // Puzzle?!
            // jhead uses a different calculation as far as i can tell...  this confuses me...
            // fStop = (float)Math.exp(aperture * Math.log(2) * 0.5));
        }

        /// <summary>
        /// Converts a shutter speed to an exposure time.
        /// </summary>
        /// <param name="aShutterSpeed">the shutter speed to convert</param>
        /// <returns>the exposure time of the specified shutter speed</returns>
        public static double ShutterSpeedToExposureTime(double aShutterSpeed)
        {
            return (float)(1 / Math.Exp(aShutterSpeed * Math.Log(2)));
        }
    }
}