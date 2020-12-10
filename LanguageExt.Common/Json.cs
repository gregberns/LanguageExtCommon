﻿using System;
using System.Collections.Generic;
using System.IO;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Newtonsoft.Json;

namespace LanguageExt.Common
{
    public static class Json
    {
        public static Either<Error, T> Deserialize<T>(Byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(data))
                using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                using (var jsonTextReader = new JsonTextReader(reader))
                {
                    try
                    {
                        return JsonSerializer.Create().Deserialize<T>(jsonTextReader);
                    }
                    catch (Exception e)
                    {
                        return Left<Error, T>(Error.New(e));
                    }

                }
            }
            catch (Exception e)
            {
                return Left<Error, T>(Error.New(e));
            }
        }

        public static Either<Error, T> Deserialize<T>(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {
                return Left<Error, T>(Error.New(e));
            }
        }
    }

    public static class JsonExt
    {
        public static Either<Error, T> DeserializeJson<T>(this string str) => Json.Deserialize<T>(str);
    }
}