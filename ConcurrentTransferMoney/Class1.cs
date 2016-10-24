using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentTransferMoney
{
    class Program
    {
        private static void Main(string[] args)
        {
            int transfersCompleted = 0;
            BankAccount a = new BankAccount {Id = 1, Balance = 1000};
            BankAccount b = new BankAccount {Id = 2, Balance = 1000};

            Parallel.For(0, 10, i =>
            {
                Transfer(a, b, 100);
                Transfer(b, a, 100);
                transfersCompleted += 2;
            });
            Console.WriteLine(transfersCompleted);
            Console.WriteLine("Balance of A {0}", a.Balance);
            Console.WriteLine("Balance of B {0}", b.Balance);
        }

        private class BankAccount
        {
            public int Id;
            public int Balance;
        }

        static void Transfer(BankAccount one, BankAccount two, int amount)
        {
            if (one.Id < two.Id)
            {
                lock (one)
                {
                    Console.WriteLine("Locked one {0}", one.Id);
                    lock (two)
                    {
                        Console.WriteLine("Locked two {0}", two.Id);
                        Test(one, two, amount);
                    }
                }
            }
            else
            {
                lock (two)
                {
                    Console.WriteLine("Locked two {0}", two.Id);
                    lock (one)
                    {
                        Console.WriteLine("Locked one {0}", one.Id);
                        Test(one, two, amount);
                    }
                }
            }
        }
        static void Test(BankAccount one, BankAccount two, int amount)
        {
            one.Balance -= amount;
            two.Balance += amount;
        }
    }
}
