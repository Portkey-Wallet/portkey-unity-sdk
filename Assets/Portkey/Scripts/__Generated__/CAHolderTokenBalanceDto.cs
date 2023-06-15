using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTokenBalanceDto
	{
		public string chainId {get; set;}
		public string caAddress {get; set;}
		public TokenInfoDto tokenInfo {get; set;}
		public long balance {get; set;}
		public IList<long> tokenIds {get; set;}
	}
}