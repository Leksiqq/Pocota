namespace Net.Leksi.Pocota.Core;
/// <summary>
/// <para xml:lang="ru">
/// Исключение, выбрасываемое при попытке неэксклюзивного доступа к первичному ключу во время присвоения его полей.
/// </para>
/// <para xml:lang="en">
/// The exception thrown when attempting non-exclusive access to the primary key while assigning its fields.
/// </para>
/// </summary>
public class KeyRingConcurrentException: Exception
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public KeyRingConcurrentException() : base() { }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public KeyRingConcurrentException(string message) : base(message) { }
}
