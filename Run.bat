@echo off


cd "%~dp0\TestHarnessGUI\bin\Debug"
start TestHarnessGUI.exe

cd "%~dp0\Client1\bin\Debug"
start Client1.exe

cd "%~dp0\TestExecutive\bin\Debug"
start TestExecutive.exe

cd "%~dp0\Repository\bin\Debug"
start Repository.exe


@pause