using BrowserController.Controllers;

namespace BrowserController
{
    public class Program
    {
        static void Main(string[] args)
        {
            I2IController.SetupBatchFromDirectory();
        }
    }
}