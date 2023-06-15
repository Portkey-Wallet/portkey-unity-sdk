namespace Portkey.GraphQL
{
	public class CAHolderNFTBalanceInfoDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string caAddress {get; set;}
		NFTItemInfoDto nftInfo {get; set;}
	}
}