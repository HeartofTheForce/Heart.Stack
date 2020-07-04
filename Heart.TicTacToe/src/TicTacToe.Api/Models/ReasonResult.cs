using System.Collections.Generic;

namespace TicTacToe.Api.Models
{
    public class ReasonResult
    {
        public ActionStatus Status { get; private set; }
        public IEnumerable<Reason>? Reasons { get; private set; }

        protected ReasonResult() { }

        public static ReasonResult Success() => new ReasonResult() { Status = ActionStatus.Success };
        public static ReasonResult BadRequest(IEnumerable<Reason> reasons) => new ReasonResult()
        {
            Status = ActionStatus.BadRequest,
            Reasons = reasons,
        };
        public static ReasonResult NotFound() => new ReasonResult() { Status = ActionStatus.NotFound };
        public static ReasonResult Forbidden(IEnumerable<Reason>? reasons = null) => new ReasonResult()
        {
            Status = ActionStatus.Forbidden,
            Reasons = reasons,
        };
    }

    public class ReasonResult<T>
        where T : class
    {
        public T? Data { get; private set; }
        public ActionStatus Status { get; private set; }
        public IEnumerable<Reason>? Reasons { get; private set; }

        protected ReasonResult() { }

        public static ReasonResult<T> Success(T data) => new ReasonResult<T>()
        {
            Status = ActionStatus.Success,
            Data = data,
        };
        public static ReasonResult<T> BadRequest(IEnumerable<Reason> reasons) => new ReasonResult<T>()
        {
            Status = ActionStatus.BadRequest,
            Reasons = reasons,
        };
        public static ReasonResult<T> NotFound() => new ReasonResult<T>() { Status = ActionStatus.NotFound };
        public static ReasonResult<T> Forbidden(IEnumerable<Reason>? reasons = null) => new ReasonResult<T>()
        {
            Status = ActionStatus.Forbidden,
            Reasons = reasons,
        };
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

    public enum ActionStatus
    {
        Success = 0,
        BadRequest = 1,
        NotFound = 2,
        Forbidden = 3,
    }
}
