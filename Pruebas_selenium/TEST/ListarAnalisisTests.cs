using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; // Necesario para WebDriverWait
using System;

namespace WebTrustTests.Tests
{
    public class ListarAnalisisTests : BaseTest
    {

        private readonly int MaxWaitTimeInSeconds = 30;
        [Test]
        public void Listar_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));

     
            wait.Until(d => d.FindElement(By.CssSelector("button[data-tab='listar']"))).Click();


            wait.Until(d => d.FindElement(By.Id("tab-listar")).Displayed);

 
            wait.Until(d => d.FindElement(By.Id("btn-actualizar"))).Click();


            wait.Until(d =>
            {
                var cont = d.FindElement(By.Id("lista-analisis"));
                return cont.Text.Length > 0; // Cualquier contenido
            });

            Assert.Pass("Listado mostrado correctamente");
        }

        [Test]
        public void Listar_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(MaxWaitTimeInSeconds));

            SafeFind(wait, By.CssSelector("button[data-tab='listar']")).Click();
            wait.Until(drv => drv.FindElement(By.Id("tab-listar")).Displayed);

  
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string script =
                "window._originalFetch = window.fetch;" +
                "window.fetch = function() { return Promise.reject(new Error('Simulated network error')); };";
            js.ExecuteScript(script);

        
            var btnActualizar = SafeFind(wait, By.Id("btn-actualizar"));
            btnActualizar.Click();


            bool errorDisplayed = wait.Until(drv =>
            {
                try
                {
                    var cont = drv.FindElement(By.Id("mensaje-listar"));
                    var txt = cont.Text ?? "";

                    return txt.Contains("Error") || txt.Contains("Error al cargar") || txt.Contains("❌");
                }
                catch
                {
                    return false;
                }
            });


            js.ExecuteScript("if(window._originalFetch) window.fetch = window._originalFetch;");

            Assert.IsTrue(errorDisplayed, "No se mostró mensaje de error tras simular fallo de fetch. Revisa el frontend.");
        }

        private IWebElement SafeFind(WebDriverWait wait, By by)
        {
            return wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(by);
                    return el.Displayed ? el : null;
                }
                catch
                {
                    return null;
                }
            });
        }
    }

}
