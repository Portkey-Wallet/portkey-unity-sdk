namespace Portkey.GraphQL
{
	public class CAHolderTransactionAddressDto
	{
		public string chainId {get; set;}
		public string caAddress {get; set;}
		public string address {get; set;}
		public string addressChainId {get; set;}
		public long transactionTime {get; set;}
	}
}