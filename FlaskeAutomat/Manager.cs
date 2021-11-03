using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    class Manager
    {
        Bottle[] flasks;
        Bottle[] beerBuffer;
        Bottle[] sodaBuffer;
        Consumer cons;
        Producer pro;
        const int maxLength = 10;
        public Manager()
        {
            flasks = new Bottle[maxLength];
            beerBuffer = new Bottle[maxLength];
            sodaBuffer = new Bottle[maxLength];
            cons = new Consumer(flasks, beerBuffer, sodaBuffer, maxLength);
            pro = new Producer(flasks);

            pro.OnBottleAdded += Pro_OnBottleAdded;
            cons.OnBottleConsumed += Cons_OnBottleConsumed;
        }

        private void Cons_OnBottleConsumed(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
        }

        private void Pro_OnBottleAdded(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
        }

        public void Start()
        {
            Thread t1 = new Thread(cons.Update);
            Thread t2 = new Thread(pro.Update);

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }
    }
}
