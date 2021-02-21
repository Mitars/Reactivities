using Xunit;
using System.Threading.Tasks;
using Reactivities.Domain;
using Moq;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Interfaces;
using System.Threading;

namespace Reactivities.Application.User.ConfirmEmail.Tests
{
    public class CurrentUserTests
    {
        [Fact]
        public async Task CurrentUser_GetCurrentUser_CurrentUser()
        {
            var userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            var jwtGeneratorMock = new Mock<IJwtGenerator>();
            var userAccessorMock = new Mock<IUserAccessor>();

            var userName = "tim";
            var token = "12345678";

            var appUser = new AppUser
            {
                DisplayName = userName,
                UserName = userName
            };
            var refreshToken = new Domain.RefreshToken
            {
                Token = token
            };

            userAccessorMock.Setup(x => x.GetCurrentUserName()).Returns(userName);
            userManagerMock.Setup(x => x.FindByNameAsync(userName)).Returns(Task.FromResult(appUser));
            userManagerMock.Setup(x => x.UpdateAsync(appUser));
            jwtGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            jwtGeneratorMock.Setup(x => x.CreateToken(appUser)).Returns(token);

            var handler = new CurrentUser.Handler(userManagerMock.Object, jwtGeneratorMock.Object, userAccessorMock.Object);
            var result = await handler.Handle(new CurrentUser.Query(), CancellationToken.None);

            var userDto = new UserDto(appUser, jwtGeneratorMock.Object, string.Empty);

            Assert.Equal(result.DisplayName, userDto.DisplayName);
            Assert.Equal(result.UserName, userDto.UserName);
            Assert.Equal(result.Token, userDto.Token);
        }
    }
}