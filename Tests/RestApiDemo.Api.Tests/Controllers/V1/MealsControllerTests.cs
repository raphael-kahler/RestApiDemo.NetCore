using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApiDemo.Api.Controllers.V1;
using RestApiDemo.Api.DTO.Converters;
using RestApiDemo.Domain;
using RestApiDemo.Domain.Values;
using RestApiDemo.Framework;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RestApiDemo.Api.Tests.Controllers.V1
{
    public class MealsControllerTests
    {
        private Meal _meal;
        private readonly Mock<IMealApplicationService> _mealService;
        private readonly DtoConverter _dtoConverter;

        public MealsControllerTests()
        {
            _dtoConverter = new DtoConverter(new Mock<IUrlHelper>().Object);
            _meal = new Meal(1, new MealName("meal"), new List<MealIngredient>(), new CookingInstructions("text"), new ServingSize(1));

            _mealService = new Mock<IMealApplicationService>();
            _mealService.Setup(m => m.TryGetMeal(1, out _meal)).Returns(true);
        }

        [Fact]
        public void GetSingle_MealFound_ReturnsResult()
        {
            var sut = new MealsController(_mealService.Object, _dtoConverter);

            var response = sut.Get(id: 1, peopleToFeed: null);

            Assert.IsType<DTO.Response.Meal>(response.Value);
            Assert.NotNull(response.Value);
        }

        [Fact]
        public void GetSingle_MealNotFound_ReturnsNotFoundResult()
        {
            var sut = new MealsController(_mealService.Object, _dtoConverter);

            var response = sut.Get(id: 2, peopleToFeed: null);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public void GetAll_MealsExist_ReturnsNonEmptyCollection()
        {
            var numMeals = 1;
            var mealService = new Mock<IMealApplicationService>();
            mealService.Setup(m => m.GetMeals(It.IsAny<int>(), It.IsAny<int>(), out numMeals)).Returns(new List<Meal> { _meal });
            var sut = new MealsController(mealService.Object, _dtoConverter);

            var response = sut.Get(count: 10, offset: 0);

            Assert.NotEmpty(response.Value.Items);
        }

        [Fact]
        public void GetAll_MealsExist_CountAndActualNumberOfItemsMatch()
        {
            var numMeals = 1;
            var mealService = new Mock<IMealApplicationService>();
            mealService.Setup(m => m.GetMeals(It.IsAny<int>(), It.IsAny<int>(), out numMeals)).Returns(new List<Meal> { _meal });
            var sut = new MealsController(mealService.Object, _dtoConverter);

            var response = sut.Get(count: 10, offset: 0);

            Assert.Equal(response.Value.Count, response.Value.Items.Count());
        }

        [Fact]
        public void GetAll_NoMealsExist_ReturnsEmptyCollection()
        {
            var sut = new MealsController(new Mock<IMealApplicationService>().Object, _dtoConverter);

            var response = sut.Get(count: 10, offset: 0);

            Assert.Empty(response.Value.Items);
        }

        [Fact]
        public void Delete_MealFound_ReturnsOk()
        {
            _mealService.Setup(m => m.TryDeleteMeal(1)).Returns(true);
            var sut = new MealsController(_mealService.Object, _dtoConverter);
            var response = sut.Delete(1);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void Delete_MealNotFound_ReturnsNotFound()
        {
            var sut = new MealsController(_mealService.Object, _dtoConverter);
            var response = sut.Delete(2);
            Assert.IsType<NotFoundObjectResult>(response);
        }
    }
}
