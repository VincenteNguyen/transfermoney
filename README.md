# transfermoney
- Using 2 solutions to solve this problem:
  + Using multi producer/one consumer + BlockCollection
  + Using ConcurrentDictionary to contains multi key and lock it in order
- Controller: AccountsController
- Database: LocalDb using CodeFirst to generate and seed data
- Database for testing: drop and generate each test.

+ API for TransferUsingQueue: .../accounts/TransferUsingQueue
+ API for TransferUsingLock: .../accounts/TransferUsingLock
