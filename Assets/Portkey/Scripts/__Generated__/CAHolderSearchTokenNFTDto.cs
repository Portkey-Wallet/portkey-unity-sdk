namespace Portkey.GraphQL
{
	public class CAHolderSearchTokenNFTDto
	{
		public string chainId {get; set;}
		public string caAddress {get; set;}
		public long balance {get; set;}
		public long tokenId {get; set;}
		public TokenInfoDto tokenInfo {get; set;}
		public NFTItemInfoDto nftInfo {get; set;}
	}
}