Unity does not have good integration with Nuget, and even if it did you'd have to compile a nuget package for every single Unity version ever.
For this reason, the files in this folder are set to BuildAction=None.
You can use these files by copying them into your assets folder (possibly in an AssemblyDef folder). 