using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; // Necesario para WebDriverWait
using System;

namespace WebTrustTests.Tests
{
    public class ValidacionLimitesTests : BaseTest
    {
        // Define un tiempo máximo de espera
        private readonly int MaxWaitTimeInSeconds = 30;
        [Test]
        public void ValidarLimite_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.FindElement(By.Id("idUsuario")).SendKeys("1");
            driver.FindElement(By.Id("url")).SendKeys("https://ejemplo.com");
            driver.FindElement(By.Id("titulo")).SendKeys("Prueba");
            driver.FindElement(By.Id("porcentajeConfianza")).SendKeys("80"); // válido
            driver.FindElement(By.Id("detalles")).SendKeys("OK");

            driver.FindElement(By.Id("btn-crear")).Click();

            Thread.Sleep(1000);

            string html = driver.FindElement(By.Id("mensaje-crear")).Text;

            Assert.IsFalse(html.Contains("El porcentaje debe estar entre 0 y 100"));
        }
        [Test]
        public void ValidarLimite_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;


            js.ExecuteScript("document.getElementById('form-crear').setAttribute('novalidate', 'true');");

            js.ExecuteScript("document.getElementById('porcentajeConfianza').setAttribute('type','text');");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));


            driver.FindElement(By.Id("idUsuario")).Clear();
            driver.FindElement(By.Id("idUsuario")).SendKeys("1");

            driver.FindElement(By.Id("url")).Clear();
            driver.FindElement(By.Id("url")).SendKeys("https://ejemplo.com");

            driver.FindElement(By.Id("titulo")).Clear();
            driver.FindElement(By.Id("titulo")).SendKeys("Prueba");

            var por = driver.FindElement(By.Id("porcentajeConfianza"));
            por.Clear();
            por.SendKeys("150"); 

            driver.FindElement(By.Id("detalles")).Clear();
            driver.FindElement(By.Id("detalles")).SendKeys("detalle");

            driver.FindElement(By.Id("btn-crear")).Click();

       
            string mensaje = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("mensaje-crear"));
                return string.IsNullOrWhiteSpace(el.Text) ? null : el.Text;
            });

            Assert.IsTrue(
                mensaje.ToLower().Contains("porcentaje"),
                $"No llegó el mensaje esperado, mensaje real: '{mensaje}'"
            );
        }







    }
}