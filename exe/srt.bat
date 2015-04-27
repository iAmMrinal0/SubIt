@echo off
cls
:start
IF %1=="" GOTO start
  C:\SubIt.exe %1
  SHIFT
  GOTO my_loop
:start
