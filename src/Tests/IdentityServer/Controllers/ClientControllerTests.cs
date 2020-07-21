using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.Constants;
using IdentityServer.Controllers.Client;
using IdentityServer.Exceptions;
using IdentityServer.Extensions;
using IdentityServer.Models.ApiScope;
using IdentityServer.Models.Client;
using IdentityServer.Models.IdentityResource;
using IdentityServer.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace IdentityServer.Tests.Controllers
{
    [TestFixture]
    public class ClientControllerTests
    {
        private ClientController ClientController;
        private Mock<IClientService> ClientServiceMock;
        private Mock<IApiScopeService> ApiScopeServiceMock;
        private Mock<IIdentityResourceService> IdentityServiceMock;

        [SetUp]
        public void Setup()
        {
            ClientServiceMock = new Mock<IClientService>();
            ApiScopeServiceMock = new Mock<IApiScopeService>();
            IdentityServiceMock = new Mock<IIdentityResourceService>();

            ClientController = new ClientController(ClientServiceMock.Object, ApiScopeServiceMock.Object, IdentityServiceMock.Object);
        }

        [Test]
        public void WhenCallIndex_ThenShouldReturnIndexViewWithAllClients()
        {
            // Arrange
            var id = "1";
            var decryptedSecret = "secret 1";

            var clientsModel = new[]
            {
                CreateClientModel(id, decryptedSecret),
                CreateClientModel("2", "secret 2"),
                CreateClientModel("3", "secret 3"),
            };

            ClientServiceMock.Setup(service => service.GetAllClientsAsync()).ReturnsAsync(clientsModel.AsEnumerable());

            // Act
            var actionResult = ClientController.Index().GetAwaiter().GetResult() as ViewResult;
            var model = actionResult?.Model as IEnumerable<ClientModel>;

            // Assert
            ClientServiceMock.Verify(service => service.GetAllClientsAsync(), Times.Once);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Never);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(model);
            Assert.AreEqual(expected: 3, model.Count());
        }

        [Test]
        public void WhenCallEdit_AndItsANewClient_ThenShouldReturnEditViewWithEmptyModel()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());

            // Act
            var actionResult = ClientController.Edit(id: string.Empty).GetAwaiter().GetResult() as ViewResult;
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            // Assert
            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 3, viewDataScopes.Count());
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsTrue(viewDataScopes.Contains(apiName_2));
            Assert.IsTrue(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);

            Assert.IsNotNull(inputModel);
            Assert.IsFalse(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndItIsNotANewClient_AndHasScopeAndGrant_ThenShouldReturnEditViewWithClientInputModelAndViewData()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            var identity_1 = "Identity 1";
            var identity_2 = "Identity 2";
            var identity_3 = "Identity 3";
            var identity = new []
            {
                new IdentityResourceModel() { Name =  identity_1},
                new IdentityResourceModel() { Name =  identity_2},
                new IdentityResourceModel() { Name =  identity_3}
            };

            var clientId = "1";
            var clientSecret = "secret";
            var clientAllowedScopes = new List<string>() { apiName_3, identity_2 };


            var clientModel = CreateClientModel(clientId, clientSecret, clientAllowedScopes, GrantTypes.Code);

            this.ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());
            this.IdentityServiceMock.Setup(service => service.GetAllIdentityResourcesAsync()).ReturnsAsync(identity.AsEnumerable());
            this.ClientServiceMock.Setup(service => service.GetClientByInternalIdAsync(clientId)).ReturnsAsync(clientModel);

            // Act
            var actionResult = ClientController.Edit(clientId).GetAwaiter().GetResult() as ViewResult;

            // Assert
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Once);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 4, viewDataScopes.Count());
            
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsTrue(viewDataScopes.Contains(apiName_2));
            Assert.IsTrue(viewDataScopes.Contains(identity_1));
            Assert.IsTrue(viewDataScopes.Contains(identity_3));
            
            Assert.IsFalse(viewDataScopes.Contains(identity_2));
            Assert.IsFalse(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);
            Assert.IsFalse(viewDataGrantTypes.Contains(GrantTypes.Code.First()));

            Assert.IsNotNull(inputModel);
            Assert.IsTrue(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndItIsAPostChange_AndIsACancelCommand_ThenShouldRedirectToIndex()
        {
            // Arrange

            // Act
            var actionResult = ClientController.Edit(default(ClientInputModel), ControllerConstants.CANCEL, listValue: string.Empty).GetAwaiter().GetResult() as RedirectToActionResult;

            // Assert

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Never);
            Assert.AreEqual(expected: "Index", actionResult.ActionName);
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsAddScope_ThenShouldReturnEditViewWithClientInputModelAndViewData()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            var clientId = "1";
            var clientSecret = "secret";
            var clientAllowedScopes = new List<string>() { apiName_3 };

            var clientInputModel = CreateClientModel(clientId, clientSecret, clientAllowedScopes, GrantTypes.Code).ToInputModel();

            ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());

            // Act
            var button = $"{ClientController.ADD}-{ClientController.SCOPE}";
            var actionResult = ClientController.Edit(clientInputModel, button, apiName_2).GetAwaiter().GetResult() as ViewResult;

            // Assert
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 1, viewDataScopes.Count());
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsFalse(viewDataScopes.Contains(apiName_2));
            Assert.IsFalse(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);
            Assert.IsFalse(viewDataGrantTypes.Contains(GrantTypes.Code.First()));

            Assert.IsNotNull(inputModel);
            Assert.IsTrue(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsRemoveScope_ThenShouldReturnEditViewWithClientInputModelAndViewData()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            var clientId = "1";
            var clientSecret = "secret";
            var clientAllowedScopes = new List<string>() { apiName_3 };

            var clientInputModel = CreateClientModel(clientId, clientSecret, clientAllowedScopes, GrantTypes.Code).ToInputModel();

            ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());

            // Act
            var button = $"{ClientController.REMOVE}-{ClientController.SCOPE}";
            var actionResult = ClientController.Edit(clientInputModel, button, apiName_3).GetAwaiter().GetResult() as ViewResult;

            // Assert
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 3, viewDataScopes.Count());
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsTrue(viewDataScopes.Contains(apiName_2));
            Assert.IsTrue(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);
            Assert.IsFalse(viewDataGrantTypes.Contains(GrantTypes.Code.First()));

            Assert.IsNotNull(inputModel);
            Assert.IsTrue(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsRemoveGrant_ThenShouldReturnEditViewWithClientInputModelAndViewData()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            var clientId = "1";
            var clientSecret = "secret";
            var clientAllowedScopes = new List<string>() { apiName_3 };

            var clientInputModel = CreateClientModel(clientId, clientSecret, clientAllowedScopes, GrantTypes.Code).ToInputModel();

            ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());

            // Act
            var button = $"{ClientController.REMOVE}-{ClientController.GRANT}";
            var actionResult = ClientController.Edit(clientInputModel, button, GrantTypes.Code.First()).GetAwaiter().GetResult() as ViewResult;

            // Assert
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 2, viewDataScopes.Count());
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsTrue(viewDataScopes.Contains(apiName_2));
            Assert.IsFalse(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);
            Assert.IsTrue(viewDataGrantTypes.Contains(GrantTypes.Code.First()));

            Assert.IsNotNull(inputModel);
            Assert.IsTrue(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsAddGrant_ThenShouldReturnEditViewWithClientInputModelAndViewData()
        {
            // Arrange
            var apiName_1 = "Name 1";
            var apiName_2 = "Name 2";
            var apiName_3 = "Name 3";
            var scopes = new[]
            {
                new ApiScopeModel(id: "1", apiName_1, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "2", apiName_2, displayName: "name", description: string.Empty, enabled: true),
                new ApiScopeModel(id: "3", apiName_3, displayName: "name", description: string.Empty, enabled: true)
            };

            var clientId = "1";
            var clientSecret = "secret";
            var clientAllowedScopes = new List<string>() { apiName_3 };

            var clientInputModel = CreateClientModel(clientId, clientSecret, clientAllowedScopes, GrantTypes.Code).ToInputModel();

            ApiScopeServiceMock.Setup(service => service.GetAllApiScopesAsync()).ReturnsAsync(scopes.AsEnumerable());

            // Act
            var button = $"{ClientController.ADD}-{ClientController.GRANT}";
            var actionResult = ClientController.Edit(clientInputModel, button, GrantTypes.Hybrid.First()).GetAwaiter().GetResult() as ViewResult;

            // Assert
            var inputModel = actionResult?.Model as ClientInputModel;
            var viewDataScopes = actionResult?.ViewData["Scopes"] as IEnumerable<string>;
            var viewDataGrantTypes = actionResult?.ViewData["GrantTypes"] as IEnumerable<string>;

            ClientServiceMock.Verify(service => service.GetClientByInternalIdAsync(It.IsAny<string>()), Times.Never);
            ApiScopeServiceMock.Verify(service => service.GetAllApiScopesAsync(), Times.Once);

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);

            Assert.IsNotNull(viewDataScopes);
            Assert.AreEqual(expected: 2, viewDataScopes.Count());
            Assert.IsTrue(viewDataScopes.Contains(apiName_1));
            Assert.IsTrue(viewDataScopes.Contains(apiName_2));
            Assert.IsFalse(viewDataScopes.Contains(apiName_3));

            Assert.IsNotNull(viewDataGrantTypes);
            Assert.IsTrue(viewDataGrantTypes.Count() > 0);
            Assert.IsFalse(viewDataGrantTypes.Contains(GrantTypes.Code.First()));
            Assert.IsFalse(viewDataGrantTypes.Contains(GrantTypes.Hybrid.First()));

            Assert.IsNotNull(inputModel);
            Assert.IsTrue(CheckClientInputModel(inputModel));
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsSave_AndModelIsInvalid_ThenShouldReturnViewWithInvalidState()
        {
            // Arrange
            var inputModel = CreateClientModel("id", "secret").ToInputModel();

            // Act
            ClientController.ModelState.AddModelError("someError", "someMessage");
            var actionResult = ClientController.Edit(inputModel, ControllerConstants.SAVE, string.Empty).GetAwaiter().GetResult() as ViewResult;
            var model = actionResult?.Model as ClientInputModel;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(expected: ControllerConstants.EDIT, actionResult.ViewName);
            Assert.IsNotNull(model);
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsSave_AndThrowAlreadyExistsException_TheShouldReturnEditViewWithModelStateInvalid()
        {
            // Arrange
            var clientInputModel = CreateClientModel("1", "secret").ToInputModel();
            var alreadyExistsException = new AlreadyExistsException();
            alreadyExistsException.AddModelError("someKey", "someMessage");

            ClientServiceMock.Setup(service => service.UpsertClientAsync(It.IsAny<ClientInputModel>())).Throws(alreadyExistsException);

            // Act
            var actionResult = ClientController.Edit(clientInputModel, ControllerConstants.SAVE, string.Empty).GetAwaiter().GetResult() as ViewResult;
            var actionResultModel = actionResult?.Model as ClientInputModel;

            // Assert
            Assert.IsFalse(ClientController.ModelState.IsValid);
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(clientInputModel, actionResultModel);
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsSave_AndThrowException_TheShouldReturnEditViewWithModelStateInvalid()
        {
            // Arrange
            var clientInputModel = CreateClientModel("1", "secret").ToInputModel();
            ClientServiceMock.Setup(service => service.UpsertClientAsync(It.IsAny<ClientInputModel>())).Throws(new Exception());

            // Act
            var actionResult = ClientController.Edit(clientInputModel, ControllerConstants.SAVE, string.Empty).GetAwaiter().GetResult() as ViewResult;
            var actionResultModel = actionResult?.Model as ClientInputModel;

            // Assert
            Assert.IsFalse(ClientController.ModelState.IsValid);
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(clientInputModel, actionResultModel);
        }

        [Test]
        public void WhenCallEdit_AndTheCommandIsSave_AndThrowValidationException_TheShouldReturnEditViewWithModelStateInvalid()
        {
            // Arrange
            var clientInputModel = CreateClientModel("1", "secret").ToInputModel();
            var someKey = "someKey";
            var validationException = new ValidationException();
            validationException.AddValidation(() => true, someKey, "someMessage");
            ClientServiceMock.Setup(service => service.UpsertClientAsync(It.IsAny<ClientInputModel>())).Throws(validationException);

            // Act
            var actionResult = ClientController.Edit(clientInputModel, ControllerConstants.SAVE, string.Empty).GetAwaiter().GetResult() as ViewResult;
            var actionResultModel = actionResult?.Model as ClientInputModel;

            // Assert
            Assert.IsFalse(ClientController.ModelState.IsValid);
            Assert.IsTrue(ClientController.ModelState.ContainsKey(someKey));
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(clientInputModel, actionResultModel);
        }

        private bool CheckClientInputModel(ClientInputModel inputModel)
        {
            var properties = inputModel.GetType().GetProperties().Select(property => property.GetValue(inputModel));
            return properties.All(p => (p is IEnumerable<object> enumerable && enumerable.Count() > 0) || p != null);
        }

        private ClientModel CreateClientModel(string id,
            string secret,
            ICollection<string> allowedScopes = null,
            ICollection<string> allowedGrantTypes = null)
        {
            Random random = new Random();
            return new ClientModel(id, secret)
            {
                AllowedScopes = allowedScopes ?? new List<string>(),
                AllowedGrantTypes = allowedGrantTypes ?? new List<string>(),
                ClientId = random.Next().ToString(),
                ClientName = random.Next().ToString(),
                Description = random.Next().ToString(),
                RedirectUris = new List<string>() { random.Next().ToString() },
                PostLogoutRedirectUris = new List<string>() { random.Next().ToString() },
            };
        }
    }
}