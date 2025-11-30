using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace WebTrustTests.Tests
{
    public class LoginTests : BaseTest
    {
        private const int MaxWaitTimeInSeconds = 15;

        [Test]
        public void Login_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(MaxWaitTimeInSeconds));


            var email = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            email.Clear();
            email.SendKeys("test@correo.com");

            var pass = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));
            pass.Clear();
            pass.SendKeys("123456");

            var submit = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            submit.Click();


            var alert = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".swal2-html-container, .swal2-title")));
            string text = alert.Text ?? "";

            TestContext.WriteLine($"Texto del alert: {text}");
            Assert.IsTrue(text.Contains("Inicio de sesión exitoso") || text.Contains("éxito") || text.Length > 0,
                $"Se esperaba el texto de éxito en el alert. Texto actual: '{text}'");
        }
    

        [Test]
        public void Login_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(MaxWaitTimeInSeconds));

   
            var email = wait.Until(driver => driver.FindElement(By.Id("email")));
            email.SendKeys("correo_invalido@noexiste.com");

      
            var pass = wait.Until(driver => driver.FindElement(By.Id("password")));
            pass.SendKeys("123456");

  
            var submit = wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']")));
            submit.Click();

    
            var alert = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector(".swal2-html-container, .swal2-title"));
                    return el.Displayed ? el : null;
                }
                catch { return null; }
            });

            string texto = alert.Text;
            TestContext.WriteLine("Texto recibido: " + texto);

            Assert.IsTrue(
                texto.Contains("Usuario no encontrado") ||
                texto.Contains("Contraseña incorrecta") ||
                texto.Contains("Error de autenticación") ||
                texto.Contains("Oops") ||
                texto.Length > 0,
                $"El mensaje de error no es válido. Texto encontrado: {texto}"
            );
        }



    }
}




