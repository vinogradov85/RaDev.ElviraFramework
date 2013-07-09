using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;
using Microsoft.CSharp;

namespace RaDev.ElviraFramework.Templating
{
    /// <summary>
    /// Шаблонизатор Razor
    /// </summary>
    /// <typeparam name="T">Тип модели</typeparam>
    public sealed class RazorTemplater<T> where T : class
    {
        #region Переменные и вспомогательные классы

        /// <summary>
        /// Вспомогательный класс шаблона
        /// </summary>
        public abstract class TemplateBase
        {
            /// <summary>
            /// Поток вывода
            /// </summary>
            public TextWriter Output { get; set; }
            /// <summary>
            /// Модель
            /// </summary>
            public dynamic Model { get; set; }
            /// <summary>
            /// Выполнить
            /// </summary>
            public abstract void Execute();
            /// <summary>
            /// Вывод объекта
            /// </summary>
            /// <param name="value">Объект</param>
            public virtual void Write(object value)
            {
                Output.Write(value);
            }
            /// <summary>
            /// Вывод символа
            /// </summary>
            /// <param name="value">Символ</param>
            public virtual void WriteLiteral(object value)
            {
                Output.Write(value);
            }
        }

        /// <summary>
        /// Разор движек
        /// </summary>
        private readonly RazorEngineHost _razorHost = new RazorEngineHost(new CSharpRazorCodeLanguage());

        /// <summary>
        /// Модель
        /// </summary>
        private readonly T _model;

        /// <summary>
        /// Шаблон
        /// </summary>
        private readonly string _template;

        #endregion

        /// <summary>
        /// Шаблонизатор Razor
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="template">Шаблон</param>
        public RazorTemplater(T model, string template)
        {
            if (string.IsNullOrEmpty(template))
                throw new Exception("Шаблон пуст");

            _model = model;
            _template = template;
            _razorHost.DefaultBaseClass = typeof(TemplateBase).FullName;
            _razorHost.DefaultNamespace = GetType().Namespace;
            _razorHost.DefaultClassName = "Template";

            _razorHost.NamespaceImports.Add("System");
        }

        /// <summary>
        /// Выполнить
        /// </summary>
        /// <returns>Результат выполнения шаблона</returns>
        public string Execute()
        {
            var generatedCode = GetTemplate();
            var outAssembly = CompileTemplate(generatedCode);
            var template = outAssembly.CreateInstance(_razorHost.DefaultNamespace + "." + _razorHost.DefaultClassName) as TemplateBase;
            if (template != null)
            {
                using (var writer = new StringWriter())
                {
                    template.Output = writer;
                    template.Model = _model;
                    template.Execute();

                    return writer.ToString();
                }
            }
            throw new Exception("Ошибка создания шаблона");
        }

        #region Приватные методы

        /// <summary>
        /// Получить шаблон
        /// </summary>
        /// <returns>Шаблон</returns>
        private CodeCompileUnit GetTemplate()
        {
            var razorEngine = new RazorTemplateEngine(_razorHost);

            using (var reader = new StringReader(_template))
            {
                var generatorResults = razorEngine.GenerateCode(reader);
                if (!generatorResults.Success)
                    throw new Exception(string.Format("В шаблоне обнаружены ошибки {0}"
                                                      , string.Join(Environment.NewLine,
                                                                  generatorResults.ParserErrors.Select(
                                                                      error =>
                                                                      string.Format("Razor error: ({0}, {1}) {2}",
                                                                                    error.Location.LineIndex + 1,
                                                                                    error.Location.CharacterIndex + 1,
                                                                                    error.Message)))));
                return generatorResults.GeneratedCode;
            }
        }

        /// <summary>
        /// Скомпилировать шаблон
        /// </summary>
        /// <param name="generatedCode">Шаблон</param>
        /// <returns>Скомпилированная библиотека с шаблоном</returns>
        private Assembly CompileTemplate(CodeCompileUnit generatedCode)
        {
            var codeProvider = new CSharpCodeProvider();

            var refAssemblyNames = new List<string>
                {
                    new Uri(typeof (TemplateBase).Assembly.CodeBase).AbsolutePath,
                    "System.Core.dll",
                    "Microsoft.CSharp.dll",
                    new Uri(_model.GetType().Assembly.CodeBase).AbsolutePath
                };


            var compilerResults = codeProvider.CompileAssemblyFromDom(
                new CompilerParameters(refAssemblyNames.ToArray()),
                generatedCode);

            if (compilerResults.Errors.HasErrors)
            {
                var errors = compilerResults
                    .Errors
                    .OfType<CompilerError>()
                    .Where(x => !x.IsWarning);

                throw new Exception(string.Format("Ошибки компиляции шаблона {0}"
                                                  , string.Join(Environment.NewLine, errors.Select(
                                                      error => string.Format("Razor error: ({0}, {1}) {2}",
                                                                             error.Line,
                                                                             error.Column,
                                                                             error.ErrorText)))));
            }
            return compilerResults.CompiledAssembly;
        }

        #endregion
    }
}
