using System;
using System.Collections.Concurrent;

namespace PowerLanguage
{
    /// <summary>
    /// Implements thread-safe global shares using instances of the ConcurrentDictionary type.  The
    /// dictionaries are designed to hold key,value pairs similar to TradeStation's GlobalVariable.dll.
    /// </summary>
    /// <remarks>
    /// The dictionaries are implemented with the parameterless constructor, which results in a 
    /// default initial dictionary capacity of 31 and a concurrency level of 4 * number of 
    /// processors.  These defaults were considered adequate for the purpose of this Globals class.
    /// As of .NET 4.5, the concurrency level is allowed to dynamically increase as more values are
    /// stored in a dictionary.  This dynamic addition of locks is lost if concurrency level is
    /// specified in the constructor.
    /// </remarks>
    public static class Globals
    {
        #region Fields
        private static ConcurrentDictionary<string, int> namedInteger = new ConcurrentDictionary<string, int>();
        private static ConcurrentDictionary<string, float> namedFloat = new ConcurrentDictionary<string, float>();
        private static ConcurrentDictionary<string, double> namedDouble = new ConcurrentDictionary<string, double>();
        private static ConcurrentDictionary<string, bool> namedBool = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, string> namedString = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<int, int> numberedInteger = new ConcurrentDictionary<int, int>();
        private static ConcurrentDictionary<int, float> numberedFloat = new ConcurrentDictionary<int, float>();
        private static ConcurrentDictionary<int, double> numberedDouble = new ConcurrentDictionary<int, double>();
        private static ConcurrentDictionary<int, bool> numberedBool = new ConcurrentDictionary<int, bool>();
        private static ConcurrentDictionary<int, string> numberedString = new ConcurrentDictionary<int, string>();
        #endregion

        /// <summary>
        /// Gets dictionary for storage of integer values accessed by string name keys
        /// </summary>
        public static ConcurrentDictionary<string, int> NamedInteger
        {
            get
            {
                return namedInteger;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of double values accessed by string name keys
        /// </summary>
        public static ConcurrentDictionary<string, double> NamedDouble
        {
            get
            {
                return namedDouble;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of float values accessed by string name keys
        /// </summary>
        public static ConcurrentDictionary<string, float> NamedFloat
        {
            get
            {
                return namedFloat;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of bool values accessed by string name keys
        /// </summary>
        public static ConcurrentDictionary<string, bool> NamedBool
        {
            get
            {
                return namedBool;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of string values accessed by string name keys
        /// </summary>
        public static ConcurrentDictionary<string, string> NamedString
        {
            get
            {
                return namedString;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of integer values accessed by integer index keys
        /// </summary>
        public static ConcurrentDictionary<int, int> NumberedInteger
        {
            get
            {
                return numberedInteger;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of float values accessed by integer index keys
        /// </summary>
        public static ConcurrentDictionary<int, float> NumberedFloat
        {
            get
            {
                return numberedFloat;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of double values accessed by integer index keys
        /// </summary>
        public static ConcurrentDictionary<int, double> NumberedDouble
        {
            get
            {
                return numberedDouble;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of bool values accessed by integer index keys
        /// </summary>
        public static ConcurrentDictionary<int, bool> NumberedBool
        {
            get
            {
                return numberedBool;
            }
        }

        /// <summary>
        /// Gets dictionary for storage of string values accessed by integer index keys
        /// </summary>
        public static ConcurrentDictionary<int, string> NumberedString
        {
            get
            {
                return numberedString;
            }
        }
    }
}
