using System;
using System.Collections.Generic;

namespace Heart.Auth.Logic.Exceptions
{
    public class ReasonableException : Exception
    {
        public IEnumerable<Reason> Reasons { get; }

        public ReasonableException(IEnumerable<Reason> reasons)
        {
            Reasons = reasons;
        }

        public ReasonableException(params Reason[] reasons) : this(reasons as IEnumerable<Reason>)
        {
        }

        public class Reason
        {
            public string Code { get; set; }
            public string Description { get; set; }

            public Reason(string code, string description)
            {
                Code = code;
                Description = description;
            }
        }
    }
}