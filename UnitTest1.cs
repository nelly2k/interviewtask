using System;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Nelli
{
    public class UnitTest1
    {
        [Fact]
        public void SimplePriceStrategyTests()
        {
            new SimpleStrategy
            {
                Price = 0.65m
            }.Calc(10).ShouldBe(6.5m);

        }

        [Theory]
        [InlineData(1, 1.25)]
        [InlineData(3, 3)]
        [InlineData(4, 4.25)]
        [InlineData(7, 7.25)]
        public void FixedPriceForAmountStrategyTests(int count, decimal result)
        {
            new FixedPriceForAmountStrategy
            {
                FixedAmount = 3,
                FixedPrice = 3m,
                Price = 1.25m
            }.Calc(count).ShouldBe(result);
        }

        [Theory]
        [InlineData(1, 2.1)]
        [InlineData(2, 4.2)]
        [InlineData(3, 4.2)]
        [InlineData(5, 8.4)]
        [InlineData(6, 8.4)]
        public void FreeForFixedAmountStrategyTests(int count, decimal result)
        {
            new FreeForFixedAmountStrategy
            {
                Price = 2.1m,
                FreeAmount = 1,
                FixedAmont = 2
            }.Calc(count).ShouldBe(result);
        }

        [Fact]
        public void CheckoutTest()
        {
            var goods = new Dictionary<Product, int>();
            goods.Add(new Product
            {
                Title = "beans",
                Strategy = new SimpleStrategy
                {
                    Price = 0.65m
                }
            }, 10);

            goods.Add(new Product
            {
                Title = "avocado",
                Strategy = new FixedPriceForAmountStrategy
                {
                    FixedAmount = 3,
                    FixedPrice = 3m,
                    Price = 1.25m
                }
            }, 3);

            goods.Add(new Product
            {
                Title = "soda",
                Strategy = new FreeForFixedAmountStrategy
                {
                    Price = 2.1m,
                    FreeAmount = 1,
                    FixedAmont = 2
                }
            }, 3);

            goods.Sum(x => x.Key.Strategy.Calc(x.Value)).ShouldBe(13.7m);

        }
    }

    public class Product
    {
        public string Title { get; set; }
        public IStrategy Strategy { get; set; }
    }

    public interface IStrategy
    {
        decimal Calc(int amount);
    }

    internal class FreeForFixedAmountStrategy : IStrategy
    {
        public decimal Price { get; set; }
        public int FreeAmount { get; set; }
        public int FixedAmont { get; set; }

        public decimal Calc(int count)
        {
            var freeAndPaid = FreeAmount + FixedAmont;
            var reminder = count % freeAndPaid;
            return reminder * Price + (count - reminder) / freeAndPaid * (FixedAmont * Price);
        }
    }

    internal class FixedPriceForAmountStrategy : IStrategy
    {
        public int FixedAmount { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal Price { get; set; }

        public decimal Calc(int count)
        {
            var reminder = count % FixedAmount;
            return reminder * Price + (count - reminder) / FixedAmount * FixedPrice;
        }
    }

    internal class SimpleStrategy : IStrategy
    {
        public decimal Price { get; set; }

        public decimal Calc(int count)
        {

            return count * Price;
        }
    }
}
