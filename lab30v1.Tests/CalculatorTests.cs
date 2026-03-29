using System;
using Xunit;
using lab30v1;

namespace lab30v1.Tests
{
    public class CalculatorTests
    {
        private readonly Calculator _calculator = new Calculator();

        [Theory]
        [InlineData(5, 3, 8)]
        [InlineData(-5, -3, -8)]
        [InlineData(-5, 3, -2)]
        public void Add_ValidInputs_ReturnsCorrectSum(double a, double b, double expected)
        {
            double result = _calculator.Add(a, b);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(10, 4, 6)]
        [InlineData(0, 5, -5)]
        [InlineData(-5, -5, 0)]
        public void Subtract_ValidInputs_ReturnsCorrectDifference(double a, double b, double expected)
        {
            double result = _calculator.Subtract(a, b);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(4, 3, 12)]
        [InlineData(-2, 3, -6)]
        [InlineData(-2, -3, 6)]
        public void Multiply_ValidInputs_ReturnsCorrectProduct(double a, double b, double expected)
        {
            double result = _calculator.Multiply(a, b);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(10, 2, 5)]
        [InlineData(-10, 2, -5)]
        [InlineData(5, 2, 2.5)]
        public void Divide_ValidInputs_ReturnsCorrectQuotient(double a, double b, double expected)
        {
            double result = _calculator.Divide(a, b);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(10, 0));
        }

        [Fact]
        public void Power_PositiveExponent_ReturnsCorrectResult()
        {
            double result = _calculator.Power(2, 3);
            Assert.Equal(8, result);
        }

        [Fact]
        public void Power_ZeroExponent_ReturnsOne()
        {
            double result = _calculator.Power(5, 0);
            Assert.Equal(1, result);
        }

        [Fact]
        public void Power_NegativeExponent_ReturnsCorrectFraction()
        {
            double result = _calculator.Power(2, -2);
            Assert.Equal(0.25, result);
        }

        [Fact]
        public void Add_LargeNumbers_ReturnsCorrectSum()
        {
            double result = _calculator.Add(1e15, 1e15);
            Assert.Equal(2e15, result);
        }

        [Fact]
        public void Multiply_ByZero_ReturnsZero()
        {
            double result = _calculator.Multiply(999, 0);
            Assert.Equal(0, result);
        }
    }
}