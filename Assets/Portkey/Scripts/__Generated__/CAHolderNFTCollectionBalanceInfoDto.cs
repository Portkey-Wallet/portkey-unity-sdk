using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderNFTCollectionBalanceInfoDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string caAddress {get; set;}
		IList<long> tokenIds {get; set;}
		NFTCollectionDto nftCollectionInfo {get; set;}
	}
}