using System;

namespace UnityMiscExceptions
{
    [Serializable]
    class ExistingSingletonException : Exception
    {
        /// <summary>
        /// An exception that is thrown when a singleton instance is already globally set but attempts to be set again
        /// </summary>
        public ExistingSingletonException()
        {

        }

        /// <summary>
        /// An exception that is thrown when a singleton instance is already globally set but attempts to be set again
        /// </summary>
        /// <param name="type">Name of singleton type</param>
        public ExistingSingletonException(string type) : base(String.Format("A singleton of this type already exists: {0}", type))
        {

        }

    }
}