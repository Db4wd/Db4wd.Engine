namespace DbForward.Engine.Utilities;

internal static class BuilderExtensions
{
    internal static T AddBuildActions<T>(this T builder, IEnumerable<Action<T>> actions)
    {
        return actions
            .Aggregate(builder, (current, next) =>
            {
                next(current);
                return current;
            });
    }
}