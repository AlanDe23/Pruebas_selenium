using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; 
using System;

namespace WebTrustTests.Tests
{
    public class BuscarUsuarioTests : BaseTest
    {
      
        private readonly int MaxWaitTimeInSeconds = 30;

        [Test]
        public void BuscarUsuario_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

         
            wait.Until(drv => drv.FindElement(By.CssSelector("button[data-tab='usuario']"))).Click();

     
            wait.Until(drv => drv.FindElement(By.Id("tab-usuario")).Displayed);

  
            wait.Until(drv => drv.FindElement(By.Id("filtroUsuario"))).SendKeys("1");

            wait.Until(drv => drv.FindElement(By.Id("btn-buscar-usuario"))).Click();

   
            wait.Until(drv =>
            {
                var lista = drv.FindElement(By.Id("lista-usuario")).Text;
                return lista.Length > 0;
            });

            Assert.Pass("Busqueda realizada correctamente");
        }

        [Test]
        public void BuscarUsuario_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/CrearAnalisis.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

      
            wait.Until(drv => drv.FindElement(By.CssSelector("button[data-tab='usuario']"))).Click();
            wait.Until(drv => drv.FindElement(By.Id("tab-usuario")).Displayed);

        
            driver.FindElement(By.Id("filtroUsuario")).Clear();


            driver.FindElement(By.Id("btn-buscar-usuario")).Click();

      
            wait.Until(drv =>
            {
                var msg = drv.FindElement(By.Id("mensaje-usuario")).Text;
                return msg.ToLower().Contains("id");
            });

            Assert.IsTrue(driver.FindElement(By.Id("mensaje-usuario")).Text.ToLower().Contains("id"));
        }
    }
    }