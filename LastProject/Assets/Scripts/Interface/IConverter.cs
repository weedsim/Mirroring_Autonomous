public interface IConverter<in In, out Out>
{
    public Out Convert(In input);
}
