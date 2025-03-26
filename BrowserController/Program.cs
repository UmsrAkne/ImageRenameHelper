using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BrowserController
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 既存のブラウザに接続
            var options = new ChromeOptions
            {
                DebuggerAddress = "localhost:9222", // デバッグモードに接続
            };

            var targetDirectory = "targetDir";
            var pngInfoDirectory = "pngInfoDir";

            var driver = new ChromeDriver(options);

            // i2i tab
            // ButtonClick(driver, "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[1]/button[2]");
            ButtonClick(driver,
                "#tabs > div > button:nth-child(2)",
                "i2i タブをクリック");

            // Generation tab
            ButtonClick(driver, "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[1]/button[1]");

            // Batch tab
            ButtonClick(driver, "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[1]/button[6]");

            // From Directory tab
            ButtonClick(driver, "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[1]/div[1]/button[2]");

            InputTextArea(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[1]/div[4]/div/div[2]/div[1]/label/textarea",
                targetDirectory);

            // 'icon'クラスを持つspan要素を取得
            var iconElement = driver.FindElement(By.XPath("/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[2]/span[2]"));

            // 'transform'プロパティの値を取得
            var transformValue = iconElement.GetCssValue("transform");

            if (transformValue.Contains("rotate(0deg)"))
            {
                // PngInfo エリアの表示トグル
                ButtonClick(driver, "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[2]");
                Thread.Sleep(400);
            }

            // PNG Info directory
            InputTextArea(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[3]/div/div/div[2]/label/textarea",
                pngInfoDirectory);

            // Append Png info to prompts
            ChangeCheckBoxState(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[3]/div/div/div[1]/label/input",
                true);

            // Prompt
            ChangeCheckBoxState(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[3]/div/div/fieldset/div[3]/label[1]/input",
                true);

            // Negative Prompt
            ChangeCheckBoxState(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[3]/div/div/fieldset/div[3]/label[2]/input",
                true);

            // Seed
            ChangeCheckBoxState(driver,
                "/html/body/gradio-app/div/div/div[1]/div/div/div[2]/div[3]/div/div[2]/div[2]/div/div/div[1]/div[1]/div[8]/div/div[2]/div[3]/div/div/fieldset/div[3]/label[3]/input",
                true);

            driver.Quit();
            Console.WriteLine("処理を終了します");
        }

        private static void ButtonClick(IWebDriver driver, string fullXPath)
        {
            var button = driver.FindElement(By.XPath(fullXPath));
            button.Click();

            Thread.Sleep(200);
        }

        private static void ButtonClick(IWebDriver driver, string cssSelector, string consoleMessage)
        {
            var button = driver.FindElement(By.CssSelector(cssSelector));
            button.Click();
            Console.WriteLine(consoleMessage);

            Thread.Sleep(200);
        }

        private static void InputTextArea(IWebDriver driver, string fullXPath, string inputText)
        {
            var textArea = driver.FindElement(By.XPath(fullXPath));
            textArea.Clear();
            textArea.SendKeys(inputText);

            Thread.Sleep(200);
        }

        private static void ChangeCheckBoxState(IWebDriver driver, string fullXPath, bool state)
        {
            var checkBox = driver.FindElement(By.XPath(fullXPath));
            if (checkBox.Selected == state)
            {
                return;
            }

            checkBox.Click();  // チェックボックスをオンにする
            Thread.Sleep(200);
        }
    }
}