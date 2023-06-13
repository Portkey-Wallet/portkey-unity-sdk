using System.Collections.Generic;

namespace Portkey.GraphQL
{
    public interface IGraphQLCodeGenerator
    {
        void GenerateDTOClass(string className, List<Introspection.SchemaClass.Data.Schema.Type.Field> fields);
    }
}