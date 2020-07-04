using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        private ClaimsUser? _claimsUser;
        public ClaimsUser ClaimsUser
        {
            get
            {
                if (_claimsUser == null)
                    _claimsUser = new ClaimsUser(User);

                return _claimsUser;
            }
        }

        protected bool InvalidModelState(out ActionResult invalidResponse)
        {
            if (ModelState.IsValid)
            {
                invalidResponse = null!;
                return false;
            }
            else
            {
                var reasons = ModelState
                     .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                     .SelectMany(x => x.Value.Errors.Select(y => new Reason(x.Key, y.ErrorMessage)));

                invalidResponse = BadRequest(reasons);
                return true;
            }
        }

        protected ActionResult BuildResponse(ReasonResult reasonResult)
        {
            switch (reasonResult.Status)
            {
                case ActionStatus.Success: return NoContent();
                case ActionStatus.BadRequest: return BadRequest(reasonResult.Reasons);
                case ActionStatus.NotFound: return NotFound(reasonResult.Reasons);
                case ActionStatus.Forbidden: return StatusCode(403, reasonResult.Reasons);
                default:
                    throw new Exception($"Unknown {nameof(ActionStatus)}: {reasonResult.Status}");
            }
        }

        protected ActionResult<T> BuildResponse<T>(ReasonResult<T> reasonResult)
            where T : class
        {
            switch (reasonResult.Status)
            {
                case ActionStatus.Success:
                    {
                        if (reasonResult.Data != null)
                            return Ok(reasonResult.Data);
                        else
                            return NoContent();
                    }
                case ActionStatus.BadRequest: return BadRequest(reasonResult.Reasons);
                case ActionStatus.NotFound: return NotFound(reasonResult.Reasons);
                case ActionStatus.Forbidden: return StatusCode(403, reasonResult.Reasons);
                default:
                    throw new Exception($"Unknown {nameof(ActionStatus)}: {reasonResult.Status}");
            }
        }
    }
}
