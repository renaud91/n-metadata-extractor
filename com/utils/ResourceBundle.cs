using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Resources;
using System.Globalization;

/// <summary>
/// The C# class was made by Ferret Renaud: 
/// <a href="mailto:renaud91@free.fr">renaud91@free.fr</a>
/// If you find a bug in the C# code, feel free to mail me.
/// </summary>
namespace com.utils
{
	public class ResourceBundle // : IResourceReader
	{

		protected ResourceManager resourceManager;

		public string this[string aKey] 
		{
			get 
			{
				return this[aKey, new string[] {null}];
			}
		}

		public string this[string aKey, string fillGapWith] 
		{
			get 
			{
				return this[aKey, new string[] {fillGapWith}];
			}
		}

		public string this[string aKey, string fillGap0, string fillGap1] 
		{
			get 
			{
				return this[aKey, new string[] {fillGap0, fillGap1}];
			}
		}


		public string this[string aKey, string[] fillGapWith] 
		{
			get 
			{
				string resu = (string)this.resourceManager.GetString(aKey);
				if (resu==null) 
				{
					throw new Exception("\""+aKey+"\" No found");
				}
				return replace(resu, fillGapWith);
			}
		}

		private ResourceBundle() : base()
		{
		}

		public ResourceBundle(string aPropertyFileName) : base()
		{	
			this.resourceManager = new ResourceManager(aPropertyFileName, Assembly.GetAssembly(Type.GetType("com.utils.ResourceBundle")));
		}

				
		public void Dispose() 
		{
			if (this.resourceManager != null) 
			{
				this.resourceManager.ReleaseAllResources();
				this.resourceManager = null;
			}
		}

		protected string replace(string aLine, string[] fillGapWith) 
		{
			for (int i=0; i<fillGapWith.Length; i++) 
			{
				if (fillGapWith[i]==null) 
				{
					fillGapWith[i] = "";
				}
				aLine = aLine.Replace("{"+i+"}",fillGapWith[i]);			
			}
			return aLine;
		}
	}
}