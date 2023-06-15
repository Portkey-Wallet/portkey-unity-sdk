namespace Portkey.GraphQL
{
	public class TransferInfo
	{
		public string fromAddress {get; set;}
		public string fromCAAddress {get; set;}
		public string toAddress {get; set;}
		public long amount {get; set;}
		public string fromChainId {get; set;}
		public string toChainId {get; set;}
		public string transferTransactionId {get; set;}
	}
}