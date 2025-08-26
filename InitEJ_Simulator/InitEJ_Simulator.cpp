// InitEJ_Simulator.cpp :console application.
// Simulates InitEJ.exe on ATMs in that it copies EJDATA.LOG to EJRCPY.LOG.
// It does not initialize EJDATA.LOG!!!
// It fails if EJRCPY.LOG already exists
// On failure returns non-zero error code to the OS. Zero otherwise.

// The following C++ redistributable DLLs may be needed for distribution
/*
msvcp120.dll
msvcr120.dll
vccorlib120.dll
*/


#include "stdafx.h"
#include <windows.h>
#include <tchar.h>
#include <stdio.h>
#include <strsafe.h>


int main()
{
	BOOL rc;
	BOOL FailIfExists = 1;

	// Random sleep (1 to 60 seconds)
	/*
	int randNum;
	randNum = rand()%(59000) +1000; 
	Sleep(randNum);
	*/
	
	// Copy the file
	//   If the function succeeds, the return value is nonzero.
	//	 If the function fails, the return value is zero.To get extended error information, call GetLastError.
		rc = CopyFile((_T("C:\\Program Files\\Advance NDC\\Data\\EJDATA.LOG")), (_T("C:\\Program Files\\Advance NDC\\Data\\EJRCPY.LOG")), FailIfExists);
		if (rc)
		{
			return 0;
		}
		else
		{
			DWORD dw = GetLastError();
			printf("InitEJ returned (hex): %#x", dw);
			return (int)dw;
		}
}

