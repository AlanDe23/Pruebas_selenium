using System;
using System.Collections.Generic;
using System.IO;

namespace WebTrustTests.Reports
{
    public static class HtmlReportManager
    {
        private static readonly List<TestResultInfo> Results = new List<TestResultInfo>();

        public static void AddResult(string testName, bool passed, string errorMessage, string screenshotPath)
        {
            Results.Add(new TestResultInfo
            {
                TestName = testName,
                Passed = passed,
                ErrorMessage = errorMessage,
                ScreenshotPath = screenshotPath
            });
        }

        public static void GenerateHtmlReport()
        {
            string reportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportDir);

            string reportPath = Path.Combine(reportDir, "TestReport.html");

            using (StreamWriter sw = new StreamWriter(reportPath))
            {
                sw.WriteLine("<html><head><title>Reporte de Pruebas</title>");
                sw.WriteLine("<style>");
                sw.WriteLine("table { width: 100%; border-collapse: collapse; }");
                sw.WriteLine("th, td { padding: 10px; border: 1px solid #ddd; }");
                sw.WriteLine(".passed { background: #c8e6c9; }");
                sw.WriteLine(".failed { background: #ffcdd2; }");
                sw.WriteLine("</style>");
                sw.WriteLine("</head><body>");

                sw.WriteLine("<h1>Reporte de Pruebas Selenium</h1>");
                sw.WriteLine("<table>");
                sw.WriteLine("<tr><th>Prueba</th><th>Resultado</th><th>Error</th><th>Screenshot</th></tr>");

                foreach (var r in Results)
                {
                    string rowClass = r.Passed ? "passed" : "failed";
                    sw.WriteLine($"<tr class='{rowClass}'>");

                    sw.WriteLine($"<td>{r.TestName}</td>");
                    sw.WriteLine($"<td>{(r.Passed ? "PASSED" : "FAILED")}</td>");
                    sw.WriteLine($"<td>{r.ErrorMessage}</td>");

                    // 🔥 REEMPLAZA AQUÍ EL BLOQUE DE SCREENSHOT
                    if (!string.IsNullOrWhiteSpace(r.ScreenshotPath))
                    {
                        string relativePath = Path.GetRelativePath(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports"),
                            r.ScreenshotPath
                        ).Replace("\\", "/");

                        sw.WriteLine($"<td><a href='{relativePath}' target='_blank'>Ver</a></td>");
                    }
                    else
                    {
                        sw.WriteLine("<td>—</td>");
                    }

                    sw.WriteLine("</tr>");
                }

            }
        }

        private class TestResultInfo
        {
            public string TestName;
            public bool Passed;
            public string ErrorMessage;
            public string ScreenshotPath;
        }
    }
}
