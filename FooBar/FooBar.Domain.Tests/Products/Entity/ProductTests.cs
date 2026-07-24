using FooBar.Domain.Exceptions;

namespace FooBar.Domain.Tests.Products.Entity
{
    public class ProductTests
    {
        const int MinimunLengthName = 3;
        const int MaximunLengthName = 100;

        [Fact]
        public void Product_WithNameNull_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductDataBuilder().WithName(default!).Build());

            Assert.Equal("the name should not be null or empty.", exception.Message);
        }

        [Fact]
        public void Product_WithNameMinimunLength_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductDataBuilder().WithName("Ad").Build());

            Assert.Equal($"the name should be between {MinimunLengthName} and {MaximunLengthName} characters.", exception.Message);
        }

        [Fact]
        public void Product_WithNameMaximunLength_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductDataBuilder().WithName("The output includes credentials that you must protect. Be sure that you do not include these credentials").Build());

            Assert.Equal($"the name should be between {MinimunLengthName} and {MaximunLengthName} characters.", exception.Message);
        }

        [Fact]
        public void Product_WithValueZero_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductDataBuilder().WithValue(0).Build());

            Assert.Equal("the value should be greater than zero.", exception.Message);
        }
    }
}
