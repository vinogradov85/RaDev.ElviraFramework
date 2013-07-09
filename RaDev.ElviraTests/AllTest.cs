using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaDev.ElviraFramework;
using RaDev.ElviraFramework.Extensions;
using RaDev.ElviraFramework.Templating;

namespace RaDev.ElviraTests
{
    /// <summary>
    /// Проверка фреймворка
    /// </summary>
    [TestClass]
    public class AllTest
    {
        /// <summary>
        /// Проверка расширений HTML
        /// </summary>
        [TestMethod]
        [Description("Проверка SafetyExecuter")]
        public void TestSafetyExecuter()
        {
            var existException = false;
            SafetyExecuter<int>.SafetyExecute(() =>
            {
                var i = Math.Sqrt(25);
                var r = 5 - i;
                return (int)(i / r);
            }, ex => { existException = true; });
            Assert.IsFalse(existException, "Не верно работает SafetyExecuter при ошибке");

            const int intValue = 17;
            var result = SafetyExecuter<int>.SafetyExecute(() => intValue);
            Assert.IsTrue(intValue == result, "Не верно работает SafetyExecuter результате");
        }

        /// <summary>
        /// Проверка расширений HTML
        /// </summary>
        [TestMethod]
        [Description("Проверка расширений HTML")]
        public void TestHtmlExtension()
        {
            const string normalText = "Любой текст без хлама";
            const string textInject = "<script>alert('test!');</script> ";

            var noScriptResult = string.Join(textInject, normalText.Split(' '));
            Assert.IsTrue(noScriptResult.NoScript() == normalText, "Не верно работает NoScript()!");
            Assert.IsTrue(noScriptResult.ForLength(9).Length == 9, "Не верно работает ForLength()");
            Assert.IsTrue(string.Format("<p>{0}</p>", normalText).NoTags() == normalText, "Не верно работает NoTages()");
        }

        /// <summary>
        /// Проверка работы шаблонизатора Razor
        /// </summary>
        [TestMethod]
        [Description("Проверка RazorTemplater")]
        public void RazorTemplaterTest()
        {
            const string testResult = "Test this Method";
            const string templateText = "<p>@Model</p>";
            var razor = new RazorTemplater<string>(testResult, templateText);
            var result = razor.Execute().NoTags();
            Assert.IsTrue(result.ToLower() == testResult.ToLower(), "Шаблон работает не корректно");
        }
    }
}
