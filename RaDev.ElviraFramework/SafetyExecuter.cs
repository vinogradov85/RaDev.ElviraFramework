using System;

namespace RaDev.ElviraFramework
{
    /// <summary>
    /// ���������� ������ ��� ������ � ���������������� ���������
    /// </summary>
    /// <typeparam name="TResult">��� ������������� ����������</typeparam>
    public class SafetyExecuter<TResult>
    {
        #region ����������

        /// <summary>
        /// ��������
        /// </summary>
        protected Func<TResult> Action;

        #endregion

        /// <summary>
        /// ���������� ������ ��� ������ � ���������������� ���������
        /// </summary>
        /// <param name="action">��������</param>
        /// <param name="exceptionAction">�������� � ������ ������</param>
        /// <returns>���������</returns>
        public static TResult SafetyExecute(Func<TResult> action, Action<Exception> exceptionAction = null)
        {
            var se = new SafetyExecuter<TResult>(action);
            return se.Execute(exceptionAction);
        }

        /// <summary>
        /// ���������������� ����������
        /// </summary>
        /// <param name="action">�������� ��� ����������</param>
        public SafetyExecuter(Func<TResult> action)
        {
            Action = action;
        }

        /// <summary>
        /// ��������� ��������
        /// </summary>
        /// <param name="exceptionAction">�������� � ������ ������</param>
        /// <returns>���������</returns>
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