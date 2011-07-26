@echo off
set shortdrive=X:
echo %cd%
subst %shortdrive% /D
subst %shortdrive% %cd%
