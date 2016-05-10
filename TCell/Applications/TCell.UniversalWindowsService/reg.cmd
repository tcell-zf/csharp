@@ECHO OFF
ECHO Register Mexxum.SysManageWindowsService windows services bunch...

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe TCell.UniversalWindowsService.exe
Net Start TCell.UniversalWindowsService
sc config TCell.UniversalWindowsService start=auto

ECHO Completed!

PAUSE
