﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphQLCodeGen {
  public class Types {
    
    #region Query
    public class Query {
      #region members
      [JsonProperty("tokenInfo")]
      public List<TokenInfoDto> tokenInfo { get; set; }
    
      [JsonProperty("caHolderTransaction")]
      public CAHolderTransactionPageResultDto caHolderTransaction { get; set; }
    
      [JsonProperty("caHolderTransactionInfo")]
      public CAHolderTransactionPageResultDto caHolderTransactionInfo { get; set; }
    
      [JsonProperty("caHolderManagerInfo")]
      public List<CAHolderManagerDto> caHolderManagerInfo { get; set; }
    
      [JsonProperty("caHolderInfo")]
      public List<CAHolderInfoDto> caHolderInfo { get; set; }
    
      [JsonProperty("loginGuardianInfo")]
      public List<LoginGuardianDto> loginGuardianInfo { get; set; }
    
      [JsonProperty("caHolderNFTCollectionBalanceInfo")]
      public CAHolderNFTCollectionBalancePageResultDto caHolderNFTCollectionBalanceInfo { get; set; }
    
      [JsonProperty("caHolderNFTBalanceInfo")]
      public CAHolderNFTBalancePageResultDto caHolderNFTBalanceInfo { get; set; }
    
      [JsonProperty("caHolderTokenBalanceInfo")]
      public CAHolderTokenBalancePageResultDto caHolderTokenBalanceInfo { get; set; }
    
      [JsonProperty("caHolderTransactionAddressInfo")]
      public CAHolderTransactionAddressPageResultDto caHolderTransactionAddressInfo { get; set; }
    
      [JsonProperty("loginGuardianChangeRecordInfo")]
      public List<LoginGuardianChangeRecordDto> loginGuardianChangeRecordInfo { get; set; }
    
      [JsonProperty("caHolderManagerChangeRecordInfo")]
      public List<CAHolderManagerChangeRecordDto> caHolderManagerChangeRecordInfo { get; set; }
    
      [JsonProperty("caHolderSearchTokenNFT")]
      public CAHolderSearchTokenNFTPageResultDto caHolderSearchTokenNFT { get; set; }
    
      [JsonProperty("syncState")]
      public SyncStateDto syncState { get; set; }
      #endregion
    }
    #endregion
    
    #region TokenInfoDto
    public class TokenInfoDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("blockHash")]
      public string blockHash { get; set; }
    
      [JsonProperty("blockHeight")]
      public long blockHeight { get; set; }
    
      [JsonProperty("previousBlockHash")]
      public string previousBlockHash { get; set; }
    
      [JsonProperty("symbol")]
      public string symbol { get; set; }
    
      [JsonProperty("type")]
      public TokenType type { get; set; }
    
      [JsonProperty("tokenContractAddress")]
      public string tokenContractAddress { get; set; }
    
      [JsonProperty("decimals")]
      public int decimals { get; set; }
    
      [JsonProperty("totalSupply")]
      public long totalSupply { get; set; }
    
      [JsonProperty("tokenName")]
      public string tokenName { get; set; }
    
      [JsonProperty("issuer")]
      public string issuer { get; set; }
    
      [JsonProperty("isBurnable")]
      public bool isBurnable { get; set; }
    
      [JsonProperty("issueChainId")]
      public int issueChainId { get; set; }
      #endregion
    }
    #endregion
    public enum TokenType {
      TOKEN,
      NFT_COLLECTION,
      NFT_ITEM
    }
    
    
    #region GetTokenInfoDto
    public class GetTokenInfoDto {
      #region members
      public string symbol { get; set; }
    
      public string chainId { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderTransactionPageResultDto
    public class CAHolderTransactionPageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderTransactionDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderTransactionDto
    public class CAHolderTransactionDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("blockHash")]
      public string blockHash { get; set; }
    
      [JsonProperty("blockHeight")]
      public long blockHeight { get; set; }
    
      [JsonProperty("previousBlockHash")]
      public string previousBlockHash { get; set; }
    
      [JsonProperty("transactionId")]
      public string transactionId { get; set; }
    
      [JsonProperty("methodName")]
      public string methodName { get; set; }
    
      [JsonProperty("tokenInfo")]
      public TokenInfoDto tokenInfo { get; set; }
    
      [JsonProperty("nftInfo")]
      public NFTItemInfoDto nftInfo { get; set; }
    
      [JsonProperty("status")]
      public TransactionStatus status { get; set; }
    
      [JsonProperty("timestamp")]
      public long timestamp { get; set; }
    
      [JsonProperty("transferInfo")]
      public TransferInfo transferInfo { get; set; }
    
      [JsonProperty("fromAddress")]
      public string fromAddress { get; set; }
    
      [JsonProperty("transactionFees")]
      public List<TransactionFee> transactionFees { get; set; }
      #endregion
    }
    #endregion
    
    #region NFTItemInfoDto
    public class NFTItemInfoDto {
      #region members
      [JsonProperty("symbol")]
      public string symbol { get; set; }
    
      [JsonProperty("tokenContractAddress")]
      public string tokenContractAddress { get; set; }
    
      [JsonProperty("decimals")]
      public int decimals { get; set; }
    
      [JsonProperty("supply")]
      public long supply { get; set; }
    
      [JsonProperty("totalSupply")]
      public long totalSupply { get; set; }
    
      [JsonProperty("tokenName")]
      public string tokenName { get; set; }
    
      [JsonProperty("issuer")]
      public string issuer { get; set; }
    
      [JsonProperty("isBurnable")]
      public bool isBurnable { get; set; }
    
      [JsonProperty("issueChainId")]
      public int issueChainId { get; set; }
    
      [JsonProperty("imageUrl")]
      public string imageUrl { get; set; }
    
      [JsonProperty("collectionSymbol")]
      public string collectionSymbol { get; set; }
    
      [JsonProperty("collectionName")]
      public string collectionName { get; set; }
      #endregion
    }
    #endregion
    public enum TransactionStatus {
      NOT_EXISTED,
      PENDING,
      FAILED,
      MINED,
      CONFLICT,
      PENDING_VALIDATION,
      NODE_VALIDATION_FAILED
    }
    
    
    #region TransferInfo
    public class TransferInfo {
      #region members
      [JsonProperty("fromAddress")]
      public string fromAddress { get; set; }
    
      [JsonProperty("fromCAAddress")]
      public string fromCAAddress { get; set; }
    
      [JsonProperty("toAddress")]
      public string toAddress { get; set; }
    
      [JsonProperty("amount")]
      public long amount { get; set; }
    
      [JsonProperty("fromChainId")]
      public string fromChainId { get; set; }
    
      [JsonProperty("toChainId")]
      public string toChainId { get; set; }
    
      [JsonProperty("transferTransactionId")]
      public string transferTransactionId { get; set; }
      #endregion
    }
    #endregion
    
    #region TransactionFee
    public class TransactionFee {
      #region members
      [JsonProperty("symbol")]
      public string symbol { get; set; }
    
      [JsonProperty("amount")]
      public long amount { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderTransactionDto
    public class GetCaHolderTransactionDto {
      #region members
      public string chainId { get; set; }
    
      
      [JsonRequired]
      public long startBlockHeight { get; set; }
    
      
      [JsonRequired]
      public long endBlockHeight { get; set; }
    
      public string symbol { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      public string transactionId { get; set; }
    
      public string blockHash { get; set; }
    
      public string transferTransactionId { get; set; }
    
      public List<string> methodNames { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderManagerDto
    public class CAHolderManagerDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caHash")]
      public string caHash { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("managerInfos")]
      public List<ManagerInfo> managerInfos { get; set; }
    
      [JsonProperty("originChainId")]
      public string originChainId { get; set; }
      #endregion
    }
    #endregion
    
    #region ManagerInfo
    public class ManagerInfo {
      #region members
      [JsonProperty("address")]
      public string address { get; set; }
    
      [JsonProperty("extraData")]
      public string extraData { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderManagerInfoDto
    public class GetCaHolderManagerInfoDto {
      #region members
      public string chainId { get; set; }
    
      public string caHash { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      public string manager { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderInfoDto
    public class CAHolderInfoDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caHash")]
      public string caHash { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("managerInfos")]
      public List<ManagerInfo> managerInfos { get; set; }
    
      [JsonProperty("originChainId")]
      public string originChainId { get; set; }
    
      [JsonProperty("guardianList")]
      public GuardianList guardianList { get; set; }
      #endregion
    }
    #endregion
    
    #region GuardianList
    public class GuardianList {
      #region members
      [JsonProperty("guardians")]
      public List<Guardian> guardians { get; set; }
      #endregion
    }
    #endregion
    
    #region Guardian
    public class Guardian {
      #region members
      [JsonProperty("type")]
      public int type { get; set; }
    
      [JsonProperty("verifierId")]
      public string verifierId { get; set; }
    
      [JsonProperty("identifierHash")]
      public string identifierHash { get; set; }
    
      [JsonProperty("salt")]
      public string salt { get; set; }
    
      [JsonProperty("isLoginGuardian")]
      public bool isLoginGuardian { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderInfoDto
    public class GetCaHolderInfoDto {
      #region members
      public string chainId { get; set; }
    
      public string caHash { get; set; }
    
      public List<string> caAddress { get; set; }
    
      public string loginGuardianIdentifierHash { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region LoginGuardianDto
    public class LoginGuardianDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caHash")]
      public string caHash { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("manager")]
      public string manager { get; set; }
    
      [JsonProperty("loginGuardian")]
      public GuardianDto loginGuardian { get; set; }
      #endregion
    }
    #endregion
    
    #region GuardianDto
    public class GuardianDto {
      #region members
      [JsonProperty("type")]
      public int type { get; set; }
    
      [JsonProperty("verifierId")]
      public string verifierId { get; set; }
    
      [JsonProperty("identifierHash")]
      public string identifierHash { get; set; }
    
      [JsonProperty("salt")]
      public string salt { get; set; }
    
      [JsonProperty("isLoginGuardian")]
      public bool isLoginGuardian { get; set; }
      #endregion
    }
    #endregion
    
    #region GetLoginGuardianInfoDto
    public class GetLoginGuardianInfoDto {
      #region members
      public string chainId { get; set; }
    
      public string caHash { get; set; }
    
      public string caAddress { get; set; }
    
      public string loginGuardian { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderNFTCollectionBalancePageResultDto
    public class CAHolderNFTCollectionBalancePageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderNFTCollectionBalanceInfoDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderNFTCollectionBalanceInfoDto
    public class CAHolderNFTCollectionBalanceInfoDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("tokenIds")]
      public List<long> tokenIds { get; set; }
    
      [JsonProperty("nftCollectionInfo")]
      public NFTCollectionDto nftCollectionInfo { get; set; }
      #endregion
    }
    #endregion
    
    #region NFTCollectionDto
    public class NFTCollectionDto {
      #region members
      [JsonProperty("symbol")]
      public string symbol { get; set; }
    
      [JsonProperty("tokenContractAddress")]
      public string tokenContractAddress { get; set; }
    
      [JsonProperty("decimals")]
      public int decimals { get; set; }
    
      [JsonProperty("supply")]
      public long supply { get; set; }
    
      [JsonProperty("totalSupply")]
      public long totalSupply { get; set; }
    
      [JsonProperty("tokenName")]
      public string tokenName { get; set; }
    
      [JsonProperty("issuer")]
      public string issuer { get; set; }
    
      [JsonProperty("isBurnable")]
      public bool isBurnable { get; set; }
    
      [JsonProperty("issueChainId")]
      public int issueChainId { get; set; }
    
      [JsonProperty("imageUrl")]
      public string imageUrl { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderNftCollectionInfoDto
    public class GetCaHolderNftCollectionInfoDto {
      #region members
      public string chainId { get; set; }
    
      public string symbol { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderNFTBalancePageResultDto
    public class CAHolderNFTBalancePageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderNFTBalanceInfoDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderNFTBalanceInfoDto
    public class CAHolderNFTBalanceInfoDto {
      #region members
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("balance")]
      public long balance { get; set; }
    
      [JsonProperty("nftInfo")]
      public NFTItemInfoDto nftInfo { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderNftInfoDto
    public class GetCaHolderNftInfoDto {
      #region members
      public string chainId { get; set; }
    
      public string symbol { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      public string collectionSymbol { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderTokenBalancePageResultDto
    public class CAHolderTokenBalancePageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderTokenBalanceDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderTokenBalanceDto
    public class CAHolderTokenBalanceDto {
      #region members
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("tokenInfo")]
      public TokenInfoDto tokenInfo { get; set; }
    
      [JsonProperty("balance")]
      public long balance { get; set; }
    
      [JsonProperty("tokenIds")]
      public List<long> tokenIds { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderTokenBalanceDto
    public class GetCaHolderTokenBalanceDto {
      #region members
      public string chainId { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      public string symbol { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderTransactionAddressPageResultDto
    public class CAHolderTransactionAddressPageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderTransactionAddressDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderTransactionAddressDto
    public class CAHolderTransactionAddressDto {
      #region members
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("address")]
      public string address { get; set; }
    
      [JsonProperty("addressChainId")]
      public string addressChainId { get; set; }
    
      [JsonProperty("transactionTime")]
      public long transactionTime { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderTransactionAddressDto
    public class GetCaHolderTransactionAddressDto {
      #region members
      public string chainId { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region LoginGuardianChangeRecordDto
    public class LoginGuardianChangeRecordDto {
      #region members
      [JsonProperty("changeType")]
      public string changeType { get; set; }
    
      [JsonProperty("blockHeight")]
      public long blockHeight { get; set; }
    
      [JsonProperty("id")]
      public string id { get; set; }
    
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caHash")]
      public string caHash { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("manager")]
      public string manager { get; set; }
    
      [JsonProperty("loginGuardian")]
      public GuardianDto loginGuardian { get; set; }
      #endregion
    }
    #endregion
    
    #region GetLoginGuardianChangeRecordDto
    public class GetLoginGuardianChangeRecordDto {
      #region members
      public string chainId { get; set; }
    
      
      [JsonRequired]
      public long startBlockHeight { get; set; }
    
      
      [JsonRequired]
      public long endBlockHeight { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderManagerChangeRecordDto
    public class CAHolderManagerChangeRecordDto {
      #region members
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("caHash")]
      public string caHash { get; set; }
    
      [JsonProperty("manager")]
      public string manager { get; set; }
    
      [JsonProperty("changeType")]
      public string changeType { get; set; }
    
      [JsonProperty("blockHeight")]
      public long blockHeight { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderManagerChangeRecordDto
    public class GetCaHolderManagerChangeRecordDto {
      #region members
      public string chainId { get; set; }
    
      
      [JsonRequired]
      public long startBlockHeight { get; set; }
    
      
      [JsonRequired]
      public long endBlockHeight { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region CAHolderSearchTokenNFTPageResultDto
    public class CAHolderSearchTokenNFTPageResultDto {
      #region members
      [JsonProperty("totalRecordCount")]
      public long totalRecordCount { get; set; }
    
      [JsonProperty("data")]
      public List<CAHolderSearchTokenNFTDto> data { get; set; }
      #endregion
    }
    #endregion
    
    #region CAHolderSearchTokenNFTDto
    public class CAHolderSearchTokenNFTDto {
      #region members
      [JsonProperty("chainId")]
      public string chainId { get; set; }
    
      [JsonProperty("caAddress")]
      public string caAddress { get; set; }
    
      [JsonProperty("balance")]
      public long balance { get; set; }
    
      [JsonProperty("tokenId")]
      public long tokenId { get; set; }
    
      [JsonProperty("tokenInfo")]
      public TokenInfoDto tokenInfo { get; set; }
    
      [JsonProperty("nftInfo")]
      public NFTItemInfoDto nftInfo { get; set; }
      #endregion
    }
    #endregion
    
    #region GetCaHolderSearchTokenNftDto
    public class GetCaHolderSearchTokenNftDto {
      #region members
      public string chainId { get; set; }
    
      public List<string> caAddresses { get; set; }
    
      public string searchWord { get; set; }
    
      
      [JsonRequired]
      public int skipCount { get; set; }
    
      
      [JsonRequired]
      public int maxResultCount { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region SyncStateDto
    public class SyncStateDto {
      #region members
      [JsonProperty("confirmedBlockHeight")]
      public long confirmedBlockHeight { get; set; }
      #endregion
    }
    #endregion
    
    #region GetSyncStateDto
    public class GetSyncStateDto {
      #region members
      public string chainId { get; set; }
    
      
      [JsonRequired]
      public BlockFilterType filterType { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    public enum BlockFilterType {
      BLOCK,
      TRANSACTION,
      LOG_EVENT
    }
    
  }
  
}
