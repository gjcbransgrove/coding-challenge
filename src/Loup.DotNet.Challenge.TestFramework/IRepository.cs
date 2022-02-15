namespace Loup.DotNet.Challenge.TestFramework
{
    public interface IRepository
    {
        public T Get<T>(int contentId);
    }
}
