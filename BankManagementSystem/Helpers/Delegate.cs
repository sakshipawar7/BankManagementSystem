namespace BankManagementSystem.Helpers;
using BankManagementSystem.Services;
using BankManagementSystem.DAL;
using BankManagementSystem.Repo;

public delegate IAccountService AccountServiceDelegate(AccountType type);
//public delegate IAccountSavingsOrCurrent AccountServiceDelegate2(AccountType type);
