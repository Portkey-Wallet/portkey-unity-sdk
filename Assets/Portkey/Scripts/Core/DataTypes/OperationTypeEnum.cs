namespace Portkey.Core
{
    public enum OperationTypeEnum
    {
        // unknown
        unknown = 0,
        // register
        register = 1,
        // community recovery
        communityRecovery = 2,
        // add guardian
        addGuardian = 3,
        // delete guardian
        deleteGuardian = 4,
        // edit guardian
        editGuardian = 5,
        // remove other manager
        removeOtherManager = 6,
        // set login account
        setLoginAccount = 7,
    }
}