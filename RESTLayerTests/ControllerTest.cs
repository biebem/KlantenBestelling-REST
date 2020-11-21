using KlantenBestelling_REST.Controllers;
using DomainLayer;
using Moq;
using Xunit;
using System;
using Microsoft.AspNetCore.Mvc;
using KlantenBestelling_REST.BaseClasses;
using Microsoft.Extensions.Logging;

namespace RESTLayerTests
{
    public class ControllerTest
    {
        private readonly Mock<IDomainController> mockRepo;
        private readonly KBController kbController;

        public ControllerTest()
        {
            mockRepo = new Mock<IDomainController>();
            kbController = new KBController(mockRepo.Object, new LoggerFactory());
        }
        [Fact]
        public void GETClient_UnknownID_ReturnsNotFound()
        {
            mockRepo.Setup(repo => repo.GetClient(2))
                .Throws(new Exception("Client already in database."));
            var result = kbController.GetClient(2);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        [Fact]
        public void GETClient_CorrectID_ReturnsOkResult()
        {
            mockRepo.Setup(repo => repo.GetClient(2))
                    .Returns(new Client("bart", "simpsonlaan 12"));
            var result = kbController.GetClient(2);
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void GETClient_CorrectID_ReturnsRClientOut()
        {
            Client c = new Client("bart", "simpsonlaan 12");
            c.Id = 2;
            mockRepo.Setup(repo => repo.GetClient(2))
                    .Returns(c);
            var result = kbController.GetClient(2).Result as OkObjectResult;

            Assert.IsType<RClientOut>(result.Value);
            Assert.Equal(Constants.URI+2, (result.Value as RClientOut).ClientIdString);
            Assert.Equal(c.Name, (result.Value as RClientOut).Name);
            Assert.Equal(c.Address, (result.Value as RClientOut).Address);
            Assert.Equal(c.GetOrders().Count, (result.Value as RClientOut).Orders.Count);
        }
        [Fact]
        public void POSTClient_ValidObject_ReturnsCreatedAtAction()
        {
            RClientIn client = new RClientIn("trala", "simpsonlaan 12");
            Client clientRepo = new Client(client.Name, client.Address);
            mockRepo.Setup(repo => repo.AddClient(clientRepo)).Returns(clientRepo);
            //Client added = dc.AddClient(toAdd);
            var response = kbController.PostClient(client);
            Assert.IsType<CreatedAtActionResult>(response.Result);
        }
        [Fact]
        public void POSTClient_ValidObject_ReturnsCorrectItem()
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            c.ClientID = 2;
            Client clientRepo = new Client(c.Name, c.Address);
            clientRepo.Id = 2;
            mockRepo.Setup(repo => repo.AddClient(clientRepo)).Returns(clientRepo);
            var tussenResponse = kbController.PostClient(c);
            var response = tussenResponse.Result as CreatedAtActionResult;
            var item = response.Value as RClientOut;
            Assert.IsType<RClientOut>(item);
            Assert.Equal(Constants.URI + 2, item.ClientIdString);
            Assert.Equal(c.Name, item.Name);
            Assert.Equal(c.Address, item.Address);
        }
        [Fact]
        public void POSTClient_InValidObject_ReturnsNotFound()
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            kbController.ModelState.AddModelError("format error", "int expected");
            var response = kbController.PostClient(c).Result;
            Assert.IsType<NotFoundObjectResult>(response);
        }
        [Fact]
        public void PUTClient_InValidObject_ReturnsBadRequest() 
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            c.ClientID = 5;
            var response = kbController.PutClient(2, c);
            Assert.IsType<BadRequestResult>(response.Result);
        }
        [Fact]
        public void PUTClient_InValidObject_ReturnsNotFound() 
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            c.ClientID = 2;
            kbController.ModelState.AddModelError("simulated exception", "duno client already in db");
            var response = kbController.PutClient(2,c).Result;
            Assert.IsType<NotFoundObjectResult>(response);
        }
        [Fact]
        public void PUTClient_InValidId_ReturnsNotFound()
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            c.ClientID = 2;
            Client clientRepo = new Client(c.Name, c.Address);
            clientRepo.Id = 2;
            mockRepo.Setup(repo => repo.UpdateClient(clientRepo)).Throws(new Exception("Client not in DB."));
            var response = kbController.PutClient(2, c).Result;
            Assert.IsType<NotFoundObjectResult>(response);
        }
        [Fact]
        public void PUTClient_ValidObject_ReturnsCorrectItem() 
        {
            RClientIn c = new RClientIn("trala", "simpsonlaan 12");
            c.ClientID = 2;
            Client clientRepo = new Client(c.Name, c.Address);
            clientRepo.Id = 2;
            mockRepo.Setup(repo => repo.AddClient(clientRepo)).Returns(clientRepo);
            var tussenResponse = kbController.PutClient(2,c);
            var response = tussenResponse.Result as CreatedAtActionResult;
            var item = response.Value as RClientOut;
            Assert.IsType<RClientOut>(item);
            Assert.Equal(Constants.URI + 2, item.ClientIdString);
            Assert.Equal(c.Name, item.Name);
            Assert.Equal(c.Address, item.Address);
        }
        [Fact]
        public void DeleteClient_ValidObject_ReturnsNoContent() 
        {
            var result = kbController.DeleteClient(1);
            Assert.IsType<NoContentResult>(result);

        }
        [Fact]
        public void DeleteClient_InValidObject_ReturnsNotFound() 
        {
            kbController.ModelState.AddModelError("simulated exception", "duno client not in db");
            var result = kbController.DeleteClient(1);
            Assert.IsType<NoContentResult>(result);
        }


    }
}
