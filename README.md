# transfermoney
- Using 2 solutions to solve this problem:
  + Using multi producer/one consumer + BlockCollection
  + Using ConcurrentDictionary to contains multi key and lock it in order
- Controller: AccountsController
- Database: LocalDb using CodeFirst to generate and seed data
- Database for testing: drop and generate each test.

+ API seed data: http://localhost:34921/accounts/seed?numOfAccount=2
+ API quick check: http://localhost:34921/accounts/QuickCheckResult?id1=1&id2=2
+ API for TransferUsingQueue: http://localhost:34921/accounts/TransferUsingQueue?FromAccountId=1&ToAccountId=2&Amount=100
+ API for TransferUsingLock: http://localhost:34921/accounts/TransferUsingLock?FromAccountId=1&ToAccountId=2&Amount=100
