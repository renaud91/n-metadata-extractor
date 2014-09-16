using System;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using com.drew.metadata;
using com.drew.imaging.jpg;

using com.utils;
using com.utils.xml;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com
{
    public sealed class Run
    {
        private static readonly string AS_XML = "asXml";
        private static readonly string NO_UNKNOWN = "noUnknown";
        private static readonly string DO_SUB = "doSub";

        private static bool asXml = false;
        private static bool noUnknown = false;
        private static bool doSub = false;

        /// <summary>
        /// Search for the asXml parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindAsXml(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (AS_XML.Equals(someArgs[i]))
                {
                    Run.asXml = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for the noUnknown parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindNoUnknown(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (NO_UNKNOWN.Equals(someArgs[i]))
                {
                    Run.noUnknown = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for the doSub parameter in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        private static void FindDoSub(string[] someArgs)
        {
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (DO_SUB.Equals(someArgs[i]))
                {
                    Run.doSub = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Search for file names in the given args.
        /// </summary>
        /// <param name="someArgs">the given args</param>
        /// <returns>a file name list</returns>
        private static List<string> FindFileNames(string[] someArgs)
        {
            List<string> lcResu = new List<string>(someArgs.Length);
            for (int i = 0; i < someArgs.Length; i++)
            {
                if (AS_XML.Equals(someArgs[i]) || NO_UNKNOWN.Equals(someArgs[i]) || DO_SUB.Equals(someArgs[i]))
                {
                    continue;
                }
                lcResu.AddRange(Utils.SearchAllFileIn(someArgs[i], Run.doSub, "*.jpg"));
            }
            return lcResu;
        }


        /// <summary>
        /// The example.
        /// </summary>
        /// <param name="someArgs">Arguments</param>
        [STAThread]
        public static void Main(string[] someArgs)
        {

            if (someArgs.Length == 0)
            {
                Console.Error.WriteLine("Use: MetaDataExtractor [FilePaths|DirectoryPaths] [noUnknown|asXml|doSub]");
                Console.Error.WriteLine("     - noUnknown: will hide unknown metadata");
                Console.Error.WriteLine("     - asXml    : will generate an XML stream");
                Console.Error.WriteLine("     - doSub    : will search subdirectories for *.jpg");
                Console.Error.WriteLine("Examples:");
                Console.Error.WriteLine("     - Will show you MyImage.jpg info as text:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\MyImage.jpg");
                Console.Error.WriteLine(" or ");
                Console.Error.WriteLine("     - Will show you all c:\\*.jpg and img1 and img2 info as text:");
                Console.Error.WriteLine("       MetaDataExtractor C:\\ d:\\img1.jpg e:\\img2.jpg");
                Console.Error.WriteLine("     - Will show you all c:\\*.jpg info as text but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ noUnknown");
                Console.Error.WriteLine("     - Will show you all c:\\*.jpg info as XML:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ asXml");
                Console.Error.WriteLine("     - Will show you all c:\\*.jpg info as XML but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\ noUnknown asXml");
                Console.Error.WriteLine("     - Will show you all c:\\Temp\\*.jpg and all its subdirectories info as XML but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\Temp noUnknown asXml doSub");
                Console.Error.WriteLine("     - Will put in a file all c:\\Temp\\*.jpg and all its subdirectories info as XML but with no unkown tags:");
                Console.Error.WriteLine("       MetaDataExtractor c:\\Temp noUnknown asXml doSub > sample.xml");
                Console.Error.WriteLine("Cautions:");
                Console.Error.WriteLine(" + Options are case sensitive.");
                Console.Error.WriteLine(" + Pointing on c:\\ with doSub option is a very bad idea ;-)");
            }
            else
            {
                Run.FindAsXml(someArgs);
                Run.FindNoUnknown(someArgs);
                Run.FindDoSub(someArgs);

                StringBuilder lcGlobalBuff = new StringBuilder(1024);

                IOutPutTextStreamHandler lcXmlHandler = null;

                if (Run.asXml)
                {
                    lcXmlHandler = new XmlOutPutStreamHandler();
                }
                else
                {
                    lcXmlHandler = new TxtOutPutStreamHandler();
                }
                lcXmlHandler.DoUnknown = !Run.noUnknown;

                List<string> lcFileNameLst = Run.FindFileNames(someArgs);
                // Args for OutPutTextStream objects

                // Indicate your Xsl here
                string lcXslFileName = null; // For example: ="exif.xslt"; 
                string[] lcOutputParams = new string[] { "ISO-8859-1", lcXslFileName, lcFileNameLst.Count.ToString() };

                lcXmlHandler.startTextStream(lcGlobalBuff, lcOutputParams);
                IEnumerator<string> lcFileNameEnum = lcFileNameLst.GetEnumerator();
                while (lcFileNameEnum.MoveNext())
                {
                    StringBuilder lcBuff = new StringBuilder(1024);
                    string lcFileName = lcFileNameEnum.Current;
                    Metadata lcMetadata = null;
                    try
                    {
                        FileInfo lcImgFileInfo = new FileInfo(lcFileName);
                        lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFileInfo);
                        lcXmlHandler.Metadata = lcMetadata;
                    }
                    catch (JpegProcessingException e)
                    {
                        Console.Error.WriteLine(e.Message);
                        break;
                    }

                    if (Run.asXml)
                    {
                        // First open file name tag
                        lcBuff.Append("<file name=\"");
                        lcXmlHandler.Normalize(lcBuff, lcFileName, false);
                        lcBuff.Append("\">").AppendLine();
                        // Then create all directory tag
                        lcBuff.Append(lcXmlHandler.AsText());
                        // Then close file tag
                        lcBuff.Append("</file>").AppendLine();
                    }
                    else
                    {
                        lcBuff.Append("-> ");
                        lcXmlHandler.Normalize(lcBuff, lcFileName, false);
                        lcBuff.Append(" <-").AppendLine();
                        // Then create all directory tag
                        lcBuff.Append(lcXmlHandler.AsText());
                    }
                    lcMetadata = null;
                    // Adds result for this file to big buffer
                    lcGlobalBuff.Append(lcBuff);
                    lcGlobalBuff.AppendLine();
                }
                lcXmlHandler.endTextStream(lcGlobalBuff, lcOutputParams);

                Console.Out.WriteLine(lcGlobalBuff.ToString());
            }
            // Uncomment if you are running under VisualStudio
            // Console.In.ReadLine();
        }
    }
}