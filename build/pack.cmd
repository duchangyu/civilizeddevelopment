@echo off

:: Project variables.
::
set project=Colibra
set vmajor=2
set vminor=0
if (%1)==() (
  goto error_no_build_number
) else (
  set vbuild=%1
)
if (%2)==() (
  set vsub=0
) else (
  set vsub=%2
)

:: Process variables.
::
set zip_util="C:\Program Files\7-Zip\7z.exe"
set file_list=filelist.txt

%zip_util% a -r -tzip %project%_%vmajor%_%vminor%_%vbuild%_%vsub%.zip @%file_list%

goto end

:error_no_build_number
  echo ERROR: No build number specified.
  echo Usage: pack buildnum [subnum]

:end
  echo Done.
