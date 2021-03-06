using InpatientTherapySchedulingProgram.Controllers;
using InpatientTherapySchedulingProgram.Models;
using InpatientTherapySchedulingProgramTests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using InpatientTherapySchedulingProgram.Services;
using System;
using System.Linq;

namespace InpatientTherapySchedulingProgramTests.IntegrationTests
{
    [TestClass]
    public class UserServiceControllerTests
    {
        private List<User> _testUsers;
        private CoreDbContext _testContext;
        private UserService _testUserService;
        private UserController _testUserController;
        private User _nonActiveUser;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<CoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UserDatabase")
                .Options;
            _testUsers = new List<User>();
            _testContext = new CoreDbContext(options);
            _testContext.Database.EnsureDeleted();

            for(var i = 0; i < 10; i++)
            {
                var newUser = ModelFakes.UserFake.Generate();
                _testContext.Add(newUser);
                _testContext.SaveChanges();
                _testUsers.Add(ObjectExtensions.Copy(_testContext.User.FirstOrDefault(u => u.Username.Equals(newUser.Username))));
            }

            _nonActiveUser = ModelFakes.UserFake.Generate();
            _nonActiveUser.Active = false;
            _testContext.Add(_nonActiveUser);
            _testContext.SaveChanges();
            _testUsers.Add(ObjectExtensions.Copy(_testContext.User.FirstOrDefault(u => u.Username.Equals(_nonActiveUser.Username))));

            _testUserService = new UserService(_testContext);
            _testUserController = new UserController(_testUserService);
        }

