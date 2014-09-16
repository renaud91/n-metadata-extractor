Renaud’s MetaDataExtractor 
====================

What is it ?

MetaDataExtractor is a program/library that let you extract the metadata from your pictures. Your JPG files contain a lot of interesting information (meta data) like:
the date
the setings, flash, exposure time, program used ...
and many more

An Example of how to use the librairy is available in the com.Run.cs class. This code is inside the source release. 
An other example is also available, since version 2.3.0c, as com.SimpleRun.cs.

If you need a Java librairy that does the same, you can have a look at those projects:
MetaDataSpr: Meta data reader in Spring, writen by me, still in alpha
MetaDataExtractor: Writen by Drew Noakes


Changes in 2.4.0d (17 June 2011):
* Following the remark of Lars Åke Vinberg, I change the code in order to use the foreach optimisation for the Directory and Metadata classes. 
* I keept the old GetIterator method for compatibility.

Changes in 2.4.0c (14 January 2011):
* Corrected a bug in ExifReader.cs where a space was preventing introspection * of OlympusDirectory to work (thanks to Zvika Gart).
* Changed the code for method StoreThumbnailBytes in order to use Buffer.BlockCopy instead of the manual byte/byte copy (thanks to Zvika Gart).
* Added some docmentation in Rationnal class in order to warn about IntValue and other methods when denominator is 0 (thanks to Steven Jeuris).

* Changes in 2.4.0b (18 April 2009):
* Corrected a bug in IPTC reading when date was wrong (thanks to Mathieu).
* Corrected a bug in method that was supposed to filter image (occured when there was only one file).

Changes in 2.4.0a (30 Juin 2008):
Changed back Dispose call to Close call since Dispose does not always close Stream.
Added some Close where it was needed (Thanks to X Bühlmann)
Added some Canon tags (Thanks to X Bühlmann)

Changes in 2.4.0 (22 Mai 2008):
One big change in this version is the way resources are handled. I was fed up with the C# .resources that does not work well. 
So now, a resource file is a simple txt file that can be found in the resource folder. 
You can translate them if you want and add them to the project. Simply create a directory (for example resource\fr) and put the file in it. DO NOT FORGET to make a right click and add to project as embeded resource. 
The behaviour is as follow : if resource is in the langage folder, then it will be used, if not it will use the default resource file (the one at the root of the resource directory). 
With this system there should not be any more error regarding resource file (I hope ...).
Impacted change from beta version of Drew, so this version can read metadata from RAW files (*.raw, *.cr2, *.crw). Note that this does not work with all raw format (it works for me with the 5D raw, but not with the 10D raw)
Changed the Default namespace to MetaDataExtractor (instead of com).
Corrected the bug found by TIMOTHY regarding the GetFocalPlaneYResolutionDescription in ExifDirectory.
Corrected a bug where some date may not be parsed correcly.
Project is compiled for .NET 2.0 but is to be used with Visual Studio Free Edition 2008. It is fully compatible with 3.0 and 3.5 .NET version
Added a new XML output format (use asXml2 instead of asXml). I use this version and found it more practical.
Changed the API in order to be more object
Added a few support for Trace and Debug (using System.Diagnostics)

Changes in 2.3.0g (18 Mai 2006):
Changed the method InitTagMap in all Directory object. This method will now use introspection in order to discover tags.With this system, adding a tag is very simple, you just need to add the attribute in the directory class and the linked label in the resource file.
Change ResourceBundle default lookup path for resource files. 
For those who need to recompile the project, and if you ever change the Default namespace property of your project:
Project Property
DO NOT FORGET to change the DEFAULT_NAME_SPACE value of the ResourceBundle class:
DEFAULT_NAME_SPACE attribute
If you forget, then all resources will not be found, since .NET 2, adds the Default namespace property to the XXX.resources files.

Changes in 2.3.0f (17 April 2006):
Corrected a strange character problem in some Nikon firmware version tag (thanks to Eric)
If XML output is used, all values will be trimed now.

Changes in 2.3.0e (06 April 2006):
Corrected the misspelling of FujifilmDirectory
Added a simple XSLT writen by Eric (Thanks to him).

Changes in 2.3.0d (31 March 2006):
Corrected all bundle missing/misspelling key problem using a test class (TestAllKeyWords.cs)
Changed the text writer in order to use Unicode(thanks to Lytton Liou again)
Corrected a double key in on of the Directory class.
Added an option doSub and changed the lauch process for the exe file.

Changes in 2.3.0c (30 March 2006):
Wrote CENTRE instead of CENTER in Nikon file (thanks to Lytton Liou)
Added the com.SimpleRun.cs examples.

Changes in 2.3.0b (19 March 2006):
Wrote RED_YEY_REDUCTION instead of RED_EYE_REDUCTION in Commons.txt file (thanks to Jeppe Høiby)

Changes in 2.3.0a (12 March 2006):
Forgot a tag in Nikon resources file (thanks to Jeppe Høiby)
VERY IMPORTANT: Changes were made to the XML output
Added a DTD and an XML sample file

Changes in 2.3.0 (05 March 2006):
Updated from the 2.3.0 Java version (see http://www.drewnoakes.com/code/exif/ web site for details)
Added an asXml option. If someone is good in XSL, he may send me its own, I will add it into the next release.
VERY IMPORTANT: This release is based on .NET 2.0 and may not work with .NET 1.0 or 1.1
For those who would like to recompile or change the code, do not forget that VisualStudio 2005 for CSharp is free in its Express edition. Simply go to Microsoft Web site and you can download it and use it for free.

Changes in 2.2.2d (second edition):
I forgot to uncommant the line that printed the output in the executable file, so I rebuild it, sorry, I have two left hands :-) somtimes. 
I did not change the version for that.
Corrected a bug with StringArray, in fact all data that was treated as an array of String were showned as one string that was the first element in the array. For example keywords for IPTC tag. Thanks to Graziano Tona and Don Kim who found this bug almost at the same time :-).

Changes in 2.2.2c :
Changed the TAG_WHITE_BALANCE value (thanks to Peter Hiemenz)
Added some new values into the Exif directory ( TAG_CUSTOM_RENDERED, TAG_EXPOSURE_MODE, TAG_DIGITAL_ZOOM_RATIO TAG_FOCAL_LENGTH_IN_35MM_FILM, TAG_SCENE_CAPTURE_TYPE, TAG_GAIN_CONTROL TAG_CONTRAST, TAG_SATURATION, TAG_SHARPNESS, TAG_DEVICE_SETTING_DESCRIPTION TAG_SUBJECT_DISTANCE_RANGE, TAG_IMAGE_UNIQUE_ID) (thanks to Peter Hiemenz)
Changed all static readonly into const, this allow the use of swich instead of ifelse. It should also speed up the program

Changes in 2.2.2b :
the file acces mode for ASPX user is readonly now (thanks to Ryan Patridge)
Added support for Windows properties (Tiltle, Author, Comments ...) (thanks to Ryan Patridge)
Changed the ReadString behaviour (removed spaces)


You can find more information on this site http://www.drewnoakes.com/code/exif/ 

There is only the C# version on my site, the version is almost the same as the Java one made by Drew Noakes
If you are interested in a C++ version one is available on the Andreas Huggel web site
And of course, do not miss the ExifTool by Phil Harvey (can read and write a lot of tags).
