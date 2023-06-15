namespace Portkey.GraphQL
{
	public class TokenInfoDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string blockHash {get; set;}
		long blockHeight {get; set;}
		string previousBlockHash {get; set;}
		string symbol {get; set;}
		TokenType type {get; set;}
		string tokenContractAddress {get; set;}
		int decimals {get; set;}
		long totalSupply {get; set;}
		string tokenName {get; set;}
		string issuer {get; set;}
		bool isBurnable {get; set;}
		int issueChainId {get; set;}
	}
}