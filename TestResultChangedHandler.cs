namespace ContinuousRunner
{
    /// <summary>
    /// An event handler indicating that a test has been re-run and its state has changed
    /// </summary>
    /// <param name="changedEvent">An object describing the state change for this test</param>
    public delegate void TestResultChangedHandler(TestResultChanged changedEvent);
}
