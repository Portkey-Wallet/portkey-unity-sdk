namespace Portkey.GraphQL
{
	public class CAHolderSearchTokenNFTDto
	{
		string chainId {get; set;}
		string caAddress {get; set;}
		long balance {get; set;}
		long tokenId {get; set;}
		TokenInfoDto tokenInfo {get; set;}
		NFTItemInfoDto nftInfo {get; set;}
	}
}