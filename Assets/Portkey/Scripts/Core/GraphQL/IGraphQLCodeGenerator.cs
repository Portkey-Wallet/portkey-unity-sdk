namespace Portkey.Core
{
    /// <summary>
    /// An interface to generate GraphQL DTO classes.
    /// </summary>
    public interface IGraphQLCodeGenerator
    {
        /// <summary>Gnerate a DTO class from a GraphQL query.</summary>
        /// <param name="className">The name of the class to generate.</param>
        void GenerateDTOClass(string className);
    }
}