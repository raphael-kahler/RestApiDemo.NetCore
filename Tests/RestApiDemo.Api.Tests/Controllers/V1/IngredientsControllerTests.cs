using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApiDemo.Api.Controllers.V1;
using RestApiDemo.Api.DTO.Converters;
using RestApiDemo.Domain;
using RestApiDemo.Framework;
using Xunit;

namespace RestApiDemo.Api.Tests.Contollers.V1
{
    public class IngredientsControllerTests
    {
        private string _ingredientName;
        private Ingredient _ingredient;
        private readonly Mock<IMealApplicationService> _mealService;
        private readonly DtoConverter _dtoConverter;

        public IngredientsControllerTests()
        {
            _ingredientName = "ingredient";
            _ingredient = new Ingredient(new IngredientId(_ingredientName), "ingredient");
            _dtoConverter = new DtoConverter(new Mock<IUrlHelper>().Object);

            _mealService = new Mock<IMealApplicationService>();
            _mealService.Setup(m => m.TryGetIngredient(_ingredientName, out _ingredient)).Returns(true);

        }

        [Fact]
        public void GetSingle_IngredientFound_ReturnsResult()
        {
            var sut = new IngredientsController(_mealService.Object, _dtoConverter);

            var response = sut.Get(_ingredientName);

            Assert.IsType<DTO.Response.Ingredient>(response.Value);
            Assert.NotNull(response.Value);
        }

        [Fact]
        public void GetSingle_IngredientNotFound_ReturnsNoResult()
        {
            var sut = new IngredientsController(_mealService.Object, _dtoConverter);

            var response = sut.Get("doesn't exist");

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public void Delete_IngredientFound_ReturnsOk()
        {
            _mealService.Setup(m => m.TryDeleteIngredient(_ingredientName)).Returns(true);
            var sut = new IngredientsController(_mealService.Object, _dtoConverter);

            var response = sut.Delete(_ingredientName);

            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void Delete_IngredientNotFound_ReturnsNotFound()
        {
            var sut = new IngredientsController(_mealService.Object, _dtoConverter);

            var response = sut.Delete("doesn't exist");

            Assert.IsType<NotFoundResult>(response);
        }
    }
}
