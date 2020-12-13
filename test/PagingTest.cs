using System.Net.Http;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using LanguageExtCommon;
using Xunit;

namespace LanguageExtCommonTest
{
    public class PagingTest
    {
        public class TestResponse
        {
            public int TotalItems;
            public Lst<string> Items;
            public override string ToString()
            {
                return $"{{TotalItems: {TotalItems}, Items: {Items}}}";
            }
        }

        public Task<Either<Error, Lst<TestResponse>>> Run(
            int pageSize,
            Func<int, EitherAsync<Error, TestResponse>> executeRequest
        )
        {
            var doContinue = fun((int pageNumber, TestResponse t) =>
                ((pageNumber + 1) * pageSize) < t.TotalItems);

            return Paging.PageAsync(executeRequest, doContinue);
        }

        [Fact]
        public async void Paging_Zero_Results()
        {
            var r1 = new TestResponse { TotalItems = 0, Items = new Lst<string>() };

            var a = await Run(3,
                fun((int i) =>
                    i == 0 ? RightAsync<Error, TestResponse>(r1)
                    : LeftAsync<Error, TestResponse>(Error.New("Oops"))
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Right<Error, Lst<TestResponse>>(List(r1)),
                a
            );
        }

        [Fact]
        public async void Paging_One_Results()
        {
            var r1 = new TestResponse { TotalItems = 1, Items = List("a") };

            var a = await Run(3,
                fun((int i) =>
                    i == 0 ? RightAsync<Error, TestResponse>(r1)
                    : LeftAsync<Error, TestResponse>(Error.New("Oops"))
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Right<Error, Lst<TestResponse>>(List(r1)),
                a
            );
        }

        [Fact]
        public async void Paging_Full_Page()
        {
            var r1 = new TestResponse { TotalItems = 3, Items = List("a", "b", "c") };

            var a = await Run(3,
                fun((int i) =>
                    i == 0 ? RightAsync<Error, TestResponse>(r1)
                    : LeftAsync<Error, TestResponse>(Error.New("Oops"))
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Right<Error, Lst<TestResponse>>(List(r1)),
                a
            );
        }

        [Fact]
        public async void Paging_TwoPages_OneItem()
        {
            var r1 = new TestResponse { TotalItems = 4, Items = List("a", "b", "c") };
            var r2 = new TestResponse { TotalItems = 4, Items = List("a") };

            var a = await Run(3,
                fun((int i) =>
                      i == 0 ? RightAsync<Error, TestResponse>(r1)
                    : i == 1 ? RightAsync<Error, TestResponse>(r2)
                    : LeftAsync<Error, TestResponse>(Error.New("Oops"))
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Right<Error, Lst<TestResponse>>(List(r1, r2)),
                a
            );
        }

        [Fact]
        public async void Paging_TwoPages_Full()
        {
            var r1 = new TestResponse { TotalItems = 6, Items = List("a", "b", "c") };
            var r2 = new TestResponse { TotalItems = 6, Items = List("d", "e", "f") };

            var a = await Run(3,
                fun((int i) =>
                      i == 0 ? RightAsync<Error, TestResponse>(r1)
                    : i == 1 ? RightAsync<Error, TestResponse>(r2)
                    : LeftAsync<Error, TestResponse>(Error.New("Oops"))
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Right<Error, Lst<TestResponse>>(List(r1, r2)),
                a
            );
        }

        [Fact]
        public async void Paging_Error_PageOne()
        {
            var r1 = LeftAsync<Error, TestResponse>(Error.New("Some error"));

            var a = await Run(3,
                fun((int i) =>
                    r1
                ));

            Console.WriteLine($"{a}");

            Assert.Equal(
                Left<Error, Lst<TestResponse>>(Error.New("Some error")),
                a
            );
        }
    }
}