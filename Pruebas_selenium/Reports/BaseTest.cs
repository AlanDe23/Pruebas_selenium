using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using WebTrustTests.Reports;

namespace WebTrustTests.Tests
{
    public class BaseTest
    {
        protected IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
        }

        [TearDown]
        public void TearDown()
        {
            string screenshotPath = "";
            bool passed = TestContext.CurrentContext.Result.Outcome.Status ==
                          NUnit.Framework.Interfaces.TestStatus.Passed;

            string testName = TestContext.CurrentContext.Test.Name;
            string errorMessage = passed ? "" : TestContext.CurrentContext.Result.Message;

            try
            {
                if (!passed)
                {
                    string screenshotsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                    Directory.CreateDirectory(screenshotsDir);

                    screenshotPath = Path.Combine(screenshotsDir, $"{testName}.png");

                    Screenshot shot = ((ITakesScreenshot)driver).GetScreenshot();
                    shot.SaveAsFile(screenshotPath);
                }
            }
            catch { }

            HtmlReportManager.AddResult(testName, passed, errorMessage, screenshotPath);

            try { driver.Quit(); } catch { }
            try { driver.Dispose(); } catch { }
        }

        [OneTimeTearDown]
        public void GenerateReport()
        {
            HtmlReportManager.GenerateHtmlReport();
            TestContext.WriteLine("📄 Reporte HTML generado.");
        }
    }
}
