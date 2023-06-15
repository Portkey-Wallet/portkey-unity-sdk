using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderNFTCollectionBalanceInfoDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string caAddress {get; set;}
		public IList<long> tokenIds {get; set;}
		public NFTCollectionDto nftCollectionInfo {get; set;}
	}
}