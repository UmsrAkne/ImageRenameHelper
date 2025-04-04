using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BrowserController.Controllers
{
    public static class I2IController
    {
        private readonly static string TopPageI2ITabSelector = "#tabs > div > button:nth-child(2)";
        private readonly static string I2IGenerationTabSelector = "#img2img_extra_tabs > div > button";
        private readonly static string I2IGenerationBatchTabSelector = "#mode_img2img > div.tab-nav.scroll-hide > button:nth-of-type(6)";
        private readonly static string I2IGenerationBatchFromDirTabSelector = "#img2img_batch_source > div.tab-nav.scroll-hide > button:nth-of-type(2)";
        private readonly static string PngInfoAreaToggleButtonSelector = "#component-894 > div.label-wrap > span:nth-child(1)";

        private readonly static string I2IPromptArea = "#img2img_prompt > label > textarea";
        private readonly static string I2INegativePromptArea = "#img2img_neg_prompt > label > textarea";

        public static void SetupBatchFromDirectory(string pngInfoDirectoryPath, string targetDirectoryPath)
        {
            // 既存のブラウザに接続
            var options = new ChromeOptions
            {
                DebuggerAddress = "localhost:9222", // デバッグモードに接続
            };

            var driver = new ChromeDriver(options);

            // i2i tab
            ButtonClick(driver,
                TopPageI2ITabSelector,
                "i2i タブをクリック");


            InputTextArea(driver,
                I2IPromptArea,
                string.Empty,
                "Prompt　エリアをリセット"
            );

            InputTextArea(driver,
                I2INegativePromptArea,
                string.Empty,
                "Negative Prompt　エリアをリセット"
            );

            // Generation tab
            ButtonClick(driver, I2IGenerationTabSelector, "i2i.Generation タブをクリック");

            // Batch tab
            ButtonClick(driver, I2IGenerationBatchTabSelector, "i2i.Generation.Batch タブをクリック");

            // From Directory tab
            ButtonClick(driver, I2IGenerationBatchFromDirTabSelector, "i2i.Generation.Batch.FromDirectory タブをクリック");

            InputTextArea(driver,
                "#img2img_batch_input_dir > label > textarea",
                targetDirectoryPath,
                "Input Directory テキストボックスに書き込み"
            );

            // 'icon'クラスを持つspan要素を取得
            var iconElement = driver.FindElement(By.CssSelector("#component-894 > div.label-wrap > span.icon"));

            // 'transform'プロパティの値を取得
            var transformValue = iconElement.GetCssValue("transform");

            if (!transformValue.Contains("matrix(1, 0, 0, 1, 0, 0)"))
            {
                // PngInfo エリアの表示トグル
                ButtonClick(driver, PngInfoAreaToggleButtonSelector, "png 表示エリアのトグルボタンをクリック");
                Thread.Sleep(400);
            }

            System.Diagnostics.Debug.WriteLine($"一時停止(Program : 60)");

            // PNG Info directory
            InputTextArea(driver,
                "#img2img_batch_png_info_dir > label > textarea",
                pngInfoDirectoryPath,
                "png info ディレクトリのパスを入力");

            // Append Png info to prompts
            ChangeCheckBoxState(driver, "#img2img_batch_use_png_info > label > input", true);

            // Prompt
            ChangeCheckBoxState(driver, "#component-897 > div.wrap > label:nth-child(1) > input", true);

            // Negative Prompt
            ChangeCheckBoxState(driver, "#component-897 > div.wrap > label:nth-child(2) > input", true);

            // Seed
            ChangeCheckBoxState(driver, "#component-897 > div.wrap > label:nth-child(3) > input", true);

            driver.Quit();
            Console.WriteLine("処理を終了します");
        }

        private static void ButtonClick(IWebDriver driver, string cssSelector, string consoleMessage)
        {
            var button = driver.FindElement(By.CssSelector(cssSelector));
            button.Click();
            Console.WriteLine(consoleMessage);

            Thread.Sleep(200);
        }

        private static void InputTextArea(IWebDriver driver, string selector, string inputText, string consoleMessage)
        {
            var textArea = driver.FindElement(By.CssSelector(selector));
            textArea.Clear();
            textArea.SendKeys(inputText);
            Console.WriteLine(consoleMessage);

            Thread.Sleep(200);
        }

        private static void ChangeCheckBoxState(IWebDriver driver, string selector, bool state)
        {
            var checkBox = driver.FindElement(By.CssSelector(selector));
            if (checkBox.Selected == state)
            {
                return;
            }

            Console.WriteLine($"{selector}(Click : 110)");

            checkBox.Click();  // チェックボックスをオンにする
            Thread.Sleep(200);
        }
    }
}