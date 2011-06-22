@echo off
set shortdrive=Y:

subst %shortdrive% /D
subst %shortdrive% %cd%
