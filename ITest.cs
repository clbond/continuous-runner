using System.Threading.Tasks;

namespace TestRunner
{
    public interface ITest
    {
        Task<TestResult> Run();
    }
}