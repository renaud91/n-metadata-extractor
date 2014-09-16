@rem
@rem Use this bat in order to rebuild resources files from txt one
@rem

SET RESGEN_PATH="C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\resgen.exe"

%RESGEN_PATH% resources/CanonMarkernote.txt CanonMarkernote.resources
%RESGEN_PATH% resources/CasioMarkernote.txt CasioMarkernote.resources
%RESGEN_PATH% resources/Commons.txt Commons.resources
%RESGEN_PATH% resources/ExifInteropMarkernote.txt ExifInteropMarkernote.resources
%RESGEN_PATH% resources/ExifMarkernote.txt ExifMarkernote.resources
%RESGEN_PATH% resources/FujiFilmMarkernote.txt FujiFilmMarkernote.resources
%RESGEN_PATH% resources/GpsMarkernote.txt GpsMarkernote.resources
%RESGEN_PATH% resources/IptcMarkernote.txt IptcMarkernote.resources
%RESGEN_PATH% resources/JpegMarkernote.txt JpegMarkernote.resources
%RESGEN_PATH% resources/KodakMarkernote.txt KodakMarkernote.resources
%RESGEN_PATH% resources/KyoceraMarkernote.txt KyoceraMarkernote.resources
%RESGEN_PATH% resources/NikonTypeMarkernote.txt NikonTypeMarkernote.resources
%RESGEN_PATH% resources/OlympusMarkernote.txt OlympusMarkernote.resources
%RESGEN_PATH% resources/PanasonicMarkernote.txt PanasonicMarkernote.resources
%RESGEN_PATH% resources/PentaxMarkernote.txt PentaxMarkernote.resources
%RESGEN_PATH% resources/SonyMarkernote.txt SonyMarkernote.resources
