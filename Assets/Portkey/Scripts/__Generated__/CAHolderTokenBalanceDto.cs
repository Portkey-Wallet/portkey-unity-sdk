using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTokenBalanceDto
	{
		string chainId {get; set;}
		string caAddress {get; set;}
		TokenInfoDto tokenInfo {get; set;}
		long balance {get; set;}
		IList<long> tokenIds {get; set;}
	}
}