using System.Collections.Generic;

namespace Portkey.GraphQL
{
    /// <summary>
    /// An interface to GraphQL editor.
    /// </summary>
    public interface IGraphQLEditor
    {
#if UNITY_EDITOR
        /// <summary>Used for GraphQL Introspection.</summary>
        public void Introspect();
        /// <summary>Used to init schema after Introspection.</summary>
        public bool InitSchema();
        /// <summary>Function for creating a new custom query.</summary>
        public void CreateNewQuery();
        /// <summary>Function to edit custom query.</summary>
        /// <param name="query">The query to be edited.</param>
        public void EditQuery(GraphQLQuery query);
        /// <summary>Used to check if the type has subfields.</summary>
        /// <param name="typeName">The name of the type to check.</param>
        public bool CheckSubFields(string typeName);
        /// <summary>Used to add all available fields of a query to a custom query.</summary>
        /// <param name="query">The query to add fields to.</param>
        /// <param name="typeName">The name of the type to add fields from.</param>
        /// <param name="parent">The parent field of the fields to be added.</param>
        public void AddAllFields(GraphQLQuery query, string typeName, Field parent = null);
        /// <summary>Used to add a field to a custom query.</summary>
        /// <param name="query">The query to add fields to.</param>
        /// <param name="typeName">The name of the type to add fields from.</param>
        /// <param name="parent">The parent field of the fields to be added.</param>
        public void AddField(GraphQLQuery query, string typeName, Field parent);
        /// <summary>Used to get the return type of a query.</summary>
        /// <param name="query">The query to set the return type.</param>
        /// <param name="queryName">The name of the query.</param>
        public void GetQueryReturnType(GraphQLQuery query, string queryName);
        /// <summary>Delete custom query.</summary>
        /// <param name="query">The list of custom queries to delete the query from.</param>
        /// <param name="index">The index of the query to delete.</param>
        public void DeleteQuery(List<GraphQLQuery> query, int index);
        /// <summary>Delete all custom queries.</summary>
        public void DeleteAllQueries();

#endif

    }
}