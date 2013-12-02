
To run tests:

1. Click "BlogEngine\BlogEngine.sln" to load solution in VisualStudio 2010 or Visual Web Developer 2010.

2. Change site root in "Constants.cs" if needed (default http://localhost:53265)

3. Build and run solution (ctrl + F5 in Visual Studio)

4. Click "..\lib\nunit\runner\nunit.exe" to start NUnit GUI.

5. File -> Open Project -> then select "BlogEngine\BlogEngine.Tests\bin\Debug\BlogEngine.Tests.dll"

6. Click "Run", all should pass.

* There are 3 categories of tests you can exclude from running if applicable:
	- online: tests require online connection
	- slow: tests that run slow, usually because they have to restart IIS
	- primary: tests only applied to primary blog, exclude if testing child blog
To exclude, click categories tab in NUnit GUI and select categories to be excluded.

Known issues:
- Sometimes Watin has issues closing IE when another instance of IE running at the same time. This results in
  long pauses at the end of most tests. To resolve, close all instances of IE before running tests.
