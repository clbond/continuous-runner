using System.Threading.Tasks;

namespace Contestual
{
    public interface ITest
    {
        Task<TestResult> Run();
    }
}