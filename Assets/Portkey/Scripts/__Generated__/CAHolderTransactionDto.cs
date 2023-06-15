using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string blockHash {get; set;}
		long blockHeight {get; set;}
		string previousBlockHash {get; set;}
		string transactionId {get; set;}
		string methodName {get; set;}
		TokenInfoDto tokenInfo {get; set;}
		NFTItemInfoDto nftInfo {get; set;}
		TransactionStatus status {get; set;}
		long timestamp {get; set;}
		TransferInfo transferInfo {get; set;}
		string fromAddress {get; set;}
		IList<TransactionFee> transactionFees {get; set;}
	}
}