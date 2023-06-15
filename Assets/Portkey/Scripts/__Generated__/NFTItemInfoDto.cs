namespace Portkey.GraphQL
{
	public class NFTItemInfoDto
	{
		public string symbol {get; set;}
		public string tokenContractAddress {get; set;}
		public int decimals {get; set;}
		public long supply {get; set;}
		public long totalSupply {get; set;}
		public string tokenName {get; set;}
		public string issuer {get; set;}
		public bool isBurnable {get; set;}
		public int issueChainId {get; set;}
		public string imageUrl {get; set;}
		public string collectionSymbol {get; set;}
		public string collectionName {get; set;}
	}
}