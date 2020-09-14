using FluentValidation;
using FluentValidation.Validators;
using MeetUp.Entity;
using MeetUp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MeetUp.Validation
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        private readonly MeetupContext _meetupContext;

        public RegisterUserValidator(MeetupContext meetupContext)
        {
            _meetupContext = meetupContext;
            
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
            RuleFor(x => x.Email).Custom((value, context) =>
            {
                var userExists = _meetupContext.Users.Any(u => u.Email == value);
                if (userExists)
                {
                    context.AddFailure("Email", "That email address has been taken");
                }
            });
            
        }
    }
}
