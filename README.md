# UnityShaderAutoCompletion
this project aim to provide the autocompletion or intellisense for unity shader.
as there is no ide or plugins for autocompletion when writing shaders for unity with cg/cgsl.
so i decide to write an add-in for monodevelop/xamarin ide,this is the easiest way for me now,
the add-in porvides basic completion functionality,i will upgrade the functionality in my rest
time.if someone has advice for autocompletion,email to me please.

1、the packages dir is monodevelop addin manager, just for backup,you'd better install the add-in
   manager in ide.

2、the shaderparsetool dir is python scripts for parse unity cginc files.
   don't tucao,i learn the python just for 3 hours.

3、TernaryTree dir is the search utils for tags built from json file.

4、unityshadertool project is the xamarin add-in project,as the help document for mono is little
   a bit ^~^.so until now,i just finish the basic completion,the ideal tools like visual studio
   intellisense need for research,if you have better way,please contact me.we can discuss it.

5、the unityshadertool.zip is the pack for add-in,you can't install by the monodevelop/xamarin
   manager,it will fail,i tryed many times.so the easiest way is copy the file into xamarin add-in dir,like xamarin stuido/Contents/Resources/lib/monodevelop/Addins/
   remember to restart the xamarin,and make sure the addin is enabled.the xamarin version is 5.5.*