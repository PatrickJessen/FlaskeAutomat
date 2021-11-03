using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    class Consumer
    {
        // event that fires when we consume a bottle.
        public event EventHandler OnBottleConsumed;
        // main buffer that we send our flasks through first.
        public Bottle[] Flasks { get; set; }
        // sorted buffer that we send our beer flasks through.
        public Bottle[] BeerBuffer { get; set; }
        // sorted buffer that we send our soda flasks through
        public Bottle[] SodaBuffer { get; set; }
        private bool consumedBottle;
        public bool ConsumedBottle
        {
            get { return consumedBottle; }
            set
            {
                if (consumedBottle != value)
                {
                    consumedBottle = value;
                    TriggerOnBottleConsumed();
                }
            }
        }


        public int maxLength;
        private bool trigger;
        Bottle tempBottle;

        public Consumer(Bottle[] flasks, Bottle[] beerBuffer, Bottle[] sodaBuffer, int maxLength)
        {
            this.Flasks = flasks;
            this.maxLength = maxLength;
            this.BeerBuffer = beerBuffer;
            this.SodaBuffer = sodaBuffer;
        }

        public void Update()
        {
            while (true)
            {
                for (int i = 0; i < Flasks.Length; i++)
                {
                    if (Flasks[i] != null && maxLength -1 < Flasks.Length)
                    {
                        if (Flasks[i].Type == BottleType.ØL)
                            SortBottles(i, BeerBuffer);
                        else if (Flasks[i].Type == BottleType.Sodavand)
                            SortBottles(i, SodaBuffer);

                    }
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Sorts the bottles to the right buffer
        /// </summary>
        /// <param name="index">The index we get when we loop through the Flasks</param>
        /// <param name="buffer">The buffer you want your bottle sorted to</param>
        private void SortBottles(int index, Bottle[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == null)
                {
                    Monitor.Enter(this);
                    try
                    {
                        buffer[i] = Flasks[index];
                        tempBottle = Flasks[index];
                        Flasks[index] = null;
                        ConsumedBottle = !ConsumedBottle;
                        trigger = true;
                    }
                    finally
                    {
                        Monitor.PulseAll(this);
                    }
                    break;
                }
                else
                {
                    Monitor.Wait(this);
                    trigger = false;
                }
            }

            RemoveFromFinalBuffer(buffer);
        }

        /// <summary>
        /// Checks if our buffer length is equals to the max length of our buffers, then removes a bottle from the last buffer.
        /// </summary>
        /// <param name="buffer">The buffer you want to remove a bottle from</param>
        private void RemoveFromFinalBuffer(Bottle[] buffer)
        {
            if (buffer.Length == maxLength)
                buffer[0] = null;
        }

        /// <summary>
        /// Triggers an envent when we consume a bottle from the buffer and sends a string through the object sender that we print out in the manager.
        /// </summary>
        private void TriggerOnBottleConsumed()
        {
            EventHandler handler = OnBottleConsumed;
            if (handler != null)
            {
                if (trigger)
                    OnBottleConsumed($"Consumer consumed {tempBottle.Type} from the buffer", EventArgs.Empty);
                else
                    OnBottleConsumed($"Consumer is waiting...", EventArgs.Empty);
            }
        }
    }
}
