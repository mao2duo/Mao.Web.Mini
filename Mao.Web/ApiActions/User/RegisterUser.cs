using Mao.Repository;
using Mao.Web.Database.Models;
using Mao.Web.Features.Attributes;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Mao.Web.ApiActions.User
{
    public class RegisterUser
    {
        public class Request : IRequest<Response>
        {
            [Required]
            public string Account { get; set; }

            [Required]
            //[PasswordValidation]
            public string Password { get; set; }

            [Required]
            [FunctionValidation]
            public string PasswordConfirm { get; set; }

            [Required]
            public string DisplayName { get; set; }

            //[Required]
            public string Email { get; set; }

            public ValidationResult ValidatePasswordConfirm()
            {
                if (Password != PasswordConfirm)
                {
                    return new ValidationResult("密碼與確認密碼不相符", new[] { nameof(PasswordConfirm) });
                }
                return ValidationResult.Success;
            }
        }

        public class Response
        {
            public bool IsSuccessed { get; set; }
            public ModelStateDictionary ModelState { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            private readonly IMediator _mediator;
            public Handler(IRepository repository, IMediator mediator)
            {
                _repository = repository;
                _mediator = mediator;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response()
                {
                    ModelState = new ModelStateDictionary()
                };
                if (_repository.Count<AppUser>("Account", request.Account) > 0)
                {
                    response.ModelState.AddModelError("request.Account", "帳號已經存在");
                    return response;
                }
                AppUser user = new AppUser();
                user.Id = Guid.NewGuid();
                user.Account = request.Account;
                user.PasswordHash = (await _mediator.Send(new GetUserPasswordHash.Request()
                {
                    Account = request.Account,
                    Password = request.Password
                })).PasswordHash;
                user.Email = request.Email;
                user.DisplayName = request.DisplayName;
                _repository.Insert(user);
                response.IsSuccessed = true;
                return response;
            }
        }
    }
}