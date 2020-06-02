using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECommerce.Web;
using ECommerce.Web.Controllers;

namespace ECommerce.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            ProdutoVitrineController vitrine = new ProdutoVitrineController();

            //VitrineController vitrine = new VitrineController();

            //ViewResult result = vitrine.Index(1, 10) as ViewResult;

            /*
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            */
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
