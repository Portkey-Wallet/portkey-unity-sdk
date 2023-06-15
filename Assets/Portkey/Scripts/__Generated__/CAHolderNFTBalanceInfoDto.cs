namespace Portkey.GraphQL
{
	public class CAHolderNFTBalanceInfoDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string caAddress {get; set;}
		public long balance {get; set;}
		public NFTItemInfoDto nftInfo {get; set;}
	}
}