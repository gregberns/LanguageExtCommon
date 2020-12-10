using System;
using System.Threading.Tasks;
using System.Diagnostics;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    public static class Process
    {
        public static Task<Either<Error, int>> RunProcessAsync(string workingDirectory, string fileName, string arguments, int timeout = 1000 * 60)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new System.Diagnostics.Process
            {
                StartInfo = {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                return Left<Error, int>(Error.New(e)).AsTask();
            }

            return tcs.Task.Map(i => Right<Error, int>(i));
        }
    }
}