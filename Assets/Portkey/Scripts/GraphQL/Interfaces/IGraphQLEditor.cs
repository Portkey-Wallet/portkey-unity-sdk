using System.Collections.Generic;
using System.Linq;
using Portkey.GraphQL;

namespace Portkey.GraphQL
{
    public interface IGraphQLEditor
    {
#if UNITY_EDITOR

        public void Introspect();
        public bool InitSchema();
        public void CreateNewQuery();
        public void EditQuery(GraphQLQuery query);
        public bool CheckSubFields(string typeName);
        public void AddField(GraphQLQuery query, string typeName, Field parent);
        public void GetQueryReturnType(GraphQLQuery query, string queryName);
        public void DeleteQuery(List<GraphQLQuery> query, int index);
        public void DeleteAllQueries();

#endif

    }
}