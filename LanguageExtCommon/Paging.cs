using System.Net.Http;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExtCommon
{
    /// <summary>
    /// Provides wrappers around `System.Net.Http`
    /// </summary>
    public static class Paging
    {
        public static EitherAsync<Error, Lst<T>> Page<T>(
            Func<int, EitherAsync<Error, T>> executeRequest,
            Func<int, T, bool> doContinue) =>
            PageAsync(executeRequest, doContinue).ToAsync();

        public static async Task<Either<Error, Lst<T>>> PageAsync<T>(
            Func<int, EitherAsync<Error, T>> executeRequest,
            Func<int, T, bool> doContinue)
        {
            var iteration = 0;
            var ls = new Lst<T>();

            while (true)
            {
                var res = await (executeRequest(iteration).ToEither());
                if (res.IsLeft)
                {
                    return res.Map(r => List(r));
                }
                var cont = res.Match(
                    Right: r =>
                    {
                        ls = ls.Add(r);
                        return doContinue(iteration, r);
                    },
                    Left: e =>
                    {
                        return false;
                    }
                );
                if (cont)
                {
                    iteration++;
                    continue;
                }
                else
                {
                    break;
                }
            }
            return Right(ls);
        }
    }
}