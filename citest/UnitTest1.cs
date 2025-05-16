using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit;

namespace lab3Test
{
    public class UnitTest1 : IDisposable
    {
        //comment for test
        //another one
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "https://opensource-demo.orangehrmlive.com";

        public UnitTest1()
        {
            var options = new ChromeOptions();

            var tempProfileDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempProfileDir);

            options.AddArgument($"--user-data-dir={tempProfileDir}");

            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                options.AddArgument("--headless");
            }

            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        public void SuccessfulLogin()
        {
            driver.Navigate().GoToUrl(baseUrl);
            wait.Until(d => d.FindElement(By.Name("username"))).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            wait.Until(d => d.Url.Contains("/dashboard"));
            Assert.Contains("/dashboard", driver.Url);
        }

        [Fact]
        public void InvalidPassword()
        {
            driver.Navigate().GoToUrl(baseUrl);
            wait.Until(d => d.FindElement(By.Name("username"))).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("wrongpassword");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var errorMessage = wait.Until(d => d.FindElement(By.CssSelector(".oxd-alert-content-text"))).Text;
            Assert.Equal("Invalid credentials", errorMessage);
        }

        [Fact]
        public void InvalidUsername()
        {
            driver.Navigate().GoToUrl(baseUrl);
            wait.Until(d => d.FindElement(By.Name("username"))).SendKeys("dmin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var errorMessage = wait.Until(d => d.FindElement(By.CssSelector(".oxd-alert-content-text"))).Text;
            Assert.Equal("Invalid credentials", errorMessage);
        }

        [Fact]
        public void RedirectToCorrectPageAfterLogin()
        {
            driver.Navigate().GoToUrl(baseUrl);
            wait.Until(d => d.FindElement(By.Name("username"))).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var dashboardHeader = wait.Until(d => d.FindElement(By.CssSelector("h6.oxd-topbar-header-breadcrumb-module"))).Text;
            Assert.Equal("Dashboard", dashboardHeader);
        }
    }
}
