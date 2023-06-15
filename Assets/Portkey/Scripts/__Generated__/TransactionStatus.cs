namespace Portkey.GraphQL
{
	public enum TransactionStatus
	{
		NOT_EXISTED,
		PENDING,
		FAILED,
		MINED,
		CONFLICT,
		PENDING_VALIDATION,
		NODE_VALIDATION_FAILED,
	}
}