using BrowserController.Controllers;

namespace BrowserController
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        static void Main(string[] args)
        {
            I2IController.SetupBatchFromDirectory("p1", "p2");
        }
    }
}