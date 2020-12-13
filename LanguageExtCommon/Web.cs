using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExtCommon
{
    public static class Web
    {
        public static string UrlEncode(Dictionary<string, string> dict) =>
            String.Join(
                "&",
                dict.Map(y => $"{y.Key}={System.Net.WebUtility.UrlEncode(y.Value)}")
            );

        public static string UrlEncode(Map<string, string> dict) =>
             String.Join(
                "&",
                dict.Map(y => $"{y.Key}={System.Net.WebUtility.UrlEncode(y.Value)}")
            );

        /// <summary>
        /// Verify string is a valid Uri
        /// </summary>
        public static Validation<string, Uri> ToUri(string str) =>
            ParseUri(UriKind.RelativeOrAbsolute, str)
                .Match(
                    Some: res => Success<string, Uri>(res),
                    None: () => Fail<string, Uri>($"Invalid Uri: {str}")
                );

        /// <summary>
        /// Verify string is a valid Absolute Uri (System.UriKind.Absolute)
        /// </summary>
        public static Validation<string, Uri> ToUriAbs(string str) =>
            ParseUri(UriKind.Absolute, str)
                .Match(
                    Some: res => Success<string, Uri>(res),
                    None: () => Fail<string, Uri>($"Invalid Absolute Uri: {str}")
                );

        /// <summary>
        /// Verify string is a valid Relative Uri (System.UriKind.Relative)
        /// </summary>
        public static Validation<string, Uri> ToUriRel(string str) =>
            ParseUri(UriKind.Relative, str)
                .Match(
                    Some: res => Success<string, Uri>(res),
                    None: () => Fail<string, Uri>($"Invalid Relative Uri: {str}")
                );

        /// <summary>
        /// Verify string is a Uri
        /// </summary>
        public static Option<Uri> ParseUri(UriKind kind, string uri)
        {
            Uri result = null;
            return Uri.TryCreate(uri, kind, out result)
                ? Some<Uri>(result)
                : Option<Uri>.None;
        }
    }
}
