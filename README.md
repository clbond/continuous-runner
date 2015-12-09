# continuous-runner
Continuous background JavaScript test runner with Visual Studio integration

## Projects
This project is broken into several pieces:
  * Continuous Runner library
  * Visual Studio editor extension
  * Console runner for testing and debugging

### Continuous Runner library
In essence, it is a C# application library that will allow you to continuously run JavaScript unit tests in a background thread. Each time a JS source file changes, it and all its dependencies, as well as files that depend -- directly or indirectly -- on the changed file, are marked as requiring a re-run. The goal is to get continuous feedback on JavaScript unit test health as you edit your code, without having to continually keep re-running the tests yourself. The second you change a line that breaks something, the extension will tell you and allow you to step through the test in question inside the VS debugger.

### Visual Studio editor extension
Then we have the Visual Studio Add-in element of the project, VsIntegration. This adds support for the Continuous Runner right inside of Visual Studio, including:

 * UI for showing the current status of all JavaScript unit-tests
 * Editor integration that shows you exactly what change broke exactly what tests
 * UI for manually running tests, viewing run logs
 
### Console runner
This is a simple console frontend for Continuous Runner. Its primary use is for development and debugging of ContinuousRunner outside of Visual Studio.
