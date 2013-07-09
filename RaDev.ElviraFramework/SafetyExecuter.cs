using System;

namespace RaDev.ElviraFramework
{
    /// <summary>
    /// Выполнение метода или фунции в ошибкопезопасном контексте
    /// </summary>
    /// <typeparam name="TResult">Тип возвращаемого результата</typeparam>
    public class SafetyExecuter<TResult>
    {
        #region Переменные

        /// <summary>
        /// Действие
        /// </summary>
        protected Func<TResult> Action;

        #endregion

        /// <summary>
        /// Выполнение метода или фунции в ошибкопезопасном контексте
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="exceptionAction">Действие в случае ошибки</param>
        /// <returns>Результат</returns>
        public static TResult SafetyExecute(Func<TResult> action, Action<Exception> exceptionAction = null)
        {
            var se = new SafetyExecuter<TResult>(action);
            return se.Execute(exceptionAction);
        }

        /// <summary>
        /// Ошибкобезопасное выполнение
        /// </summary>
        /// <param name="action">Действие для выполнения</param>
        public SafetyExecuter(Func<TResult> action)
        {
            Action = action;
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        /// <param name="exceptionAction">Действие в случае ошибки</param>
        /// <returns>Результат</returns>
        public TResult Execute(Action<Exception> exceptionAction = null)
        {
            var result = default(TResult);
            if (Action != null)
            {
                try
                {
                    result = Action();
                }
                catch (Exception ex)
                {
                    if (exceptionAction != null)
                        exceptionAction(ex);
                }
            }
            return result;
        }
    }
}