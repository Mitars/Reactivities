using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Reactivities.Domain;
using Moq;
using Reactivities.Application.User;
using System.Threading;
using System;
using FluentValidation.TestHelper;

namespace Reactivities.ApplicationTests.User
{
    public class ConfirmEmailTests
    {
        [Fact]
        public async Task ConfirmEmail_ValidRequest_Succeeds()
        {
            var request = new ConfirmEmail.Command
            {
                Token = Guid.NewGuid().ToString(),
                Email = "test@test.com",
            };

            var appUser = new AppUser();

            var userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            
            userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).Returns(Task.FromResult(appUser));
            userManagerMock.Setup(x => x.ConfirmEmailAsync(appUser, It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

            var handler = new ConfirmEmail.Handler(userManagerMock.Object);
            var result = await handler.Handle(request, CancellationToken.None);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void ConfirmEmailCommand_EmptyValues_ValidationError()
        {
            var request = new ConfirmEmail.Command
            {
                Token = string.Empty,
                Email = string.Empty,
            };

            var validator = new ConfirmEmail.CommandValidator();
            var result = validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(person => person.Email);
            result.ShouldHaveValidationErrorFor(person => person.Token);
        }
    }
}