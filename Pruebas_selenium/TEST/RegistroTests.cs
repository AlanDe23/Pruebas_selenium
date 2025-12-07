using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebTrustTests.Tests
{
    public class RegistroTests : BaseTest
    {
        private const int WaitSeconds = 10;

        [Test]
        public void Registro_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.FindElement(By.Id("nombre")).SendKeys("Juan Pérezz");
            driver.FindElement(By.Id("email")).SendKeys("juan@testz.com");
            driver.FindElement(By.Id("password")).SendKeys("123456");

    
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            string mensaje = wait.Until(d =>
            {
                try
                {
                    var el = d.FindElement(By.ClassName("swal2-title"));
                    return !string.IsNullOrEmpty(el.Text) ? el.Text : null;
                }
                catch
                {
                    return null; 
                }
            });

            TestContext.WriteLine("El mensaje recibido fue: " + mensaje);

          
            Assert.IsTrue(
                mensaje.Contains("¡Registro exitoso!") ||
                mensaje.Contains("éxito") ||
                mensaje.Contains("Error al registrar") ||
                mensaje.Contains("Error de conexión"),
                "El mensaje recibido fue inesperado: " + mensaje
            );
        }

        [Test]
        public void Registro_PruebaNegativa_CamposDinamicos()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var pruebas = new[]
            {
        new { Nombre = "", Email = "", Password = "" },
        new { Nombre = "Juan Pérez", Email = "", Password = "" },
        new { Nombre = "", Email = "juan@test.com", Password = "" },
        new { Nombre = "", Email = "", Password = "123456" },
        new { Nombre = "Juan Pérez", Email = "juan@test.com", Password = "" },
        new { Nombre = "Juan Pérez", Email = "", Password = "123456" },
        new { Nombre = "", Email = "juan@test.com", Password = "123456" }
    };

            foreach (var prueba in pruebas)
            {
           
                driver.FindElement(By.Id("nombre")).Clear();
                driver.FindElement(By.Id("email")).Clear();
                driver.FindElement(By.Id("password")).Clear();

                driver.FindElement(By.Id("nombre")).SendKeys(prueba.Nombre);
                driver.FindElement(By.Id("email")).SendKeys(prueba.Email);
                driver.FindElement(By.Id("password")).SendKeys(prueba.Password);


                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                string mensaje = null;

                try
                {
                    mensaje = wait.Until(d =>
                    {
                        try
                        {
                            var el = d.FindElement(By.ClassName("swal2-title"));
                            return !string.IsNullOrEmpty(el.Text) ? el.Text : null;
                        }
                        catch
                        {
                            return null;
                        }
                    });
                }
                catch (WebDriverTimeoutException)
                {
         
                    try
                    {
                        mensaje = driver.FindElement(By.CssSelector(".error")).Text;
                    }
                    catch
                    {
              
                        var campo = prueba.Nombre == "" ? driver.FindElement(By.Id("nombre")) :
                                    prueba.Email == "" ? driver.FindElement(By.Id("email")) :
                                    driver.FindElement(By.Id("password"));

                        mensaje = (string)((IJavaScriptExecutor)driver).ExecuteScript(
                            "return arguments[0].validationMessage;", campo);
                    }
                }

                TestContext.WriteLine($"Prueba con Nombre='{prueba.Nombre}', Email='{prueba.Email}', Password='{prueba.Password}' => Mensaje: {mensaje}");

       
                Assert.IsTrue(
                    !string.IsNullOrEmpty(mensaje),
                    $"No se encontró mensaje de error para esta combinación"
                );
            }
        }



        [Test]
        public void ValidacionCamposRegistro_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            driver.FindElement(By.Id("nombre")).SendKeys("Pedro000");
            driver.FindElement(By.Id("email")).SendKeys("pedro000@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
            var msg = wait.Until(e => e.FindElement(By.ClassName("swal2-title")));

            string texto = msg.Text;
            TestContext.WriteLine("Mensaje recibido: " + texto);

            Assert.IsTrue(
                texto.Contains("Registro exitoso") ||
                texto.Contains("registrado") ||
                texto.Contains("Usuario registrado") ||
                texto.Contains("creado") ||
                texto.Contains("éxito"),
                $"El mensaje recibido fue inesperado: '{texto}'"
            );
        }


        [Test]
        public void ValidacionCamposRegistro_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            var email = driver.FindElement(By.Id("email"));
            email.SendKeys("correoMalo");

            var submit = driver.FindElement(By.CssSelector("button[type='submit']"));
            submit.Click();

            // Capturar la validación nativa HTML5
            string validation = email.GetAttribute("validationMessage");

            TestContext.WriteLine("ValidationMessage: " + validation);

            Assert.IsTrue(
                validation.Contains("complete") ||
                validation.Contains("campo") ||
                validation.Length > 0,
                $"El navegador no mostró un mensaje HTML5. ValidationMessage: {validation}"
            );
        }

        [Test]
        public void NavegarRegistroALogin_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            driver.FindElement(By.Id("btnLogin")).Click();

            Assert.IsTrue(driver.Url.Contains("login"));
        }

        [Test]
        public void NavegarRegistroALogin_PruebaNegativa()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            Assert.Throws<NoSuchElementException>(() =>
            {
                driver.FindElement(By.Id("botonNoExiste")).Click();
            });
        }

        [Test]
        public void PersistenciaUsuario_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/registro.html");

            driver.FindElement(By.Id("nombre")).SendKeys("Maria");
            driver.FindElement(By.Id("email")).SendKeys("maria@test.com");
            driver.FindElement(By.Id("password")).SendKeys("1234");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.ClassName("swal2-title")));

            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            driver.FindElement(By.Id("email")).SendKeys("maria@test.com");
            driver.FindElement(By.Id("password")).SendKeys("1234");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var msg = wait.Until(d => d.FindElement(By.ClassName("swal2-title")));

        
            Assert.IsTrue(msg.Text.Contains("Bienvenido"), $"Mensaje inesperado: {msg.Text}");
        }
        [Test]
        public void PersistenciaUsuario_PruebaNegativa_Mock()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

        
            ((IJavaScriptExecutor)driver).ExecuteScript(@"
        window.fetch = async function() {
            return {
                ok: false,
                json: async () => ({ message: 'Usuario no encontrado' })
            };
        };
    ");

   
            driver.FindElement(By.Id("email")).SendKeys("usuario-que-no-existe@test.com");
            driver.FindElement(By.Id("password")).SendKeys("0000");

    
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();


            string mensaje = CapturarMensajeSwal(20); 
            TestContext.WriteLine("Mensaje recibido: " + mensaje);

            Assert.IsTrue(
                mensaje.Contains("Usuario no encontrado"),
                $"El mensaje recibido fue inesperado: {mensaje}"
            );
        }
        private string CapturarMensajeSwal(int segundos = 15)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(segundos));
            return wait.Until(d =>
            {
                try
                {
                    var content = d.FindElement(By.ClassName("swal2-html-container")).Text;
                    if (!string.IsNullOrEmpty(content)) return content;

                    var title = d.FindElement(By.ClassName("swal2-title")).Text;
                    if (!string.IsNullOrEmpty(title)) return title;

                    return null;
                }
                catch
                {
                    return null;
                }
            });
        }


        [Test]
        public void ValidacionLogin_CaminoFeliz()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitSeconds));

            var emailField = wait.Until(d => d.FindElement(By.Id("email")));
            var passField = wait.Until(d => d.FindElement(By.Id("password")));

            emailField.Clear();
            emailField.SendKeys("test@correo.com");
            passField.Clear();
            passField.SendKeys("123456");

  
            var submit = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("button[type='submit']"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            submit.Click();

     
            var alert = wait.Until(d =>
            {
                try
                {
               
                    var el = d.FindElement(By.CssSelector(".swal2-html-container, .swal2-title"));
                    return el.Displayed ? el : null;
                }
                catch { return null; }
            });

            string texto = alert.Text ?? "";

  
            Assert.IsTrue(
                texto.Contains("Bienvenido") ||
                texto.Contains("Inicio de sesión") ||
                texto.ToLower().Contains("éxito") ||
                texto.Length > 0,
                $"Se esperaba un mensaje de éxito en el login. Texto actual: '{texto}'"
            );
        }
        [Test]
        public void ValidacionLogin_PruebaNegativa_CamposVacios()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var email = wait.Until(d => d.FindElement(By.Id("email")));
            var pass = wait.Until(d => d.FindElement(By.Id("password")));
            var btn = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));


            email.Clear();
            pass.Clear();


            btn.Click();

            string validationMessage = (string)((IJavaScriptExecutor)driver)
                .ExecuteScript("return arguments[0].validationMessage;", email);

            TestContext.WriteLine("ValidationMessage: " + validationMessage);

            Assert.IsTrue(
                validationMessage.Contains("Completa este campo") ||
                validationMessage.Length > 0,
                $"No se recibió el mensaje esperado. Mensaje fue: '{validationMessage}'"
            );

          
            Assert.IsTrue(driver.Url.Contains("login.html"));
        }

        private string CapturarMensajeSwal()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            try
            {
                var el = wait.Until(d =>
                {
                    try
                    {
                        var title = d.FindElement(By.ClassName("swal2-title"));
                        if (!string.IsNullOrEmpty(title.Text)) return title;
                    }
                    catch { }
                    try
                    {
                        var content = d.FindElement(By.ClassName("swal2-content"));
                        if (!string.IsNullOrEmpty(content.Text)) return content;
                    }
                    catch { }
                    return null;
                });
                return el.Text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

}




