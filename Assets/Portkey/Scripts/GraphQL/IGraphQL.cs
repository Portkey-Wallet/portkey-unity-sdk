namespace Portkey.GraphQL
{
    public interface IGraphQL
    {
        public void GetHolderInfoByManager(string manager, string chainId);
        public T Query<T>(string query);
        public T Query<T>(GraphQLQuery query);
        public GraphQLQuery GetQueryByName(string queryName);
        public void SetAuthToken(string auth);
    }
}