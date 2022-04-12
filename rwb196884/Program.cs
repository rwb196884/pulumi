using System.Threading.Tasks;
using Pulumi;

namespace rwb196884
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}