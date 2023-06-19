using System.Collections;
using System.Collections.Generic;
using GraphQLCodeGen;

namespace Portkey.GraphQL
{
    /// <summary>Concatenated class for holder info with guardian.</summary>
    public class CaHolderWithGuardian
    {
        public Types.CAHolderManagerDto holderManagerInfo { get; set; }
        public IList<Types.LoginGuardianDto> loginGuardianInfo { get; set; }
    }
    
    /// <summary>Interface specifically for DID GraphQL to serve specialized calls to GraphQL.</summary>
    public interface IDIDGraphQL
    {
        /// <summary>GraphQL query for getting the holder info by manager.</summary>
        /// <param name="manager">The manager to get the holder info of.</param>
        /// <param name="chainId">The chain id related to the info to get from.</param>
        /// <param name="successCallback">Callback function when post of query is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs.</
        public IEnumerator GetHolderInfoByManager(string manager, string chainId, IGraphQL.successCallback<IList<CaHolderWithGuardian>> successCallback, IGraphQL.errorCallback errorCallback);
    }
}