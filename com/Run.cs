using System;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections;

using com.drew.metadata;
using com.drew.imaging.jpg;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com 
{
	public sealed class Run 
	{
		private static readonly string NO_UNKNOWN = "noUnknown";
		private static readonly	string UNKNOWN = "Unknown";

		private static bool noUnknown = false;

		private static void FindNoUnknown(string[] args) 
		{
			for (int i=0; i<args.Length; i++) 
			{
				if (NO_UNKNOWN.Equals(args[i])) 
				{
					Run.noUnknown = true;
					break;
				}
			}
		}

		private static IList FindFileNames(string[] args) 
		{
			IList resu = new ArrayList(args.Length);
			for (int i=0; i<args.Length; i++) 
			{
				int idS = args[i].IndexOf("*");
				if (idS != -1) 
				{
					string path = args[i].Substring(0, idS);
					string[] l = System.IO.Directory.GetFiles(path, "*.jpg");
					for(int j=0; j<l.Length; j++) 
					{
						resu.Add(l[j]);
					}
					continue;
				} 
				else if (NO_UNKNOWN.Equals(args[i]))
				{
					continue;
				}
				resu.Add(args[i]);
			}
			return resu;
		}


		[STAThread]
		public static void Main(string[] args)
		{	
			if (args.Length == 0) 
			{
				Console.Error.WriteLine("Please indicate an image path :");
				Console.Error.WriteLine("    MetaDataExtractor c:\\MyImage.jpg");
				Console.Error.WriteLine(" or ");
				Console.Error.WriteLine("    MetaDataExtractor c:\\*.jpg d:\\img1.jpg e:\\img2.jpg f:\\*.jpg");
				Console.Error.WriteLine(" You can also use the option noUnknown if you do not want");
				Console.Error.WriteLine(" to see unknown values or tags");
				Console.Error.WriteLine("    MetaDataExtractor c:\\*.jpg noUnknown");
			} 
			else 
			{		
				FindNoUnknown(args);
				IList l = FindFileNames(args);
				IEnumerator enumerator = l.GetEnumerator();
				while(enumerator.MoveNext()) 
				{
					string fileName = (string)enumerator.Current;
					Metadata metadata = null;
					try 
					{
						FileInfo imgFile = new FileInfo(fileName);
						metadata = JpegMetadataReader.ReadMetadata(imgFile);
					} 
					catch (JpegProcessingException e) 
					{
						Console.Error.WriteLine(e.Message);
						break;
					}		
					StringBuilder sb = new StringBuilder(1024);
					sb.Append("---> "+fileName+" <---\n");
					IEnumerator directoryIterator = metadata.GetDirectoryIterator();
					while (directoryIterator.MoveNext()) 
					{
						com.drew.metadata.Directory directory = (com.drew.metadata.Directory) directoryIterator.Current;
						// Console.WriteLine("Dr "+directory.ToString());
						IEnumerator ie = directory.GetErrors();
						while (ie.MoveNext()) 
						{
							Console.Error.WriteLine("Error Found: "+ie.Current);
						}
						IEnumerator tagsIterator = directory.GetTagIterator();
						while (tagsIterator.MoveNext()) 
						{
							Tag aTag = (Tag) tagsIterator.Current;
							string aDescription = UNKNOWN;
							try 
							{
								aDescription = aTag.GetDescription();
							} 
							catch (MetadataException e1) 
							{
								Console.Error.WriteLine(e1.Message);
							}
							string aName = aTag.GetTagName();
							if (noUnknown && (aName.IndexOf(UNKNOWN) >= 0
								|| aDescription.IndexOf(UNKNOWN) >= 0)) 
							{
								continue;
							} else
							{
								sb.Append(aTag.GetTagName());
								sb.Append('=');
								try 
								{
									sb.Append(aTag.GetDescription());
								} 
								catch (MetadataException e2) 
								{
									Console.Error.WriteLine(e2.Message);
								}
								sb.Append("\n");
							}
							aTag = null;
							aDescription = null;
							aName = null;
						}
						directory = null;
						tagsIterator = null;
					}
					directoryIterator = null;
					metadata = null;
					Console.Out.WriteLine(sb.ToString());
				}
			}
			
			// Console.ReadLine();
		}
	}
}