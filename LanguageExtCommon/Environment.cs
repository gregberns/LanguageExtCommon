using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExtCommon
{
    public static class Environment
    {
        public static Validation<string, string> GetEnv(string env)
        {
            var val = System.Environment.GetEnvironmentVariable(env);
            return String.IsNullOrWhiteSpace(val)
                ? Fail<string, string>($"Invalid environmental variable: {env}")
                : Success<string, string>(val);
        }
        static Validation<string, int> GetEnvInt(string env) =>
            GetEnv(env).Bind<int>(
                i => parseInt(i)
                    .ToEither($"Invalid environmental integer: {env} => {i}")
                    .ToValidation()
            );
        static Validation<string, Uri> GetEnvUri(string env) =>
            GetEnv(env).Bind<Uri>(
                u => Web.ParseUri(env)
                    .ToEither($"Invalid environmental variable Uri: {env} => {u}")
                    .ToValidation()
            );
    }
}
