using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string blockHash {get; set;}
		public long blockHeight {get; set;}
		public string previousBlockHash {get; set;}
		public string transactionId {get; set;}
		public string methodName {get; set;}
		public TokenInfoDto tokenInfo {get; set;}
		public NFTItemInfoDto nftInfo {get; set;}
		public TransactionStatus status {get; set;}
		public long timestamp {get; set;}
		public TransferInfo transferInfo {get; set;}
		public string fromAddress {get; set;}
		public IList<TransactionFee> transactionFees {get; set;}
	}
}