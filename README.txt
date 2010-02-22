Installation for Build Servers
------------------------------

Install "Microsoft Visual Studio Tools for the Microsoft Office system (version 3.0 Runtime) (x86)" from 

http://www.microsoft.com/downloads/details.aspx?FamilyID=54eb3a5a-0e52-40f9-a2d1-eecd7a092dcb 

Install "Microsoft Visual Studio Tools for the Microsoft Office System (version 3.0 Runtime) Service Pack 1 (x86)" from

http://www.microsoft.com/downloads/details.aspx?FamilyID=D8EB4921-891A-4B5E-973F-0B96E6CCF376

Install "2007 Microsoft Office System Update: Redistributable Primary Interop Assemblies" from

http://www.microsoft.com/downloads/details.aspx?FamilyID=59daebaa-bed4-4282-a28c-b864d8bfa513

Install "Windows SDK for Windows Server 2008 and .NET Framework 3.5" from

http://www.microsoft.com/downloads/details.aspx?FamilyId=F26B1AA4-741A-433A-9BE5-FA919850BDBF

Register the VCProjectEngine.dll with

regsvr32 "C:\Program Files\Microsoft Visual Studio 9.0\VC\vcpackages\VCProjectEngine.dll"

(see http://blogs.msdn.com/jjameson/archive/2009/11/07/compiling-c-projects-with-team-foundation-build.aspx)

Add "C:\Program Files\Microsoft Visual Studio 9.0\VC\vcpackages" to your PATH environment variable

Compile the DevInstaller project on a Windows Machine which has a Visual Studio
2008 Professional Edition SP1 with full VSTO support installed and install that
MSI on your build server. According to MSFT it is ok to violate your VS license in this way:

http://social.msdn.microsoft.com/Forums/en-US/vsto/thread/497324ff-a2e0-428c-bc1a-238db0c3d252

---------------------------

See this blog post for details on what the DevInstaller does:

http://kentb.blogspot.com/2008/08/building-vsto-projects-without-visual.html
