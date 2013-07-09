using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace RaDev.ElviraFramework.Extensions
{
    /// <summary>
    /// Методы расширения для объектов и типов
    /// </summary>
    public static class ObjectAndTypeExtensions
    {
        #region Работа с конструктором

        /// <summary>
        /// Проверяет наличие конструктора без параметров для заданного типа
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <returns>Есть</returns>
        public static bool ExistConstructorWithParamsForType<T>()
        {
            var type = typeof(T);
            var constrs = type.GetConstructors();
            return constrs.Any(item => !item.GetParameters().Any());
        }

        /// <summary>
        /// Проверяет наличие конструктора с параметром определенного типа для заданного типа
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="paramType">Тип параметра</param>
        /// <returns>Есть</returns>
        public static bool ExistConstructorForType<T>(Type paramType)
        {
            var type = typeof(T);
            var constrs = type.GetConstructors();
            var ct = constrs.Where(item =>
                {
                    var param = item.GetParameters();
                    return param.Count() == 1 && param.Any(par => par.ParameterType.IsAssignableFrom(paramType));
                }).Count();
            return ct == 1;
        }

        #endregion

        #region Работа с атрибутами

        /// <summary>
        /// Проверяет помечен ли метод атрибутом
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="obj">Метод</param>
        /// <returns>Помечен или нет</returns>
        public static bool AttributeMarket<T>(this MemberInfo obj) where T : Attribute
        {
            return obj != null && obj.GetCustomAttributes(typeof(T), true).Any();
        }

        /// <summary>
        /// Ищет атрибуты объекта
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="obj">Объект</param>
        /// <returns>Набор атрибутов</returns>
        public static T[] GetAttributeMarket<T>(this Type obj) where T : Attribute
        {
            return (T[])(obj != null
                              ? obj.GetCustomAttributes(typeof(T), true)
                              : new T[0]);
        }

        /// <summary>
        /// Ищет атрибуты метода
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="obj">Объект</param>
        /// <returns>Набор атрибутов</returns>
        public static T[] GetAttributeMarket<T>(this MethodInfo obj) where T : Attribute
        {
            return (T[])(obj != null
                              ? obj.GetCustomAttributes(typeof(T), true)
                              : new T[0]);
        }

        #endregion

        #region Работа с наследованием

        /// <summary>
        /// Получить типы унаследованные от базового типа
        /// </summary>
        /// <typeparam name="T">Базовый тип</typeparam>
        /// <param name="containsSelf">Включая свой тип</param>
        /// <returns>Список типов</returns>
        public static IEnumerable<Type> AssignableTypes<T>(bool containsSelf = false) where T : class
        {
            var baseType = typeof(T);
            return AssignableTypes(baseType, containsSelf);
        }

        /// <summary>
        /// Получить типы унаследованные от базового типа
        /// </summary>
        /// <param name="fromType">Базовый тип</param>
        /// <param name="containsSelf">Включая свой тип</param>
        /// <returns>Список типов</returns>
        public static IEnumerable<Type> AssignableTypes(Type fromType, bool containsSelf = false)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Select(item => item.GetLoadableTypes())
                .Aggregate((IEnumerable<Type>)new Type[] { }, (sum, item) => sum.Union(item))
                .Where(item => fromType.IsAssignableFrom(item) && (fromType != item || containsSelf));
        }

        /// <summary>
        /// Проверить является ли объект экземпляром определенного типа
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="type">Тип для проверки</param>
        /// <returns>Является</returns>
        public static bool IsExemplarOf(this object obj, Type type)
        {
            return obj != null && type.IsInstanceOfType(obj);
        }

        #endregion

        #region Создание объекта

        /// <summary>
        /// Скопировать объект
        /// </summary>
        /// <param name="fromO">Объект приемник</param>
        /// <param name="toO">Объект шаблон</param>
        private static void CopyTo(this object fromO, object toO)
        {
            if (fromO == null || toO == null)
                return;

            var fromProps = fromO.GetType().Properties().ToArray();
            var toProps = toO.GetType().Properties();
            foreach (var toProp in toProps)
            {
                var fromProp = fromProps.FirstOrDefault(item => item.Name == toProp.Name);
                if (fromProp != null)
                {
                    toProp.SetValue(toO, fromProp.GetValue(fromO, null), null);
                }
            }
        }

        /// <summary>
        /// Создать объект на основе типа
        /// </summary>
        /// <typeparam name="T">Тип нового объекта</typeparam>
        /// <param name="obj">Объект шаблон</param>
        /// <param name="param">Параметры для создания объекта</param>
        /// <returns>Новый объект типа T</returns>
        public static T CreateFromObject<T>(object obj, object param) where T : class
        {
            if (obj == null)
                return null;

            T result;
            if (ExistConstructorWithParamsForType<T>())
                result = Activator.CreateInstance<T>();
            else if (ExistConstructorForType<T>(param.GetType()))
                result = Activator.CreateInstance(typeof(T), param) as T;
            else
                throw new Exception("Конструктор не найден для типа " + typeof(T) + " с параметрами типа " +
                                    param.GetType());
            obj.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Создать generic объект типа T
        /// </summary>
        /// <typeparam name="T">Тип generic</typeparam>
        /// <param name="param">Параметры</param>
        /// <param name="genericType">базовый generic тип</param>
        /// <returns>Экземпляр generic объекта</returns>
        /// <remarks>Если дженерика такого типа не существует в сборке, то он создается динамически</remarks>
        public static object GetGenericObject<T>(object param, Type genericType)
            where T : class
        {
            var type = typeof(T);
            var resultType = AssignableTypes(type).FirstOrDefault() ?? genericType.MakeGenericType(new[] { type });
            var resultObject = Activator.CreateInstance(resultType, param);
            return resultObject;
        }

        #endregion

        #region Работа со сборкой

        /// <summary>
        /// Получить загружаемые типы из сборки
        /// </summary>
        /// <param name="assembly">Сборка</param>
        /// <returns>Набор загружаемых типов</returns>
        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        #endregion

        #region Работа с типом объекта

        /// <summary>
        /// Получить все поля типа данных доступные для чтения и записи
        /// </summary>
        /// <param name="tp">Тип объекта</param>
        /// <returns>Список полей типа</returns>
        private static IEnumerable<PropertyInfo> Properties(this Type tp)
        {
            return tp == null ? null : tp.GetProperties().Where(item => item.CanWrite && item.CanRead).ToArray();
        }

        #endregion

        #region Сериализация

        /// <summary>
        /// Получить xml из объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Объекта</param>
        /// <returns>xml</returns>
        public static string ToXml<T>(this T obj) where T : class, new()
        {
            var memoryStream = new StringWriter();
            var xs = new XmlSerializer(typeof(T));
            xs.Serialize(memoryStream, obj);
            return memoryStream.ToString();
        }

        /// <summary>
        /// Получить объект из строки xml
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="str">Строка xml</param>
        /// <returns>Объект</returns>
        public static T FromXml<T>(this string str) where T : class, new()
        {
            var xmlTextReader = new StringReader(str);
            var xs = new XmlSerializer(typeof(T));
            return xs.Deserialize(xmlTextReader) as T;
        }

        /// <summary>
        /// Получить сериализованный объект
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Объекта</param>
        /// <returns>Сериализованый объект</returns>
        public static string ToBinary<T>(this T obj) where T : class, new()
        {
            var xobj = obj.ToXml();
            return xobj.Compress();
        }

        /// <summary>
        /// Получить объект из сериализованного объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="str">Сериализованный объект</param>
        /// <returns>Объект</returns>
        public static T FromBinary<T>(this string str) where T : class, new()
        {
            var xobj = str.Decompress();
            return xobj.FromXml<T>();
        }
        #endregion
    }
}