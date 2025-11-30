    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;

namespace WebTrustTests.Tests
{
    public class CrearAnalisisTests : BaseTest
    {
        private readonly int MaxWaitTimeInSeconds = 30;

        [Test]
        public void CrearAnalisis_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            wait.Until(d => d.FindElement(By.Id("idUsuario"))).SendKeys("1");
            driver.FindElement(By.Id("url")).SendKeys("https://ejemplo.com");
            driver.FindElement(By.Id("titulo")).SendKeys("Prueba Selenium");
            driver.FindElement(By.Id("porcentajeConfianza")).SendKeys("80");
            driver.FindElement(By.Id("detalles")).SendKeys("Análisis creado para prueba.");

            var crear = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("btn-crear"));
                return (el.Displayed && el.Enabled) ? el : null;
            });

            crear.Click();

            wait.Until(d => d.PageSource.Contains("creado correctamente"));

            Assert.IsTrue(driver.PageSource.Contains("creado correctamente"));
        }
        [Test]
        public void CrearAnalisis_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));


            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.getElementById('form-crear').setAttribute('novalidate','true');");

    
            wait.Until(drv => drv.FindElement(By.Id("idUsuario"))).SendKeys("1");

            driver.FindElement(By.Id("url")).Clear();

            driver.FindElement(By.Id("titulo")).SendKeys("Prueba negativa");

            driver.FindElement(By.Id("porcentajeConfianza")).SendKeys("50");
            driver.FindElement(By.Id("detalles")).SendKeys("Prueba de validación");

         
            js.ExecuteScript("document.getElementById('btn-crear').click();");

   
            wait.Until(drv =>
            {
                try
                {
                    var msg = drv.FindElement(By.Id("mensaje-crear")).Text;
                    return msg.Contains("URL");
                }
                catch { return false; }
            });

            Assert.IsTrue(driver.FindElement(By.Id("mensaje-crear")).Text.Contains("URL"));
        }
    }
   }