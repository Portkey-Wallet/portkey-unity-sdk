namespace Portkey.GraphQL
{
	public class TransferInfo
	{
		string fromAddress {get; set;}
		string fromCAAddress {get; set;}
		string toAddress {get; set;}
		long amount {get; set;}
		string fromChainId {get; set;}
		string toChainId {get; set;}
		string transferTransactionId {get; set;}
	}
}