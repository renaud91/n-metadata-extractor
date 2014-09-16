using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Resources;
using System.Globalization;

/// <summary>
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// If you make modifications to this code that you think would benefit the
/// wider community, please send me a copy and I'll post it on my site.
/// 
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.utils
{
    /// <summary>
    /// This class is a bundle class.<br/>
    /// 
    /// Used for internationalisation (multi-language).<br/>
    /// 
    /// Allow the use of messages with holes.<br/>
    /// 
    /// Example:
    /// <pre>
    /// KEY1=Hello
    /// KEY2=Hello {0}
    /// KEY3=Hello {0} with an age of {1}
    /// Then you will use :
    /// myBundle["KEY1"];
    /// myBundle["KEY2", "Jhon"];
    /// myBundle["KEY3", "Jhon", 32.ToString()];
    /// myBundle["KEY3", new string[] {"Jhon", 32.ToString()}];
    /// </pre>
    /// </summary>
    public sealed class ResourceBundle // : IResourceReader
    {
        /// <summary>
        /// Put here the same value as you can see in your project property on tab Application for information default namespace.
        /// </summary>
        private const string DEFAULT_NAME_SPACE = "com";

        private ResourceManager resourceManager;

        /// <summary>
        /// Indexator on a simple aMessage.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey]
        {
            get
            {
                return this[aKey, new string[] { null }];
            }
        }

        /// <summary>
        /// Indexator on a aMessage with one hole {0} in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGapWith">what to put in hole {0}</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey, string fillGapWith]
        {
            get
            {
                return this[aKey, new string[] { fillGapWith }];
            }
        }

        /// <summary>
        /// Indexator on a aMessage with two holes {0} and {1} in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGap0">what to put in hole {0}</param>
        /// <param name="fillGap1">what to put in hole {1}</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey, string fillGap0, string fillGap1]
        {
            get
            {
                return this[aKey, new string[] { fillGap0, fillGap1 }];
            }
        }

        /// <summary>
        /// Indexator on a aMessage with many holes {0}, {1}, {2] ... in it.
        /// </summary>
        /// <param name="aKey">the referenced key</param>
        /// <param name="fillGapWith">what to put in holes. fillGapWith[0] used for {0}, fillGapWith[1] used for {1} ...</param>
        /// <returns>the aMessage attached to this key, or launch a MissingResourceException if none found</returns>
        public string this[string aKey, string[] fillGapWith]
        {
            get
            {
                string resu = this.resourceManager.GetString(aKey, CultureInfo.CurrentCulture);
                if (resu == null)
                {
                    throw new MissingResourceException("\"" + aKey + "\" Not found");
                }
                return replace(resu, fillGapWith);
            }
        }

        /// <summary>
        /// Constructor of the object.
        /// 
        /// Keep private, use the other one.
        /// </summary>
        private ResourceBundle()
            : base()
        {
        }

        /// <summary>
        /// Constructor of the object.
        /// </summary>
        /// <param name="aPropertyFileName">The resource file where to find keys. Do not add the extension and do not forget to add your resource file into the assembly.</param>
        public ResourceBundle(string aPropertyFileName)
            : base()
        {
            if (DEFAULT_NAME_SPACE != null && !"".Equals(DEFAULT_NAME_SPACE.Trim()))
            {
                aPropertyFileName = DEFAULT_NAME_SPACE + "." + aPropertyFileName;
            }
            this.resourceManager = new ResourceManager(aPropertyFileName, this.GetType().Assembly);
        }

        /// <summary>
        /// Clean the object.
        /// </summary>				
        public void Dispose()
        {
            if (this.resourceManager != null)
            {
                this.resourceManager.ReleaseAllResources();
                this.resourceManager = null;
            }
        }

        /// <summary>
        /// Fills the gap in a string.
        /// </summary>
        /// <param name="aLine">where to fill the gap. A gap is {0} or {1} ...</param>
        /// <param name="fillGapWith">what to put in the gap. fillGapWith[0] will go in {0} and so on</param>
        /// <returns></returns>
        private string replace(string aLine, string[] fillGapWith)
        {
            for (int i = 0; i < fillGapWith.Length; i++)
            {
                if (fillGapWith[i] == null)
                {
                    fillGapWith[i] = "";
                }
                aLine = aLine.Replace("{" + i + "}", fillGapWith[i]);
            }
            return aLine;
        }
    }
}