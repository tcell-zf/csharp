@@ECHO OFF
ECHO Un-Register Mexxum.SysManageWindowsService windows services bunch...

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u TCell.UniversalWindowsService.exe

ECHO Completed!

PAUSE
