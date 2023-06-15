namespace Portkey.GraphQL
{
	public class CAHolderTransactionAddressDto
	{
		string chainId {get; set;}
		string caAddress {get; set;}
		string address {get; set;}
		string addressChainId {get; set;}
		long transactionTime {get; set;}
	}
}