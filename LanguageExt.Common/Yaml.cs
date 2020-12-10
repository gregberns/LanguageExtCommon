using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LanguageExt.Common
{
    public static class Yaml
    {

        public static Either<Error, T> ParseYamlFile<T>(string path) =>
            Try(() => System.IO.File.ReadAllText(path))
                .ToEither()
                .MapLeft(ex => Error.New($"Reading '{path}' file failed. Error: {ex.Message}", ex))
                .Bind(str => ParseYaml<T>(str)
                            .MapLeft(ex => Error.New($"Deserializing '{path}' file failed. Error: {ex.Message}", ex)));
        public static Either<Exception, T> ParseYaml<T>(string yaml) =>
            Try(() => new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build()
                    .Deserialize<T>(yaml))
                .ToEither();
        public static Either<Exception, string> SerializeToYaml<T>(T obj) =>
            Try(() => new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .DisableAliases()
                .Build()
                .Serialize(obj)
            ).ToEither();

    }
}