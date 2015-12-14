using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public interface IJasmineReflection
    {
        /// <summary>
        /// Reflect a mock Jasmine library that will just reflect back the data it is given, in the form of a collection
        /// of test suites and test definitions. <see cref="Reflect"/> will add mock version of describe(), it(), etc.,
        /// and those mock versions send back enough data to create an <see cref="ITestCollection"/> object describing
        /// the tests inside of the script.
        /// </summary>
        /// <param name="script">The script containing the test definitions</param>
        /// <param name="engine">The V8 script engine context to apply the mock library definition to</param>
        /// <returns></returns>
        ITestCollection Reflect(IScript script, V8ScriptEngine engine);
    }
}
