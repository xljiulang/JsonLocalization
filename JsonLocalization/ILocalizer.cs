namespace JsonLocalization
{
    public interface ILocalizer<out TModel>
    {
        TModel Current { get; }

        TModel Get(string culture);
    }
}
