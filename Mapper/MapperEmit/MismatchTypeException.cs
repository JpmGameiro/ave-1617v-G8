using System;
using System.Runtime.Serialization;

namespace MapperEmit
{
    public class MismatchTypeException : Exception
    {
        public MismatchTypeException(string s) : base (s)
        {
            
        }
    }
}