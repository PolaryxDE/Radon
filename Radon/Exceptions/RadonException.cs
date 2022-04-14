namespace Radon.Exceptions;

/// <summary>
/// The <see cref="Exception"/> which gets thrown when a mechanism in Radon failed.
/// </summary>
internal class RadonException : Exception
{
    internal RadonException(string message) : base(message)
    {
    }
}