using System.Diagnostics;
using System.Text.Json;

namespace FlowingBot.Core.Infrastructure;

public class PythonRunner : IDisposable
{
    private readonly string _pythonScriptPath;
    private bool _isDisposed;

    public PythonRunner(string pythonScriptPath) =>
        _pythonScriptPath = pythonScriptPath ?? throw new ArgumentNullException(nameof(pythonScriptPath));

    public async Task<string> Run(params string[] arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{_pythonScriptPath} {string.Join(" ", arguments.Select(x => $"\"{x}\""))}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new InvalidOperationException($"Python script failed: {error}");
            }

            return output;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Python script failed!", ex);
        }
    }

    public async Task<T> Run<T>(params string[] arguments)
    {
        var output = await Run(arguments);

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            return JsonSerializer.Deserialize<T>(output, options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to convert JSON object!", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
            _isDisposed = true;
    }

    ~PythonRunner() => Dispose(false);
}