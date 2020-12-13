using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExtCommon
{
    /// <summary>
    /// Wrapper around `System.Environment`
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// Get Environment Variable
        /// </summary>
        public static Validation<string, string> GetEnv(string env)
        {
            var val = System.Environment.GetEnvironmentVariable(env);
            return String.IsNullOrWhiteSpace(val)
                ? Fail<string, string>($"Invalid environmental variable: {env}")
                : Success<string, string>(val);
        }

        /// <summary>
        /// Get Environment Variable of type integer
        /// </summary>
        public static Validation<string, int> GetEnvInt(string env) =>
            GetEnv(env).Bind<int>(
                i => parseInt(i)
                    .ToEither($"Invalid environmental integer: {env} => {i}")
                    .ToValidation()
            );

        /// <summary>
        /// Get Environment Variable of type Uri (System.UriKind.RelativeOrAbsolute)
        /// </summary>
        public static Validation<string, Uri> GetEnvUri(string env) =>
            GetEnv(env)
                .Bind<Uri>(v =>
                    Web.ParseUri(UriKind.RelativeOrAbsolute, v)
                        .Match(
                            Some: res => Success<string, Uri>(res),
                            None: () => Fail<string, Uri>($"Invalid environmental variable. Not a valid Uri. EnvVar: '{env}', Value: '{v}'")
                        ));

        /// <summary>
        /// Get Environment Variable of type Absolute Uri (System.UriKind.Absolute)
        /// </summary>
        public static Validation<string, Uri> GetEnvUriAbs(string env) =>
            GetEnv(env)
                .Bind<Uri>(v =>
                    Web.ParseUri(UriKind.RelativeOrAbsolute, v)
                        .Match(
                            Some: res => Success<string, Uri>(res),
                            None: () => Fail<string, Uri>($"Invalid environmental variable. Not a valid Absolute Uri. EnvVar: '{env}', Value: '{v}'")
                        ));

        /// <summary>
        /// Get Environment Variable of type Relative Uri (System.UriKind.Relative
        /// </summary>
        public static Validation<string, Uri> GetEnvUriRel(string env) =>
            GetEnv(env)
                .Bind<Uri>(v =>
                    Web.ParseUri(UriKind.RelativeOrAbsolute, v)
                        .Match(
                            Some: res => Success<string, Uri>(res),
                            None: () => Fail<string, Uri>($"Invalid environmental variable. Not a valid Relative Uri. EnvVar: '{env}', Value: '{v}'")
                        ));
    }
}