        [TestMethod]
        public async Task ValidGetUserReturnsOkResponse()
        {
            var response = await _testUserController.GetUser();
            var responseResult = response.Result;

            responseResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ValidGetUserReturnsCorrectType()
        {
            var response = await _testUserController.GetUser();
            var responseResult = response.Result as OkObjectResult;

            responseResult.Value.Should().BeOfType<List<User>>();
        }

        [TestMethod]
        public async Task ValidGetUserReturnsCorrectCountOfUsers()
        {
            var response = await _testUserController.GetUser();
            var responseResult = response.Result as OkObjectResult;
            var listOfUsers = (List<User>)responseResult.Value;

            listOfUsers.Count.Should().Be(10);
        }

        [TestMethod]
        public async Task ValidGetUserReturnsCorrectUsers()
        {
            var response = await _testUserController.GetUser();
            var responseResult = response.Result as OkObjectResult;
            var listOfUsers = (List<User>)responseResult.Value;

            for(var i = 0; i < 10; i++)
            {
                _testUsers.Contains(listOfUsers[i]).Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task ValidGetUserByIdReturnsOkResponse()
        {
            var response = await _testUserController.GetUser(_testUsers[0].UserId);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ValidGetUserByIdReturnsCorrectType()
        {
            var response = await _testUserController.GetUser(_testUsers[0].UserId);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().BeOfType<User>();
        }

        [TestMethod]
        public async Task ValidGetUserByIdReturnsCorrectUser()
        {
            var response = await _testUserController.GetUser(_testUsers[0].UserId);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().Be(_testUsers[0]);
        }

        [TestMethod]
        public async Task NonExistingGetUserByIdReturnsNotFoundResponse()
        {
            var response = await _testUserController.GetUser(-1);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task ValidGetUserByUsernameReturnsOkResponse()
        {
            var response = await _testUserController.GetUser(_testUsers[0].Username);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ValidGetUserByUsernameReturnsCorrectType()
        {
            var response = await _testUserController.GetUser(_testUsers[0].Username);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().BeOfType<User>();
        }

        [TestMethod]
        public async Task ValidGetUserByUsernameReturnsCorrectUser()
        {
            var response = await _testUserController.GetUser(_testUsers[0].Username);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().Be(_testUsers[0]);
        }

        [TestMethod]
        public async Task NonExistingGetUserByUsernameReturnsNotFound()
        {
            var response = await _testUserController.GetUser("-1");
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task ValidPutUserReturnsNoContentResponse()
        {
            var response = await _testUserController.PutUser(_testUsers[0].UserId, _testUsers[0]);

            response.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task ValidLoginUserReturnsOkResponse()
        {
            var newUser = ModelFakes.UserFake.Generate();
            string nonHashedPassword = newUser.Password;

            await _testUserController.PostUser(newUser);

            newUser.Password = nonHashedPassword;

            var response = await _testUserController.LoginUser(newUser);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ValidLoginUserReturnsCorrectType()
        {
            var newUser = ModelFakes.UserFake.Generate();
            string nonHashedPassword = newUser.Password;

            await _testUserController.PostUser(newUser);

            newUser.Password = nonHashedPassword;

            var response = await _testUserController.LoginUser(newUser);
            var responseResult = response.Result as OkObjectResult;

            responseResult.Value.Should().BeOfType<User>();
        }

        [TestMethod]
        public async Task ValidLoginUserReturnsCorrectUser()
        {
            var newUser = ModelFakes.UserFake.Generate();
            string nonHashedPassword = newUser.Password;

            await _testUserController.PostUser(newUser);

            newUser.Password = nonHashedPassword;

            var response = await _testUserController.LoginUser(newUser);
            var responseResult = response.Result as OkObjectResult;

            responseResult.Value.Should().Be(newUser);
        }

        [TestMethod]
        public async Task NonActiveUserLoginUserReturnsNotFoundResponse()
        {
            var response = await _testUserController.LoginUser(_nonActiveUser);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task NonExistingUserLoginUserReturnsNotFoundResponse()
        {
            var fakeUser = ModelFakes.UserFake.Generate();

            var response = await _testUserController.LoginUser(fakeUser);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task ValidPutUserCorrectlyUpdatesData()
        {
            var oldUsername = _testUsers[0].Username;
            var newUsername = ModelFakes.UserFake.Generate().Username;
            _testUsers[0].Username = newUsername;

            await _testUserController.PutUser(_testUsers[0].UserId, _testUsers[0]);

            var response = await _testUserController.GetUser(_testUsers[0].UserId);
            var responseResult = response.Result as OkObjectResult;
            User user = (User)responseResult.Value;

            user.Username.Should().NotBe(oldUsername);
            user.Username.Should().Be(newUsername);
            user.Should().Be(_testUsers[0]);
        }

        [TestMethod]
        public async Task NonMatchingPutUserIdsShouldReturnBadRequest()
        {
            var response = await _testUserController.PutUser(-1, _testUsers[0]);

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task NonExistingPutUserShouldReturnBadRequest()
        {
            var fakeUser = new User();
            fakeUser.UserId = -1;

            var response = await _testUserController.PutUser(-1, fakeUser);

            response.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task ValidPostUserReturnsCreatedAtActionResponse()
        {
            var newUser = ModelFakes.UserFake.Generate();
            var response = await _testUserController.PostUser(newUser);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<CreatedAtActionResult>();
        }

        [TestMethod]
        public async Task ValidPostUserCorrectlyAddsUser()
        {
            var newUser = ModelFakes.UserFake.Generate();
            await _testUserController.PostUser(newUser);

            var response = await _testUserController.GetUser(newUser.UserId);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().Be(newUser);
        }

        [TestMethod]
        public async Task ExistingUsernamePostUserReturnsConflictResponse()
        {
            var newUser = ModelFakes.UserFake.Generate();
            newUser.Username = _testUsers[0].Username;

            var response = await _testUserController.PostUser(newUser);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<ConflictObjectResult>();
        }

        [TestMethod]
        public async Task ExistingUsernamePostUserDoesNotAddUser()
        {
            var newUser = ModelFakes.UserFake.Generate();
            newUser.Username = _testUsers[0].Username;

            await _testUserController.PostUser(newUser);

            var getResponse = await _testUserController.GetUser(newUser.Username);
            var getResponseResult = getResponse.Result as OkObjectResult;
            var getUser = getResponseResult.Value;

            getUser.Should().NotBe(newUser);
        }

        [TestMethod]
        public async Task ValidDeleteUserReturnsOkResponse()
        {
            var response = await _testUserController.DeleteUser(_testUsers[0].UserId);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ValidDeleteUserReturnsCorrectType()
        {
            var response = await _testUserController.DeleteUser(_testUsers[0].UserId);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value; 

            user.Should().BeOfType<User>();
        }

        [TestMethod]
        public async Task ValidDeleteUserReturnsCorrectUser()
        {
            var response = await _testUserController.DeleteUser(_testUsers[0].UserId);
            var responseResult = response.Result as OkObjectResult;
            var user = responseResult.Value;

            user.Should().Be(_testUsers[0]);
        }

        [TestMethod]
        public async Task ValidDeleteUserCorrectlyRemovesUser()
        {
            await _testUserController.DeleteUser(_testUsers[0].UserId);

            var response = await _testUserController.GetUser(_testUsers[0].UserId);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task NonExistingUserDeleteUserReturnsNotFoundResponse()
        {
            var response = await _testUserController.DeleteUser(-1);
            var responseResult = response.Result;

            responseResult.Should().BeOfType<NotFoundResult>();
        }
    }
}
