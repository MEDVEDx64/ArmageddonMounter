namespace ArmageddonMounter.Wrappers
{
    public interface IConversionWrapper
    {
        byte[] ToExternal(byte[] bytes);
        byte[] ToInternal(byte[] bytes);
    }
}
