namespace Portkey.GraphQL
{
	public class NFTItemInfoDto
	{
		string symbol {get; set;}
		string tokenContractAddress {get; set;}
		int decimals {get; set;}
		long supply {get; set;}
		long totalSupply {get; set;}
		string tokenName {get; set;}
		string issuer {get; set;}
		bool isBurnable {get; set;}
		int issueChainId {get; set;}
		string imageUrl {get; set;}
		string collectionSymbol {get; set;}
		string collectionName {get; set;}
	}
}