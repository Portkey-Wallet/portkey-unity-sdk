namespace Portkey.GraphQL
{
	public class TokenInfoDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string blockHash {get; set;}
		public long blockHeight {get; set;}
		public string previousBlockHash {get; set;}
		public string symbol {get; set;}
		public TokenType type {get; set;}
		public string tokenContractAddress {get; set;}
		public int decimals {get; set;}
		public long totalSupply {get; set;}
		public string tokenName {get; set;}
		public string issuer {get; set;}
		public bool isBurnable {get; set;}
		public int issueChainId {get; set;}
	}
}