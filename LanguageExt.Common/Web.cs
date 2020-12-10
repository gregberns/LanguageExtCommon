using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;


namespace LanguageExt.Common
{
    public static class Web
    {
        public static string UrlEncode(Dictionary<string, string> dict)
        {
            return String.Join(
                "&",
                dict.Map(y => $"{y.Key}={System.Net.WebUtility.UrlEncode(y.Value)}")
            );
        }

        public static string UrlEncode(Map<string, string> dict)
        {
            return String.Join(
                "&",
                dict.Map(y => $"{y.Key}={System.Net.WebUtility.UrlEncode(y.Value)}")
            );
        }

        public static Validation<string, Uri> ToUri(string str)
        {
            Uri res;
            return Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out res)
                ? Success<string, Uri>(res)
                : Fail<string, Uri>($"Invalid environmental variable: {str}");
        }

        public static Option<Uri> ParseUri(string uri)
        {
            Uri result = null;
            return Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out result)
                ? Some<Uri>(result)
                : Option<Uri>.None;
        }
    }
}
